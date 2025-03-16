using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CombatPhase
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    END
}

public class CombatManagerScript : MonoBehaviour
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public float revengeMeter;

    public float judgementMeter;

    public bool judgementFlag;

    public float finalBossRevengeMultiplier = 1.42f;

    public CombatPhase currentPhase;

    public List<EnemyBaseScript> enemiesFront;

    public List<EnemyBaseScript> enemiesRear;

    public PlayerBehaviourScript playerReference;

    private PlayerManagerScript playerManagerReference;

    private AudioManagerScript audioManagerReference;

    public Button judgementButtonReference;

    public Button judgementPureButtonReference;

    public GameObject equipmentButtonPrefab;

    private List<GameObject> equipmentButtonReferences;

    private GameObject equipmentInfoPanelReference;

    protected bool canTurnProceed;

    public bool canPlayerAct;

    private bool dysphoriaFlag;

    // Used for specific functions such as crossbow healing
    public float previousPlayerDamageDealt;

    // Used for specific functions such as greataxe healing
    public int previousHitEnemyCount;

    // Used for banish augment
    public EquipmentBaseScript previousUsedEquipment;

    //UI
    public UIDamageScript damageUIReference;

    public UICombatDescriptionScript combatDescriptionUIReference;

    private bool combatDescriptionShowing;

    public float combatDescriptionSpeed = 4;

    public float uiDisplayTime = 1.0f;

    public float uiCoroutineTickTime = 0.1f;

    public GameObject adjusterIconPrefab;

    public GameObject enemyPlacementIndicatorPrefab;

    public GameObject damageNumberPrefab;

    public GameObject gameOverScreenPrefab;

    public GameObject enemyDeathParticlePrefab;

    [Header("Combat Spawn Prefabs")]
    public List<GameObject> enemyPrefabs;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//


    // Use this for initialization
    void Start()
    {
        playerManagerReference = GetComponent<PlayerManagerScript>();
        audioManagerReference = GetComponent<AudioManagerScript>();
        equipmentButtonReferences = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    static public CombatManagerScript GetCombatManager()
    {
        if (FindObjectOfType<CombatManagerScript>())
        {
            return FindObjectOfType<CombatManagerScript>().GetComponent<CombatManagerScript>();
        }
        else
        {
            return null;
        }
    }

    // Called when the combat scene is loaded to spawn all relevant things
    public void InitialiseCombatScene()
    {
        // Make sure judgement flag is false
        judgementFlag = false;

        // Find player reference
        playerReference = FindObjectOfType<PlayerBehaviourScript>();

        // Reset greataxe achievement counter
        GetComponent<AchievementManagerScript>().ResetGreataxeHealing();
        GetComponent<AchievementManagerScript>().ResetWarhammerWTSlow();

        // Check if comfy mode is on
        if (FindObjectOfType<GameManagerScript>().comfyModeFlag == true)
        {
            playerReference.health = 150.0f;
            playerReference.maxHealth = 150.0f;
            playerReference.GetComponentInChildren<UIHealthMeterScript>().ResetMaxHealthValue();
        }

        // Find the combat description reference
        combatDescriptionUIReference = FindObjectOfType<UICombatDescriptionScript>();

        // Find the damage ui reference
        damageUIReference = FindObjectOfType<UIDamageScript>();

        // Find the judgement ui button reference
        judgementButtonReference = FindObjectOfType<UIJudgementScript>().GetComponentInParent<Button>();

        // Find the judgement pure ui button reference
        judgementPureButtonReference = FindObjectOfType<UIJudgementPureButtonScript>().GetComponent<Button>();

        // Spawn enemies depending on the current game state
        SpawnEnemies();

        // Spawn equipment using player manager's combat inventory
        SpawnEquipmentButtons();

        // Find equipment panel object
        equipmentInfoPanelReference = GameObject.Find("EquipmentInfoPanel");

        // Turn off equipment info panel
        SetEquipmentPanelVisibility(false);
    }

    // Used to start combat
    public void CommenceCombatSimulation()
    {
        print("Starting simulation...");

        playerReference.wait = Mathf.Floor(Random.Range(10.0f, 13.0f));
        for (int i = 0; i < enemiesFront.Count; i++)
        {
            enemiesFront[i].wait = Mathf.Floor(Random.Range(15.0f, 32.0f));
        }
        for (int j = 0; j < enemiesRear.Count; j++)
        {
            enemiesRear[j].wait = Mathf.Floor(Random.Range(15.0f, 32.0f));
        }
        canTurnProceed = true;

        // Start combat by using turn end function
        NotifyTurnComplete();
    }

    // Used at the end of turns to figure out stuff for the next turn
    // This includes finding the next lowest wait value and handling tickdowns
    public void NotifyTurnComplete()
    {
        canPlayerAct = false;

        // Make sure to clean up dead enemies
        CleanUpDeadEnemies();

        // Make player lowest wait first
        float lowestWait = playerReference.wait;
        EntityBaseScript nextEntity = playerReference;

        // Check front and back row enemies if they got lower wait
        for (int i = 0; i < enemiesFront.Count; i++)
        {
            if (enemiesFront[i].wait < lowestWait)
            {
                lowestWait = enemiesFront[i].wait;
                nextEntity = enemiesFront[i];
            }
        }

        for (int i = 0; i < enemiesRear.Count; i++)
        {
            if (enemiesRear[i].wait < lowestWait)
            {
                lowestWait = enemiesRear[i].wait;
                nextEntity = enemiesRear[i];
            }
        }

        // Make sure game is not over
        if (currentPhase != CombatPhase.END)
        {
            // Set the phase
            if (nextEntity == playerReference)
            {
                currentPhase = CombatPhase.PLAYERTURN;
            }
            else
            {
                currentPhase = CombatPhase.ENEMYTURN;
            }

            StartCoroutine(HandleInBetweenTurns(lowestWait, nextEntity));
        }
    }

    // Notify Turn End part 2, deals with ui stuff
    private IEnumerator HandleInBetweenTurns(float lowestWait, EntityBaseScript nextEntity)
    {
        // Check for dysphoria
        if (dysphoriaFlag == true)
        {
            StartCoroutine(HandleEnemyDysphoriaUI());
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Set flag to false
            dysphoriaFlag = false;
        }

        print("Wait reduced by " + lowestWait);

        // Lower all entity's wait timers and add wait stacks

        // Slowly change wait for a certain amount of ticks
        float waitPerTick = lowestWait / (uiDisplayTime / uiCoroutineTickTime);

        print("Wait per tick: " + waitPerTick);

        // Cuz im dumb
        float waitTimeCounter = 0;

        for (int i = 0; i < Mathf.Ceil(uiDisplayTime / uiCoroutineTickTime) - 1; i++)
        {
            playerReference.wait -= waitPerTick;

            for (int j = 0; j < enemiesFront.Count; j++)
            {
                enemiesFront[j].wait -= waitPerTick;
            }

            for (int k = 0; k < enemiesRear.Count; k++)
            {
                enemiesRear[k].wait -= waitPerTick;
            }

            waitTimeCounter += waitPerTick;
            if (waitTimeCounter >= 4)
            {
                // Play wait sfx
                audioManagerReference.PlayCombatSFX("WaitTimeTick");

                waitTimeCounter -= 4;
            }

            yield return new WaitForSeconds(uiCoroutineTickTime);
        }

        // Increase wait stacks all at once since they're invisible
        playerReference.waitStack += lowestWait;

        for (int i = 0; i < enemiesFront.Count; i++)
        {
            enemiesFront[i].waitStack += lowestWait;
        }

        for (int i = 0; i < enemiesRear.Count; i++)
        {
            enemiesRear[i].waitStack += lowestWait;
        }

        // Lower Revenge Meter by an amount relative to wait passed, unless at 100 already
        if (revengeMeter < 100.0f)
        {
            revengeMeter = Mathf.Clamp(revengeMeter - (lowestWait * 0.2f), 0.0f, 100.0f);
            print("Revenge Meter lowered to " + revengeMeter);
        }

        // Handle enemy turn, otherwise wait for player to do their turn
        if (nextEntity != playerReference)
        {
            print("Enemy turn starting");

            // Play sfx
            audioManagerReference.PlayCombatSFX("EnemyTurnStart");

            // Play turn start animation
            nextEntity.PlayTurnStartAnimation();

            canTurnProceed = false;

            // Start tickdown process
            nextEntity.TickDown();

            yield return new WaitForSeconds(0.1f);

            // Wait for tickdown ui stuff to end
            while (canTurnProceed == false || nextEntity.tickdownFlag == true)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Check if entity can act
            if (nextEntity.CanEntityAct())
            {
                ((EnemyBaseScript)nextEntity).HandleTurn();
            }
            else
            {
                // Check if enemy is dead
                if (nextEntity.health <= 0.0f)
                {
                    ((EnemyBaseScript)nextEntity).HandleDeath();

                    // Check if battle is won
                    if (CheckBattleWon())
                    {
                        DisplayCombatDescription("Gwenaelle prevails!", 200.5f, false);

                        // Play victory sfx
                        audioManagerReference.PlayCombatSFX("VictoryHorn");
                        audioManagerReference.FadeOutBGM(0.4f);

                        // Stop all coroutines on entities

                        playerReference.StopAllCoroutines();

                        for (int i = 0; i < enemiesFront.Count; i++)
                        {
                            enemiesFront[i].StopAllCoroutines();
                        }
                        for (int i = 0; i < enemiesRear.Count; i++)
                        {
                            enemiesRear[i].StopAllCoroutines();
                        }

                        yield return new WaitForSeconds(5.5f);

                        currentPhase = CombatPhase.END;

                        // Swap to narrative
                        GetComponent<GameManagerScript>().HandleCombatVictory();
                    }
                }
                // Continue if not end phase
                if (currentPhase != CombatPhase.END)
                {
                    NotifyTurnComplete();
                }
            }
        }
        else
        {
            print("Player turn starting");

            // Play sfx
            audioManagerReference.PlayCombatSFX("PlayerTurnStart");

            // Play turn start animation
            nextEntity.PlayTurnStartAnimation();

            canTurnProceed = false;

            // Check if lose via equipment durability
            if (CheckLoseViaDurability() == true)
            {
                DisplayCombatDescription("Gwenaelle cannot fight further...", 5.0f, false);

                // Fade out bgm
                audioManagerReference.FadeOutBGM(2.5f);

                // Stop all entity coroutines
                playerReference.StopAllCoroutines();

                for (int i = 0; i < enemiesFront.Count; i++)
                {
                    enemiesFront[i].StopAllCoroutines();
                }
                for (int i = 0; i < enemiesRear.Count; i++)
                {
                    enemiesRear[i].StopAllCoroutines();
                }

                // Wait a while for this to register
                yield return new WaitForSeconds(2.5f);

                SpawnGameOverScreen();

                // Play defeat sfx
                audioManagerReference.PlayCombatSFX("Defeat");
            }
            else
            {
                // Start tickdown process
                nextEntity.TickDown();

                yield return new WaitForSeconds(0.1f);

                // Wait for tickdown ui stuff to end
                while (canTurnProceed == false || nextEntity.tickdownFlag == true)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                // Check to see if player is unable to act
                if (nextEntity.CanEntityAct() == false)
                {
                    NotifyTurnComplete();
                }
                else
                {
                    canPlayerAct = true;

                    // Turn on equipment panel
                    SetEquipmentPanelVisibility(true);

                    // Start player turn idle animation
                    playerReference.GetComponent<Animator>().Play("EntityPlayerTurnIdle");

                    // Reset morningstar stun counter if it was not used last turn
                    if (previousUsedEquipment)
                    {
                        if (previousUsedEquipment.equipmentName != "Morningstar")
                        {
                            GetComponent<AchievementManagerScript>().ResetMorningstarStun();
                        }
                    }
                }
            }
        }

        yield return null;
    }

    // Used to check if all ui effects etc are done and entity turn can continue
    public bool CanTurnProceed()
    {
        return canTurnProceed;
    }

    // Allow other scripts to manipulate the turn execution flag
    public void SetCanTurnProceed(bool newTurnStatus)
    {
        canTurnProceed = newTurnStatus;
        print("Can turn proceed set to " + newTurnStatus);
    }

    // Used to tell the combat manager to reset the judgement meter
    public void NotifyJudgementUsed()
    {
        // Set meter to 0
        judgementMeter = 0.0f;

        // Turn off judgement buttons and reset border sprites
        judgementButtonReference.interactable = false;
        judgementButtonReference.GetComponentInChildren<UIJudgementScript>().ResetJudgementBorderSprites();
        judgementPureButtonReference.GetComponent<UIJudgementPureButtonScript>().SetJudgementButtonColor(false);
        judgementPureButtonReference.GetComponent<UIJudgementPureButtonScript>().ResetJudgementBorderSprites();

        // Reset equipment button icons back to normal
        for (int i = 0; i < equipmentButtonReferences.Count; i++)
        {
            equipmentButtonReferences[i].GetComponent<EquipmentBaseScript>().SetEquipmentIcon();
        }

        // Turn the judgement flag off
        judgementFlag = false;

        // Also stop playing particles
        playerReference.GetComponent<ParticleSystem>().Stop();
    }

    // Used to change equipment judgement state and icons
    public void SetJudgementState(bool newJudgementState)
    {
        judgementFlag = newJudgementState;

        // Set icons to highlights if judgement is on
        if (judgementFlag == true)
        {
            for (int i = 0; i < equipmentButtonReferences.Count; i++)
            {
                equipmentButtonReferences[i].GetComponent<EquipmentBaseScript>().SetEquipmentJudgementIcon();
            }

            // Also start playing particles if combat and jugement really full
            if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eCombat && judgementMeter == 100.0f)
            {
                playerReference.GetComponent<ParticleSystem>().Play();
            }
        }
        // Otherwise set them to normal
        else
        {
            for (int i = 0; i < equipmentButtonReferences.Count; i++)
            {
                equipmentButtonReferences[i].GetComponent<EquipmentBaseScript>().SetEquipmentIcon();
            }

            // Also stop playing particles if combat
            if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eCombat && judgementMeter == 100.0f)
            {
                playerReference.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    // Used to tell the combat manager to reset the revenge meter
    public void NotifyRevengeUsed()
    {
        revengeMeter = 0.0f;
    }

    // Used to determine if an action hits or not
    static public bool TestAccuracy(float accuracy)
    {
        return (Random.Range(0.0f, 100.0f) <= accuracy);
    }

    // Change the combat description to match the situation, part 1
    public void DisplayCombatDescription(string description, float length = 1.5f, bool removeAfterDuration = true)
    {
        // Prevent turn from proceeding until this function is complete
        canTurnProceed = false;

        combatDescriptionShowing = true;

        // Start UI coroutine
        StartCoroutine(HandleCombatDescriptionUI(description, length, removeAfterDuration));
    }

    IEnumerator HandleCombatDescriptionUI(string description, float length, bool removeAfterDuration)
    {
        // Display the combat description
        combatDescriptionUIReference.DisplayText(description);

        // Figure out the real duration (speed ranges from 1-8)
        length *= 4 / combatDescriptionSpeed;

        // Wait for the duration
        yield return new WaitForSeconds(length);

        if (removeAfterDuration == true)
        {
            combatDescriptionUIReference.StopDisplay();
            combatDescriptionShowing = false;
        }

        // Resume entity turn
        canTurnProceed = true;


        yield return null;
    }

    public void RemoveCombatDescription()
    {
        combatDescriptionUIReference.StopDisplay();
        combatDescriptionShowing = false;

        // Then remove the damage ui
        damageUIReference.StopDisplay();
    }

    public bool IsCombatDescriptionShowing()
    {
        return combatDescriptionShowing;
    }

    // Calculate final damage then deal it, part 1
    public void InflictDamagePlayer(float damage)
    {
        // Prevent turn from proceeding until this damage function is complete
        canTurnProceed = false;

        // Figure out def mods
        if (playerReference.HasModifier(StatType.DEF))
        {
            damage *= (100.0f - playerReference.GetModifier(StatType.DEF).modifierValue) / 100.0f;
        }

        // Make sure to round the damage
        damage = Mathf.Round(damage);

        // Check if the player is blocking
        if (playerReference.HasAugment(AugmentType.BLOCK))
        {
            if (revengeMeter < 100.0f)
            {
                // Prevent the damage and make the player's turn next
                StartCoroutine(HandlePlayerBlockUI(damage));
            }
            else
            {
                // Otherwise do the damage
                StartCoroutine(HandlePlayerDamageUI(damage));

                print("Damage dealt to player: " + damage);
                print("Player health remaining: " + playerReference.health);

                // Unlock steam shield achievement
                GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_SHIELD");
            }
            
        }
        // Otherwise do the damage
        else
        {
            // Start damage coroutine
            StartCoroutine(HandlePlayerDamageUI(damage));

            print("Damage dealt to player: " + damage);
            print("Player health remaining: " + playerReference.health);
        }

    }

    // Player damage part 2, decreases health ui, then figures out if player is dead
    // Also does judgement things
    IEnumerator HandlePlayerDamageUI(float damage)
    {
        // First display the total damage taken
        damageUIReference.DisplayDamage(damage);

        // Spawn a damage number
        GameObject damageNumber = Instantiate(damageNumberPrefab, GameObject.Find("Canvas").transform);
        damageNumber.GetComponent<RectTransform>().anchorMax = playerReference.GetComponent<RectTransform>().anchorMax;
        damageNumber.GetComponent<RectTransform>().anchorMin = playerReference.GetComponent<RectTransform>().anchorMin;
        damageNumber.GetComponent<RectTransform>().anchoredPosition = playerReference.GetComponent<RectTransform>().anchoredPosition;
        damageNumber.GetComponent<UIDamageNumberScript>().Initialise(damage, true);

        // Play damage sfx
        if (revengeMeter >= 100.0f)
        {
            audioManagerReference.PlayEntitySFX("GwenDamageRevenge");
        }
        else
        {
            audioManagerReference.PlayEntitySFX("GwenDamageStandard");
        }

        // Figure out how to calculate judgement conversion
        if (judgementMeter < 100.0f)
        {
            float judgementGain = damage * 1.25f * playerReference.GetJudgementHealthScaling() + 10.0f;
            // Check for battlefield placebo effect
            if (playerReference.HasModifier(StatType.DEF))
            {
                if (playerReference.GetModifier(StatType.DEF).modifierValue == 60.0f)
                {
                    // Buff the gain by 300%
                    judgementGain *= 3.0f;
                }
            }
            judgementMeter += judgementGain;
            print("Judgement meter is now at " + judgementMeter);
        }
        if (judgementMeter >= 100.0f)
        {
            print("Judgement meter is full");
            judgementMeter = 100.0f;
            judgementPureButtonReference.GetComponent<UIJudgementPureButtonScript>().SetJudgementButtonColor(true);
            judgementPureButtonReference.image.enabled = true;
        }

        // Then slowly reduce health for a certain amount of ticks and turn health animation on
        playerReference.SetHealthAnimationFlag(true);
        playerReference.PlayDamageAnimationPlayer();

        // Check if damage is enough to kill
        if (damage >= playerReference.health)
        {
            // Set damage to the player's health
            damage = playerReference.health;
        }

        float healthPerTick = damage / (uiDisplayTime / uiCoroutineTickTime);

        for (int i = 0; i < Mathf.Ceil(uiDisplayTime / uiCoroutineTickTime); i++)
        {
            playerReference.health -= healthPerTick;

            yield return new WaitForSeconds(uiCoroutineTickTime);
        }

        //Turn off the health animation
        playerReference.SetHealthAnimationFlag(false);

        // Then remove the damage ui
        //damageUIReference.StopDisplay();

        // Wait a while for this to register
        yield return new WaitForSeconds(1.0f);

        // Check if the player is dead
        if (playerReference.health <= 0.0f)
        {
            // Check if player is unyielding
            if (playerReference.HasAugment(AugmentType.UNYIELDING))
            {
                // Restore player back to 1 hp
                playerReference.health = 1.0f;

                // Handle unyielding charges
                ((AugUnyieldingScript)playerReference.GetAugment(AugmentType.UNYIELDING)).ProcessUndeath();

                // Change Description
                DisplayCombatDescription("Gwenaelle resists death!");

                // Check for rosary achievement
                if (damage >= 50.0f)
                {
                    GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_ROSARY");
                }

                yield return new WaitForSeconds(1.5f);

                // Allow turn to continue
                canTurnProceed = true;
            }
            // Otherwise they have lost
            else
            {
                RemoveCombatDescription();

                DisplayCombatDescription("Gwenaelle falls...", 5.0f, false);

                // Fade out bgm
                audioManagerReference.FadeOutBGM(2.5f);

                // Stop all entity coroutines
                playerReference.StopAllCoroutines();

                for (int i = 0; i < enemiesFront.Count; i++)
                {
                    enemiesFront[i].StopAllCoroutines();
                }
                for (int i = 0; i < enemiesRear.Count; i++)
                {
                    enemiesRear[i].StopAllCoroutines();
                }

                // Wait a while for this to register
                yield return new WaitForSeconds(2.5f);

                SpawnGameOverScreen();

                // Play defeat sfx
                audioManagerReference.PlayCombatSFX("Defeat");
            }
        }
        else
        {
            // Allow entity turn to proceed
            canTurnProceed = true;
        }

        yield return null;
    }

    public void RestoreHealthPlayer(float healAmount)
    {
        // start the damn coroutine
        StartCoroutine(HandlePlayerHealingUI(healAmount));
    }

    IEnumerator HandlePlayerHealingUI(float healAmount)
    {
        canTurnProceed = false;

        // Figure out if this healing needs to be capped
        healAmount = Mathf.Clamp(healAmount, 0, 100 - playerReference.health);

        // Round this number
        healAmount = Mathf.Round(healAmount);

        // First display the total health restored
        damageUIReference.DisplayHealing(healAmount);

        // Play sfx
        audioManagerReference.PlayCombatSFX("Healing");

        float endHealth = playerReference.health + healAmount;

        // Then slowly increase health for a certain amount of ticks
        float healthPerTick = healAmount / (uiDisplayTime / uiCoroutineTickTime);

        for (int i = 0; i < Mathf.Ceil(uiDisplayTime / uiCoroutineTickTime); i++)
        {
            playerReference.health += healthPerTick;

            yield return new WaitForSeconds(uiCoroutineTickTime);
        }

        // Manually set the health as tick function is inaccurate
        playerReference.health = endHealth;

        // Wait a while for this to register
        yield return new WaitForSeconds(1.0f);

        // Then remove the damage ui
        damageUIReference.StopDisplay();

        // Allow entity turn to proceed
        canTurnProceed = true;

        yield return null;
    }

    IEnumerator HandlePlayerBlockUI(float damagePrevented)
    {
        // Change description
        DisplayCombatDescription("Gwenaelle blocks the attack and readies a riposte!", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayCombatSFX("ShieldCounter");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove block
        playerReference.RemoveAugment(AugmentType.BLOCK);

        // Set WT to 1
        playerReference.wait = 1;

        // Also increase judgement meter by the damage she would have taken
        if (judgementMeter < 100.0f)
        {
            float judgementGain = damagePrevented * 1.15f * playerReference.GetJudgementHealthScaling() + 5.0f;
            // Check for battlefield placebo effect
            if (playerReference.HasModifier(StatType.DEF))
            {
                if (playerReference.GetModifier(StatType.DEF).modifierValue == 60.0f)
                {
                    // Buff the gain by 300%
                    judgementGain *= 3.0f;
                }
            }
            judgementMeter += judgementGain;
            print("Judgement meter is now at " + judgementMeter);
        }
        if (judgementMeter >= 100.0f)
        {
            print("Judgement meter is full");
            judgementMeter = 100.0f;
            judgementPureButtonReference.GetComponent<UIJudgementPureButtonScript>().SetJudgementButtonColor(true);
            judgementPureButtonReference.image.enabled = true;
        }

        // Wait until turn can proceed
        while (CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        RemoveCombatDescription();

        yield return null;
    }

    public bool ApplyModifierToPlayer(StatType modifier, float amount)
    {
        // Check if Unparallelled precision is active
        if (playerReference.HasModifier(StatType.ACC))
        {
            if (playerReference.GetModifier(StatType.ACC).modifierValue == 50.0f && amount <= 0.0f)
            {
                // Modifier cannot be applied

                // Increase achievement count
                GetComponent<AchievementManagerScript>().CountDebuffAvoided();
                return false;
            }
            else
            {
                // Apply the modifier
                playerReference.AddModifier(modifier, amount);
                return true;
            }
        }
        else
        {
            // Apply the modifier
            playerReference.AddModifier(modifier, amount);
            return true;
        }
    }

    public void ApplyAugmentToPlayer(AugmentType augment)
    {
        // Apply the augment
        playerReference.AddAugment(augment);
    }

    public void RemoveAugmentFromPlayer(AugmentType augment)
    {
        // Let player remove the augment
        playerReference.RemoveAugment(augment);
    }

    // Deal damage to a set of enemies, part 1
    public void InflictDamageEnemy(TargetType target, float damageLower, float damageHigher, EntityBaseScript damageDealer)
    {
        // Prevent turn from proceeding until this damage is complete
        canTurnProceed = false;

        // Check if enemy has dysphoria
        bool dysphoriaFlag = false;
        List<EnemyBaseScript> enemiesDamaged = GetTargetedEnemies(target);
        for (int i = 0; i < enemiesDamaged.Count; i++)
        {
            if (enemiesDamaged[i].HasAugment(AugmentType.DYSPHORIA))
            {
                dysphoriaFlag = true;
            }
        }

        if (dysphoriaFlag == true)
        {
            // Prevent the damage and eat the item
            StartCoroutine(HandleEnemyDysphoriaUI());
        }

        // Otherwise do more checks
        else
        {
            // Check if the enemy is countering
            if (enemiesDamaged[0].HasAugment(AugmentType.COUNTER) && judgementFlag == false)
            {
                // Prevent the damage and make the enemy's turn next
                float preventedDamage = Mathf.Round(Random.Range(damageLower, damageHigher));
                StartCoroutine(HandleEnemyBlockUI(preventedDamage));
            }
            // Otherwise actually do damage
            else
            {
                // Start damage coroutine
                StartCoroutine(HandleEnemyDamageUI(target, damageLower, damageHigher, damageDealer));
            }
        }
    }

    // Enemy damage part 2, decreases health ui, then figures out if enemy is dead
    // Also does revenge things
    IEnumerator HandleEnemyDamageUI(TargetType target, float damageLower, float damageHigher, EntityBaseScript damageDealer)
    {
        // Get the set of enemies to damage
        List<EnemyBaseScript> enemiesDamaged = GetTargetedEnemies(target);
        previousHitEnemyCount = enemiesDamaged.Count;
        float revengeIncrease = 0;
        float actualDamage = 0;

        // Loop through until all enemies are damaged
        for (int i = 0; i < enemiesDamaged.Count; i++)
        {
            // Calculate the damage
            actualDamage = Mathf.Round(Random.Range(damageLower, damageHigher));
            if (damageDealer.HasModifier(StatType.DMG))
            {
                float damageDifference = actualDamage;
                actualDamage *= 1 + damageDealer.GetModifier(StatType.DMG).modifierValue / 100.0f;

                // Check for flag staff damage bonus achievement
                if (damageDealer.GetModifier(StatType.DMG).modifierValue == 25.0f || damageDealer.GetModifier(StatType.DMG).modifierValue == 100.0f)
                {
                    FindObjectOfType<AchievementManagerScript>().CountFlagStaffDamageBonus(Mathf.RoundToInt(actualDamage - damageDifference));
                }
            }
            // Calculate damage after def mods
            if (enemiesDamaged[i].HasModifier(StatType.DEF))
            {
                actualDamage = actualDamage * ((100.0f - enemiesDamaged[i].GetModifier(StatType.DEF).modifierValue) / 100.0f);
            }
            // Make sure to round the damage
            actualDamage = Mathf.Round(actualDamage);

            // Display the total damage taken
            damageUIReference.DisplayDamage(actualDamage);

            // Spawn a damage number
            GameObject damageNumber = Instantiate(damageNumberPrefab, GameObject.Find("Canvas").transform);
            damageNumber.GetComponent<RectTransform>().anchorMax = enemiesDamaged[i].GetComponent<RectTransform>().anchorMax;
            damageNumber.GetComponent<RectTransform>().anchorMin = enemiesDamaged[i].GetComponent<RectTransform>().anchorMin;
            damageNumber.GetComponent<RectTransform>().anchoredPosition = enemiesDamaged[i].GetComponent<RectTransform>().anchoredPosition;
            damageNumber.GetComponent<UIDamageNumberScript>().Initialise(actualDamage, false);

            // Play damage sfx
            // But check if judgement or not
            if (judgementFlag)
            {
                audioManagerReference.PlayWeaponSFX("WeaponImpactJudgement");
            }
            else
            {
                audioManagerReference.PlayWeaponSFX("WeaponImpactStandard");
            }

            // Figure out how to calculate revenge conversion
            if (revengeMeter < 100.0f)
            {
                // Store it to increase all at once
                revengeIncrease += actualDamage * (1 + actualDamage / enemiesDamaged[i].health);

                // Check if final bosses
                if (FindObjectOfType<GameManagerScript>().currentAct == 4)
                {
                    // Increase revenge gain
                    revengeIncrease *= finalBossRevengeMultiplier;
                }

                if (revengeMeter > 100.0f)
                {
                    revengeMeter = 100.0f;
                }
            }

            // Then slowly reduce health for a certain amount of ticks and play animation
            enemiesDamaged[i].SetHealthAnimationFlag(true);
            enemiesDamaged[i].PlayDamageAnimationEnemy();

            // Check pole axe damage for achievement
            if (previousUsedEquipment.equipmentName == "Pole Axe")
            {
                GetComponent<AchievementManagerScript>().CountPoleAxeDamage((int)actualDamage);
            }

            // Check if damage will kill
            if (actualDamage >= enemiesDamaged[i].health)
            {
                actualDamage = enemiesDamaged[i].health;
            }

            previousPlayerDamageDealt = actualDamage;

            float endHealth = enemiesDamaged[i].health - actualDamage;
            float healthPerTick = actualDamage / (uiDisplayTime / uiCoroutineTickTime);

            for (int j = 0; j < Mathf.Ceil(uiDisplayTime / uiCoroutineTickTime); j++)
            {
                enemiesDamaged[i].health -= healthPerTick;

                yield return new WaitForSeconds(uiCoroutineTickTime);
            }

            // Manually set the health as the tick function is inaccurate
            enemiesDamaged[i].health = endHealth;

            // Stop the damage animation
            enemiesDamaged[i].SetHealthAnimationFlag(false);

            // Wait a while for this to register
            yield return new WaitForSeconds(1.0f);

            // Then remove the damage ui
            //damageUIReference.StopDisplay();
        }

        // Wait a while after damage is inflicted
        yield return new WaitForSeconds(0.5f);

        int enemyDeathCounter = 0;

        // Check if any of the enemies are dead
        for (int i = 0; i < enemiesDamaged.Count; i++)
        {
            if (enemiesDamaged[i].health <= 0.0f)
            {
                // Check if they have unyielding
                if (enemiesDamaged[i].HasAugment(AugmentType.UNYIELDING))
                {
                    // Restore enemy back to 1 hp
                    enemiesDamaged[i].health = 1.0f;

                    // Handle unyielding charges
                    ((AugUnyieldingScript)enemiesDamaged[i].GetAugment(AugmentType.UNYIELDING)).ProcessUndeath();
                }
                else
                {
                    // Mark them for death
                    enemiesDamaged[i].HandleDeath();

                    // Claymore achievement stuff
                    enemyDeathCounter += 1;
                    if (enemyDeathCounter == 3 && previousUsedEquipment.equipmentName == "Claymore")
                    {
                        GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_CLAYMORE");
                    }

                    damageUIReference.StopDisplay();

                    // Check if this is final boss for unique death quote
                    if (FindObjectOfType<GameManagerScript>().currentAct == 4)
                    {
                        DisplayCombatDescription(enemiesDamaged[i].entityName + " is defeated.", 3.0f);
                    }
                    else
                    {
                        DisplayCombatDescription(enemiesDamaged[i].entityName + " vanishes into mist.", 3.0f);

                        // Do death anim
                        enemiesDamaged[i].GetComponent<Coffee.UIExtensions.UIDissolve>().Play();

                        // Spawn enemy death particle on enemy
                        GameObject deathParticle = Instantiate(enemyDeathParticlePrefab, GameObject.Find("Canvas").transform);
                        deathParticle.GetComponent<RectTransform>().anchoredPosition = enemiesDamaged[i].GetComponent<RectTransform>().anchoredPosition;

                        // Play death sfx
                        audioManagerReference.PlayEntitySFX("EnemyDeath");
                    }

                    yield return new WaitForSeconds(3.0f);

                    // Increase revenge meter
                    revengeIncrease += 25.0f;
                }
            }
        }

        // Check if battle is won
        if (CheckBattleWon())
        {
            DisplayCombatDescription("Gwenaelle prevails!", 200.5f, false);

            // Play victory sfx
            audioManagerReference.PlayCombatSFX("VictoryHorn");
            audioManagerReference.FadeOutBGM(0.4f);

            // Stop all coroutines on entities

            playerReference.StopAllCoroutines();

            for (int i = 0; i < enemiesFront.Count; i++)
            {
                enemiesFront[i].StopAllCoroutines();
            }
            for (int i = 0; i < enemiesRear.Count; i++)
            {
                enemiesRear[i].StopAllCoroutines();
            }

            // Make sure to reduce durability of item after victory
            previousUsedEquipment.durability -= 1;

            // Check for everlustre achievement
            if (previousUsedEquipment.equipmentName == "Everlustre")
            {
                GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_EVERLUSTRE");
            }

            yield return new WaitForSeconds(5.5f);

            currentPhase = CombatPhase.END;

            // Long sword achievement tracking
            if (playerManagerReference.CombatInventoryContains("Long Sword") && (GetComponent<GameManagerScript>().currentChapter == 4 || GetComponent<GameManagerScript>().currentAct == 4))
            {
                GetComponent<AchievementManagerScript>().CountLongSwordVictory();
            }

            // Swap to narrative
            GetComponent<GameManagerScript>().HandleCombatVictory();
        }
        else
        {
            // Apply revenge meter if damage not from everlustre
            if (previousUsedEquipment.equipmentName != "Everlustre")
            {
                revengeMeter += revengeIncrease / GetRevengeGroupMultiplier(previousHitEnemyCount);
                if (revengeMeter > 100.0f)
                {
                    revengeMeter = 100.0f;
                }
            }

            // Allow entity turn to continue
            canTurnProceed = true;
        }

        yield return null;
    }

    IEnumerator HandleEnemyDysphoriaWarningUI()
    {
        // Change description
        DisplayCombatDescription("Umbral Mass shudders...", 2.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Set the flag
        dysphoriaFlag = true;

        yield return null;
    }

    IEnumerator HandleEnemyDysphoriaUI()
    {
        // Change description
        DisplayCombatDescription("Umbral Mass screams and devours the " + previousUsedEquipment.equipmentName + "!", 2.5f, false);

        // Play sfx
        audioManagerReference.PlayCombatSFX("DysphoriaRetaliate");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        previousUsedEquipment.durability = 0;
        previousUsedEquipment.HandleEquipmentBreaking();

        RemoveCombatDescription();

        yield return null;
    }

    IEnumerator HandleEnemyBlockUI(float damagePrevented)
    {
        // Change description
        DisplayCombatDescription("Vermilion Knight counters the attack and readies a riposte!", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove block
        enemiesFront[0].RemoveAugment(AugmentType.COUNTER);

        // Set WT to 1
        enemiesFront[0].wait = 1;

        // Also increase revenge meter by the damage she would have taken
        if (revengeMeter < 100.0f)
        {
            revengeMeter += damagePrevented * (1 + damagePrevented / enemiesFront[0].health);
        }
        if (revengeMeter >= 100.0f)
        {
            revengeMeter = 100.0f;
        }

        // Wait until turn can proceed
        while (CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        RemoveCombatDescription();

        yield return null;
    }

    // Deal damage to specified enemy, part 1
    public void InflictDamageEnemySpecified(EnemyBaseScript enemy, float damage)
    {
        // Prevent turn from proceeding until this damage is complete
        canTurnProceed = false;

        // Start damage coroutine
        StartCoroutine(HandleEnemyDamageSpecifiedUI(enemy, damage));
    }

    // Specified enemy damage part 2, decreases health ui, then figures out if enemy is dead
    // Also does revenge things
    IEnumerator HandleEnemyDamageSpecifiedUI(EnemyBaseScript enemy, float damage)
    {
        // Get the set of enemies to damage
        previousHitEnemyCount = 1;
        float revengeIncrease = 0;
        float actualDamage = damage;

        actualDamage = damage;
        // Calculate damage after def mods
        if (enemy.HasModifier(StatType.DEF))
        {
            actualDamage = damage * (100.0f - enemy.GetModifier(StatType.DEF).modifierValue) / 100.0f;
        }
        // Make sure to round the damage
        actualDamage = Mathf.Round(actualDamage);

        // Display the total damage taken
        damageUIReference.DisplayDamage(actualDamage);

        // Play damage sfx
        audioManagerReference.PlayWeaponSFX("WeaponImpactStandard");


        // Figure out how to calculate revenge conversion
        if (revengeMeter < 100.0f)
        {
            // Store it to increase all at once
            revengeIncrease += actualDamage * (1 + actualDamage / enemy.health);
            if (revengeMeter > 100.0f)
            {
                revengeMeter = 100.0f;
            }
        }

        // Then slowly reduce health for a certain amount of ticks and play animation
        enemy.SetHealthAnimationFlag(true);
        enemy.PlayDamageAnimationEnemy();

        // Check if damage will kill
        if (actualDamage >= enemy.health)
        {
            actualDamage = enemy.health;
        }

        previousPlayerDamageDealt = actualDamage;

        float endHealth = enemy.health - actualDamage;
        float healthPerTick = actualDamage / (uiDisplayTime / uiCoroutineTickTime);

        for (int j = 0; j < Mathf.Ceil(uiDisplayTime / uiCoroutineTickTime); j++)
        {
            enemy.health -= healthPerTick;

            yield return new WaitForSeconds(uiCoroutineTickTime);
        }

        // Manually set the health as the tick function is inaccurate
        enemy.health = endHealth;

        // Stop the damage animation
        enemy.SetHealthAnimationFlag(false);

        // Then remove the damage ui
        damageUIReference.StopDisplay();

        // Wait a while after damage is inflicted
        yield return new WaitForSeconds(1);

        // Check if the enemy is dead
        if (enemy.health <= 0.0f)
        {
            // Mark them for death
            enemy.HandleDeath();

            DisplayCombatDescription(enemy.entityName + " vanishes into mist.");

            yield return new WaitForSeconds(1.5f);

            // Increase revenge meter
            revengeIncrease += 25.0f;
        }

        // Check if battle is won
        if (CheckBattleWon())
        {
            DisplayCombatDescription("Gwenaelle prevails!", 200.5f);

            yield return new WaitForSeconds(5.5f);

            currentPhase = CombatPhase.END;

            // Swap to narrative
            GetComponent<GameManagerScript>().HandleCombatVictory();
        }
        else
        {
            // Apply revenge meter
            revengeMeter += revengeIncrease;
            if (revengeMeter > 100.0f)
            {
                revengeMeter = 100.0f;
            }

            // Allow entity turn to continue
            canTurnProceed = true;
        }

        yield return null;
    }

    // Uses the enemy index system, 1-2 is front, 3-4 is rear, 5 is all
    // Potentially dangerous function
    public void RestoreHealthEnemy(int enemyIndex, float healAmount)
    {
        StartCoroutine(HandleEnemyHealingUI(enemyIndex, healAmount));
    }

    IEnumerator HandleEnemyHealingUI(int enemyIndex, float healAmount)
    {
        canTurnProceed = false;

        // Figure out which enemy to heal
        EnemyBaseScript targetEnemy = GetEnemyFromIndex(enemyIndex);

        // Figure out if this healing needs to be capped
        healAmount = Mathf.Clamp(healAmount, 0, targetEnemy.maxHealth - targetEnemy.health);

        // Round this number
        healAmount = Mathf.Round(healAmount);

        // First display the total health restored
        damageUIReference.DisplayHealing(healAmount);

        // Play sfx
        audioManagerReference.PlayCombatSFX("Healing");

        float endHealth = targetEnemy.health + healAmount;

        // Then slowly increase health for a certain amount of ticks
        float healthPerTick = healAmount / (uiDisplayTime / uiCoroutineTickTime);

        for (int i = 0; i < Mathf.Ceil(uiDisplayTime / uiCoroutineTickTime); i++)
        {
            targetEnemy.health += healthPerTick;

            yield return new WaitForSeconds(uiCoroutineTickTime);
        }

        // Manually set the health as tick function is inaccurate
        targetEnemy.health = endHealth;

        // Wait a while for this to register
        yield return new WaitForSeconds(1.0f);

        // Then remove the damage ui
        damageUIReference.StopDisplay();

        // Allow entity turn to proceed
        canTurnProceed = true;

        yield return null;
    }

    public List<EnemyBaseScript> GetTargetedEnemies(TargetType targets)
    {
        List<EnemyBaseScript> targetEnemies = new List<EnemyBaseScript>();

        // Do individual cases for each target type

        // ALL
        if (targets == TargetType.All)
        {
            // add all enemies into the list
            for (int i = 0; i < enemiesFront.Count; i++)
            {
                targetEnemies.Add(enemiesFront[i]);
            }
            for (int i = 0; i < enemiesRear.Count; i++)
            {
                targetEnemies.Add(enemiesRear[i]);
            }

            return targetEnemies;
        }

        // Front Line
        else if (targets == TargetType.FrontLine)
        {
            // Make sure there are enemies in front
            if (enemiesFront.Count > 0)
            {
                // add all front enemies into the list
                for (int i = 0; i < enemiesFront.Count; i++)
                {
                    targetEnemies.Add(enemiesFront[i]);
                }
            }
            else
            {
                // Otherwise add all rear enemies into the list
                for (int i = 0; i < enemiesRear.Count; i++)
                {
                    targetEnemies.Add(enemiesRear[i]);
                }
            }

            return targetEnemies;
        }

        // Rear line
        else if (targets == TargetType.RearLine)
        {
            // Make sure there are enemies in rear
            if (enemiesRear.Count > 0)
            {
                // add all rear enemies into the list
                for (int i = 0; i < enemiesRear.Count; i++)
                {
                    targetEnemies.Add(enemiesRear[i]);
                }
            }
            else
            {
                // Otherwise add all front enemies into the list
                for (int i = 0; i < enemiesFront.Count; i++)
                {
                    targetEnemies.Add(enemiesFront[i]);
                }
            }

            return targetEnemies;
        }

        // Pierce
        else if (targets == TargetType.Pierce)
        {
            // Check there is an enemy in the front and rear
            if (enemiesFront.Count > 0)
            {
                targetEnemies.Add(enemiesFront[0]);
            }
            if (enemiesRear.Count > 0)
            {
                targetEnemies.Add(enemiesRear[0]);
            }

            return targetEnemies;
        }

        // Single Front
        else if (targets == TargetType.SingleFront)
        {
            // Check if there is an enemy in the front
            if (enemiesFront.Count > 0)
            {
                targetEnemies.Add(enemiesFront[0]);
            }
            // Otherwise add the rear enemy instead
            else
            {
                targetEnemies.Add(enemiesRear[0]);
            }

            return targetEnemies;
        }

        // Single Front
        else if (targets == TargetType.SingleRear)
        {
            // Check if there is an enemy in the rear
            if (enemiesRear.Count > 0)
            {
                targetEnemies.Add(enemiesRear[0]);
            }
            // Otherwise add the front enemy instead
            else
            {
                targetEnemies.Add(enemiesFront[0]);
            }

            return targetEnemies;
        }

        // Cannot convert properly
        print("Cannot convert target properly, target type does not match listed targets.");

        return targetEnemies;
    }

    public void SetEnemyTargetedAnimations(bool shouldPlay, TargetType targets = TargetType.All)
    {
        // Used to set targeted animations when mouse hovers an equipment
        if (shouldPlay == true)
        {
            // Find the list of enemies to set animation
            List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(targets);

            // Go through them all and start the animation
            for (int i = 0; i < targetedEnemies.Count; i++)
            {
                targetedEnemies[i].SetTargetedAnimation(true);
            }
        }
        else
        {
            // Find the list of enemies to set animation
            List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(targets);

            // Go through them all and stop the animation
            for (int i = 0; i < targetedEnemies.Count; i++)
            {
                targetedEnemies[i].SetTargetedAnimation(false);
            }
        }
    }

    public void SetWaitPositionIndicators(bool shouldReveal, float equipmentWaitCost)
    {
        //List<UIWaitPositionIndicatorScript> waitPositionIndicatorReferences = new List<UIWaitPositionIndicatorScript>(FindObjectsOfType<UIWaitPositionIndicatorScript>());
        // Check if should reveal
        if (shouldReveal)
        {
            // Gather and sort all entities
            List<EntityBaseScript> allEntities = new List<EntityBaseScript>();
            for (int i = 0; i < enemiesFront.Count; i++)
            {
                allEntities.Add(enemiesFront[i]);
            }
            for (int i = 0; i < enemiesRear.Count; i++)
            {
                allEntities.Add(enemiesRear[i]);
            }

            // Add player entity with increased wait time
            EntityBaseScript playerEntity = playerReference;

            // Check if player has speed modifier
            if (playerReference.HasModifier(StatType.SPD))
            {
                playerEntity.wait += equipmentWaitCost * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f);
            }
            else
            {
                playerEntity.wait += equipmentWaitCost;
            }
            allEntities.Add(playerEntity);

            // Finally sort the entities
            allEntities = SelectionSortEntityWaitTimes(allEntities);

            // Reset player entity wait time
            playerReference.wait = 0.0f;

            // And now reveal wait positions for each
            for (int i = 0; i < allEntities.Count; i++)
            {
                allEntities[i].GetComponentInChildren<UIWaitPositionIndicatorScript>().NotifyReveal(i + 1);
            }
        }
        // Otherwise just turn all the wait position indicators off
        else
        {
            List<UIWaitPositionIndicatorScript> waitPositionIndicatorReferences = new List<UIWaitPositionIndicatorScript>(FindObjectsOfType<UIWaitPositionIndicatorScript>());

            foreach(UIWaitPositionIndicatorScript WPI in waitPositionIndicatorReferences)
            {
                WPI.StopReveal();
            }
        }
    }

    private List<EntityBaseScript> SelectionSortEntityWaitTimes(List<EntityBaseScript> entitiesToSort)
    {
        float sortingWait;
        int sortingIndex;
        EntityBaseScript swapEntityReference;

        // Go through and do selection sort
        for (int i = 0; i < entitiesToSort.Count; i++)
        {
            sortingWait = entitiesToSort[i].wait;
            sortingIndex = i;
            for (int j = i + 1; j < entitiesToSort.Count; j++)
            {
                // Check if this wait value is lower than current value
                if (entitiesToSort[j].wait < sortingWait)
                {
                    sortingWait = entitiesToSort[j].wait;
                    sortingIndex = j;
                }
            }

            // Lowest value should be found, swap with index i if necessary
            if (sortingIndex != i)
            {
                swapEntityReference = entitiesToSort[sortingIndex];
                entitiesToSort[sortingIndex] = entitiesToSort[i];
                entitiesToSort[i] = swapEntityReference;
            }
            
        }

        return entitiesToSort;
    }

    public EnemyBaseScript GetEnemyFromIndex(int enemyIndex)
    {
        // Is index for front line
        if (enemyIndex < 3)
        {
            return enemiesFront[enemyIndex - 1];
        }
        else
        {
            return enemiesRear[enemyIndex - 3];
        }
    }

    public void ApplyModifierToEnemies(TargetType target, StatType modifier, float amount, float activationChance)
    {
        // Figure out the targets then apply modifier to each
        List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(target);

        for (int i = 0; i < targetedEnemies.Count; i++)
        {
            // Check if it actually applies
            if (TestAccuracy(activationChance) && targetedEnemies[i].deathFlag == false)
            {
                targetedEnemies[i].AddModifier(modifier, amount);
            }
        }
    }

    public int GetModifierCountOfEnemies(TargetType target)
    {
        // Figure out the targets then count the modifiers for them
        List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(target);

        int modifierAmount = 0;

        for (int i = 0; i < targetedEnemies.Count; i++)
        {
            modifierAmount += targetedEnemies[i].GetModifierCount();
        }

        return modifierAmount;
    }

    public void RemovePositiveModifiersFromEnemies()
    {
        List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(TargetType.All);

        for (int i = 0; i < targetedEnemies.Count; i++)
        {
            targetedEnemies[i].RemoveAllPositiveModifiers();
        }
    }

    public void ApplyAugmentToEnemies(TargetType target, AugmentType augment, float activationChance)
    {
        if (augment == AugmentType.BLIND)
        {
            StartCoroutine(HandleApplyEnemyBlindUI(target, activationChance));
        }
        else
        {
            // Figure out the targets then apply augment to each
            List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(target);

            for (int i = 0; i < targetedEnemies.Count; i++)
            {
                // Check if it actually applies
                if (TestAccuracy(activationChance) && targetedEnemies[i].deathFlag == false)
                {
                    targetedEnemies[i].AddAugment(augment);
                }
            }
        }

    }

    // Super dumb specific fix for spear blinding
    IEnumerator HandleApplyEnemyBlindUI(TargetType target, float activationChance)
    {
        // Figure out the targets then apply augment to each
        List<EnemyBaseScript> targetedEnemies = GetTargetedEnemies(target);

        for (int i = 0; i < targetedEnemies.Count; i++)
        {
            // Check if it actually applies
            if (TestAccuracy(activationChance) && targetedEnemies[i].deathFlag == false)
            {
                targetedEnemies[i].AddAugment(AugmentType.BLIND);

                // Display a description
                DisplayCombatDescription("The " + targetedEnemies[i].entityName + " is blinded!", 1.5f);
                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        yield return null;
    }

    private float GetRevengeGroupMultiplier(int targets)
    {
        return 1 + 0.25f * (targets - 1);
    }

    public string GetTargetName(TargetType target)
    {
        // Might be broke
        return GetTargetedEnemies(target)[0].entityName;
    }

    public bool CheckTargetAlive(TargetType target)
    {
        // Get target then check if any of them are alive
        List<EnemyBaseScript> enemyTargets = GetTargetedEnemies(target);

        for (int i = 0; i < enemyTargets.Count; i++)
        {
            if (enemyTargets[i].deathFlag == false)
            {
                // If not dead, return true
                return true;
            }
        }
        // All dead, return false
        return false;
    }

    private bool CheckLoseViaDurability()
    {
        // Look at all equipment
        for (int i = 0; i < equipmentButtonReferences.Count; i++)
        {
            // If any has durability all is fine
            if (equipmentButtonReferences[i].GetComponent<EquipmentBaseScript>().durability > 0)
            {
                return false;
            }
        }
        // Otherwise gg
        return true;

    }

    // Used to tell the combat manager that player should no longer be able to act
    public void NotifyPlayerActionTaken()
    {
        canPlayerAct = false;

        // Turn off the panel
        SetEquipmentPanelVisibility(false);

        // Turn off target indications
        SetEnemyTargetedAnimations(false);

        Invoke("StopPlayerTurnIdle", 0.1f);
    }

    private void StopPlayerTurnIdle()
    {
        // Stop player turn idle animation if it is on
        if (playerReference.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EntityPlayerTurnIdle") == true)
        {
            playerReference.GetComponent<Animator>().Play("EntityIdle");
        }
    }

    private void SetEquipmentPanelVisibility(bool setVisible)
    {
        if (setVisible == true)
        {
            equipmentInfoPanelReference.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(equipmentInfoPanelReference.GetComponent<RectTransform>().anchoredPosition3D.x, equipmentInfoPanelReference.GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);

            // Also set combat damage/description ui so it doesnt block
            combatDescriptionUIReference.gameObject.SetActive(false);

            damageUIReference.gameObject.SetActive(false);
        }
        else
        {
            equipmentInfoPanelReference.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(equipmentInfoPanelReference.GetComponent<RectTransform>().anchoredPosition3D.x, equipmentInfoPanelReference.GetComponent<RectTransform>().anchoredPosition3D.y, -50000.0f);

            // Also set combat damage/description ui so it can be seen again
            combatDescriptionUIReference.gameObject.SetActive(true);
            damageUIReference.gameObject.SetActive(true);
        }
    }

    // *** MAKE SURE THE VALUES USED HERE ARE UP TO DATE AND VALID, VERY DANGEROUS FUNCTION ** //
    public void SpawnEnemies()
    {
        if (enemyPrefabs.Count > 0)
        {
            // Get the battle scenario
            EEnemyBattleScenario battleScenario = GetComponent<GameManagerScript>().currentEnemyBattleScenario;
            print(battleScenario);

            // Check for current battle scenario and spawn the right things
            if (battleScenario == EEnemyBattleScenario.eKobolds)
            {
                // Forest first stage, a group of 2 kobolds
                GameObject newEnemy = Instantiate(enemyPrefabs[2], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-350.0f, -340.0f);

                // Make the rear kobold a torcher
                newEnemy.GetComponent<KoboldScript>().torcherFlag = true;

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[2], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-600.0f, -445.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }


            else if (battleScenario == EEnemyBattleScenario.eGiantWolfSpiderSwarm)
            {
                // Forest second stage, a group of 3 giant wolf spiders

                // Make sure to spawn the back row first
                GameObject newEnemy = Instantiate(enemyPrefabs[1], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-330.0f, -240.0f);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                // Then do front row
                newEnemy = Instantiate(enemyPrefabs[1], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-380.0f, -470.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[1], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-640.0f, -315.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }

            else if (battleScenario == EEnemyBattleScenario.eHarpyHuntresses)
            {
                // Forest third stage, two different harpies

                // Make sure to spawn the back row first
                GameObject newEnemy = Instantiate(enemyPrefabs[3], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-350.0f, -340.0f);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[4], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-612.0f, -410.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }

            else if (battleScenario == EEnemyBattleScenario.eCentipedeDemon)
            {
                // Boss stage, spawn a large centipede demon
                GameObject newEnemy = Instantiate(enemyPrefabs[0], GameObject.Find("Canvas").transform);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }

            else if (battleScenario == EEnemyBattleScenario.eSkeletons)
            {
                // Plains first stage, a group of 2 skeleton soldiers, an archer and a mage

                // Make sure to spawn the back row first

                // Spawn Mage first
                GameObject newEnemy2 = Instantiate(enemyPrefabs[5], GameObject.Find("Canvas").transform);

                newEnemy2.GetComponent<RectTransform>().anchoredPosition = new Vector3(-425.0f, -240.0f);

                // Spawn Archer
                GameObject newEnemy = Instantiate(enemyPrefabs[6], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-235.0f, -350.0f);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                // Add the skeleton mage to the list after archer
                enemiesRear.Add(newEnemy2.GetComponent<EnemyBaseScript>());

                // Then do front row

                // Spawn Soldiers
                newEnemy = Instantiate(enemyPrefabs[7], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-640.0f, -315.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[7], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-410.0f, -445.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eElementalSkylarks)
            {
                // Plains second stage, a fire, frost and lightning skylark

                // Make sure to spawn the back row first

                // Spawn lightning skylark
                GameObject newEnemy2 = Instantiate(enemyPrefabs[8], GameObject.Find("Canvas").transform);

                newEnemy2.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465.0f, -240.0f);

                // Spawn frost skylark
                GameObject newEnemy = Instantiate(enemyPrefabs[9], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-270.0f, -370.0f);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                // Add the lightning skylark to the list after frost
                enemiesRear.Add(newEnemy2.GetComponent<EnemyBaseScript>());

                // Then do front row

                // Spawn fire skylark
                newEnemy = Instantiate(enemyPrefabs[10], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-570.0f, -425.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eAurochs)
            {
                // Plains third stage, two aurochs

                // Make sure to spawn the back row first
                GameObject newEnemy = Instantiate(enemyPrefabs[11], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-330.0f, -265.0f);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[11], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-612.0f, -410.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eIronborne)
            {
                // Boss stage, spawn a large metal dude
                GameObject newEnemy = Instantiate(enemyPrefabs[12], GameObject.Find("Canvas").transform);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eSwampSquad)
            {
                // Swamp first stage, a group of 2 spiders, a skeleton archer and a shadow skylark

                // Make sure to spawn the back row first

                // Shadow skylark first
                GameObject newEnemy2 = Instantiate(enemyPrefabs[13], GameObject.Find("Canvas").transform);

                newEnemy2.GetComponent<RectTransform>().anchoredPosition = new Vector3(-425.0f, -240.0f);

                // Spawn Archer
                GameObject newEnemy = Instantiate(enemyPrefabs[6], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-235.0f, -350.0f);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                // Add the skeleton mage to the list after archer
                enemiesRear.Add(newEnemy2.GetComponent<EnemyBaseScript>());

                // Then do front row

                // Spawn spiders
                newEnemy = Instantiate(enemyPrefabs[1], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-640.0f, -380.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[1], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-410.0f, -490.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eSahuagin)
            {
                // Swamp second stage, four sahuagin

                // Make sure to spawn the back row first
                GameObject newEnemy = Instantiate(enemyPrefabs[14], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-380.0f, -190.0f);

                // Enable rear row flag
                newEnemy.GetComponent<SwampSahuaginScript>().rearFlag = true;

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[14], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-160.0f, -360.0f);

                // Enable rear row flag
                newEnemy.GetComponent<SwampSahuaginScript>().rearFlag = true;

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());

                // Then front row

                newEnemy = Instantiate(enemyPrefabs[14], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-660.0f, -410.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());

                newEnemy = Instantiate(enemyPrefabs[14], GameObject.Find("Canvas").transform);

                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-410.0f, -480.0f);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eWraith)
            {
                // Swamp third stage, a single wraith
                GameObject newEnemy = Instantiate(enemyPrefabs[15], GameObject.Find("Canvas").transform);

                // Add the enemy to the list
                enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eUmbralMass)
            {
                // Boss stage, spawn a disturbing eldritch horror
                GameObject newEnemy = Instantiate(enemyPrefabs[16], GameObject.Find("Canvas").transform);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eVermillionKnight)
            {
                // Boss stage, spawn an imposing knight
                GameObject newEnemy = Instantiate(enemyPrefabs[17], GameObject.Find("Canvas").transform);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
            else if (battleScenario == EEnemyBattleScenario.eGrimDivine)
            {
                // Secret boss
                GameObject newEnemy = Instantiate(enemyPrefabs[18], GameObject.Find("Canvas").transform);

                // Add the enemy to the list
                enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
            }
        }
        else
        {
            print("Enemy prefabs on combat manager not set up");
        }
    }

    // Used by entities or events to create more enemies in the fight
    public void SpawnEnemyDuringCombat(GameObject enemyPrefab, bool shouldSpawnFrontRow)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, GameObject.Find("Canvas").transform);
        newEnemy.GetComponent<EntityBaseScript>().wait = Mathf.Floor(Random.Range(20.0f, 52.0f));

        // Check if this entity should spawn in the front row or the back
        if (shouldSpawnFrontRow)
        {

            // Check if this is the first or second enemy in the row to position it in the right place
            if (enemiesFront.Count == 0)
            {
                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-380.0f, -470.0f);
            }
            else
            {
                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-640.0f, -315.0f);
            }

            // Add the enemy to the list
            enemiesFront.Add(newEnemy.GetComponent<EnemyBaseScript>());
        }

        else
        {

            // Check if this is the first or second enemy in the row to position it in the right place
            newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector3(-235.0f, -350.0f);

            // Add the enemy to the list
            enemiesRear.Add(newEnemy.GetComponent<EnemyBaseScript>());
        }
    }

    // Used to spawn the player's combat inventory as equipment buttons
    private void SpawnEquipmentButtons()
    {
        float spawnX = 475.0f;
        float spawnY = 190.0f;
        Transform equipmentInfoTransform = GameObject.Find("EquipmentInfoPanel").transform;
        for (int i = 0; i < playerManagerReference.combatInventory.Count; i++)
        {
            // Spawn the thing
            GameObject newEquipment = Instantiate(equipmentButtonPrefab, equipmentInfoTransform);

            // Make sure the thing is in the right position
            newEquipment.transform.localPosition = new Vector3(spawnX, spawnY);

            // Add equipment script
            SortieManagerScript.InstantiateEquipmentFromName(newEquipment, playerManagerReference.combatInventory[i]);

            // Add equipment information script
            newEquipment.AddComponent<UIEquipmentInformationScript>();

            // Make sure equipment use on click is linked
            newEquipment.GetComponent<EquipmentBaseScript>().SetUpEquipmentUseButton();

            // Change button text to the name of the equipment
            newEquipment.GetComponentInChildren<Text>().text = playerManagerReference.combatInventory[i].equipmentName;

            // Change the durability animation text
            newEquipment.GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().NotifyDurabilityChange(playerManagerReference.combatInventory[i].durability);

            // Add the button to the reference list
            equipmentButtonReferences.Add(newEquipment);

            // Check equipment broken
            if (newEquipment.GetComponent<EquipmentBaseScript>().durability == 0)
            {
                newEquipment.GetComponent<EquipmentBaseScript>().DelayedEquipmentBreaking();
            }

            // Move spawn y down a bit
            spawnY -= 64.0f;
        }
    }

    public int GetEnemyCount()
    {
        return enemiesFront.Count + enemiesRear.Count;
    }

    // Used to toggle visibility of enemies
    public void DisplayEnemies(bool shouldShow)
    {
        if (shouldShow == true)
        {
            // Enable enemies
            for (int i = 0; i < enemiesFront.Count; i++)
            {
                enemiesFront[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < enemiesRear.Count; i++)
            {
                enemiesRear[i].gameObject.SetActive(true);
            }
        }
        else
        {
            // Disable enemies
            for (int i = 0; i < enemiesFront.Count; i++)
            {
                enemiesFront[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < enemiesRear.Count; i++)
            {
                enemiesRear[i].gameObject.SetActive(false);
            }
        }
    }

    public void SpawnEnemyPlacementIndicators()
    {
        // Do front row
        for (int i = 0; i < enemiesFront.Count; i++)
        {
            // Spawn the indicator
            GameObject newEnemyPlacementIndicator = Instantiate(enemyPlacementIndicatorPrefab, enemiesFront[i].transform);

            // Set indicator location
            newEnemyPlacementIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(-22.0f, -50.0f);

            // Set indicator text
            newEnemyPlacementIndicator.GetComponent<Text>().text = "Front " + (i + 1).ToString();
        }

        // Do rear row
        for (int i = 0; i < enemiesRear.Count; i++)
        {
            // Spawn the indicator
            GameObject newEnemyPlacementIndicator = Instantiate(enemyPlacementIndicatorPrefab, enemiesRear[i].transform);

            // Set indicator location
            newEnemyPlacementIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(-22.0f, -50.0f);

            // Set indicator text
            newEnemyPlacementIndicator.GetComponent<Text>().text = "Rear " + (i + 1).ToString();
        }
    }

    public void CleanUpDeadEnemies()
    {
        // Go through front and rear rows looking for dead enemies
        for (int i = enemiesFront.Count - 1; i >= 0; i--)
        {
            if (enemiesFront[i].deathFlag == true)
            {
                // Destroy and remove them if they are dead
                enemiesFront[i].DestroyAdjusterIcons();
                Destroy(enemiesFront[i].gameObject);
                enemiesFront.RemoveAt(i);
            }
        }

        for (int i = enemiesRear.Count - 1; i >= 0; i--)
        {
            if (enemiesRear[i].deathFlag == true)
            {
                // Destroy and remove them if they are dead
                enemiesRear[i].DestroyAdjusterIcons();
                Destroy(enemiesRear[i].gameObject);
                enemiesRear.RemoveAt(i);
            }
        }
    }

    // Used to check if any enemies are pending death
    public bool CheckForDeadEnemies()
    {
        // Go through front and rear rows looking for dead enemies
        for (int i = enemiesFront.Count - 1; i >= 0; i--)
        {
            if (enemiesFront[i].deathFlag == true)
            {
                return true;
            }
        }

        for (int i = enemiesRear.Count - 1; i >= 0; i--)
        {
            if (enemiesRear[i].deathFlag == true)
            {
                return true;
            }
        }

        return false;
    }

    // Check through all enemies to see if any are alive
    private bool CheckBattleWon()
    {
        // Check front row
        for (int i = 0; i < enemiesFront.Count; i++)
        {
            if (enemiesFront[i].deathFlag == false)
            {
                return false;
            }
        }

        // Check rear row
        for (int i = 0; i < enemiesRear.Count; i++)
        {
            if (enemiesRear[i].deathFlag == false)
            {
                return false;
            }
        }

        // Otherwise all enemies are dead, battle won!
        return true;
    }

    public void SpawnGameOverScreen()
    {
        // Spawn the game over screen
        Instantiate(gameOverScreenPrefab, GameObject.Find("Canvas").transform);

        currentPhase = CombatPhase.END;
    }

    // Used to restore the combat manager back to its pre-combat state
    public void ResetCombatVariables()
    {
        revengeMeter = 0.0f;
        judgementMeter = 0.0f;
        judgementFlag = false;
        currentPhase = CombatPhase.START;
        enemiesFront.Clear();
        enemiesRear.Clear();
        equipmentButtonReferences.Clear();
        canPlayerAct = false;
        canTurnProceed = false;

        StopAllCoroutines();
    }

    public void StartEntityIdleCoroutine(float bpm)
    {
        StartCoroutine(EntityIdleCoroutine(bpm));
    }

    public IEnumerator EntityIdleCoroutine(float bpm)
    {
        // Set it up
        int currentSprite = 0;

        float spriteSwapTimer = Time.deltaTime;

        float spriteSwapSeconds = 60.0f / bpm;

        // The main body
        while (currentPhase != CombatPhase.END)
        {
            // Check if swap
            if (spriteSwapTimer > spriteSwapSeconds)
            {
                // Change sprite number
                currentSprite = (currentSprite + 1) % 2;

                // Do the swap
                for (int i = 0; i < enemiesFront.Count; i++)
                {
                    if (enemiesFront[i].deathFlag != true)
                    {
                        enemiesFront[i].SwapIdleSprite(currentSprite + 1);
                    }
                }

                for (int i = 0; i < enemiesRear.Count; i++)
                {
                    if (enemiesRear[i].deathFlag != true)
                    {
                        enemiesRear[i].SwapIdleSprite(currentSprite + 1);
                    }
                }

                playerReference.SwapIdleSprite(currentSprite + 1);

                // Reset timer
                spriteSwapTimer -= spriteSwapSeconds;
                spriteSwapTimer = Mathf.Clamp(spriteSwapTimer, 0.0f, spriteSwapSeconds);
            }

            // Increase timer
            spriteSwapTimer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
