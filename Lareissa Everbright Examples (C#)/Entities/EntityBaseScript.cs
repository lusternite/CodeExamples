using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// All entities inherit from this base class
public class EntityBaseScript : MonoBehaviour
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Tooltip("How much damage this thing can take")]
    public float health;
    public float maxHealth;

    public string entityName;

    // The amount of time until this entity's next turn
    public float wait;

    // The stack of wait that has piled up on other entitys' turns
    // Mainly used to tick down modifiers and augments and other things
    // That rely on wait that occur on the entity's turn
    public float waitStack;

    public Animator healthAnimatorReference;

    public Sprite idleSprite1;
    public Sprite idleSprite2;
    public Image entityImageReference;

    protected bool canAct;

    // Used to notify if is currently processing tickdown
    public bool tickdownFlag;

    // Very weird float that is used to make sure wt displays are accurate
    // on equipment that modifies speed
    public float spdRefreshPreviousAmount;

    protected CombatManagerScript combatManagerReference;

    protected List<ModifierScript> modifiers;

    protected List<AugmentScript> augments;

    // Mods + Augments are adjusters
    protected List<GameObject> adjusterIcons;

    public float defaultAdjusterIconX = -156.0f;

    public float defaultAdjusterIconY = 80.0f;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    protected virtual void Start()
    {
        combatManagerReference = CombatManagerScript.GetCombatManager();
        augments = new List<AugmentScript>();
        modifiers = new List<ModifierScript>();
        adjusterIcons = new List<GameObject>();
        canAct = true;
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // What ticks down at the start of this entity's turn, literally just starts a coroutine
    public virtual void TickDown()
    {
        StartCoroutine(HandleTickDownUI());

    }

    public IEnumerator HandleTickDownUI()
    {
        canAct = true;
        tickdownFlag = true;
        print("Tickdown starting");

        // Reset the spd bug value
        spdRefreshPreviousAmount = 0.0f;

        // Tick each modifier and augment, then check if any have expired
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].TickDown(waitStack);
            if (modifiers[i].modifierDuration <= 0)
            {
                // Remove the adjuster icon
                RemoveModAdjusterIcon(modifiers[i].modifierType);

                Destroy(modifiers[i], 0.5f);
                modifiers.RemoveAt(i);
            }
        }
        print("Completed modifiers");

        for (int i = augments.Count - 1; i >= 0; i--)
        {
            augments[i].TickDown(waitStack);

            yield return new WaitForSeconds(0.03f);

            // Wait until turn can proceed if necessary
            while (combatManagerReference.CanTurnProceed() == false || combatManagerReference.IsCombatDescriptionShowing())
            {
                print(" CanTurnProceed = " + combatManagerReference.CanTurnProceed() + " and CombatShowing = " + combatManagerReference.IsCombatDescriptionShowing());
                yield return new WaitForSeconds(0.1f);
            }
            
            if (augments[i].augmentDuration <= 0)
            {
                // Check if this is banish
                if (augments[i].augmentType == AugmentType.BANISH)
                {
                    // Make sure to restore the equipment
                    ((AugBanishScript)augments[i]).equipmentReference.RestoreEquipment();
                }
                // Remove adjuster icon
                RemoveAugAdjusterIcon(augments[i].augmentType);

                Destroy(augments[i], 0.5f);
                augments.RemoveAt(i);
            }
        }

        print("Completed augments");

        // Clean up adjuster icons
        CleanUpAdjusterIcons();

        waitStack = 0.0f;

        // Check for stun
        if (HasAugment(AugmentType.STUN))
        {
            // Change description
            combatManagerReference.DisplayCombatDescription(entityName + " is stunned and cannot act!");

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Disable action for this turn, add wait, then remove stun
            canAct = false;

            wait += ((AugStunScript)GetAugment(AugmentType.STUN)).stunWaitDuration;

            RemoveAugment(AugmentType.STUN);
            RemoveAugAdjusterIcon(AugmentType.STUN);

            // Clean up adjuster icons
            CleanUpAdjusterIcons();
        }

        // Make sure to allow combat manager to proceed.
        combatManagerReference.SetCanTurnProceed(true);
        tickdownFlag = false;
        print("Tickdown ending");
        yield return null;
    }

    // Coroutines to handle wait ui
    public IEnumerator ChangeWaitTime(float waitAmount)
    {
        // Prevent turn from continuing
        combatManagerReference.SetCanTurnProceed(false);

        float uiChangeDuration = 2.0f;
        float uiCoroutineTickTime = 0.1f;

        // Then slowly change health for a certain amount of ticks
        float waitPerTick = waitAmount * (uiChangeDuration / uiCoroutineTickTime);

        for (int i = 0; i < Mathf.Ceil(uiChangeDuration / uiCoroutineTickTime); i++)
        {
            wait += waitPerTick;

            yield return new WaitForSeconds(uiCoroutineTickTime);
        }

        // Allow turn to continue
        combatManagerReference.SetCanTurnProceed(true);

        yield return null;
    }

    // Check to see if entity can act
    public bool CanEntityAct()
    {
        return canAct;
    }

    // Deal damage to this entity
    // Mainly used by non actions eg. venom/poison
    public void InflictDamageEntity(string description, float damage)
    {
        // Halt turn progression until ui is complete
        combatManagerReference.SetCanTurnProceed(false);

        // Start damage coroutine
        StartCoroutine(HandleDamageEntityUI(description, damage));
    }

    // The ui stuff for dealing damage directly to this entity
    public IEnumerator HandleDamageEntityUI(string description, float damage)
    {
        // First display the description
        combatManagerReference.DisplayCombatDescription(description, 1.0f, false);
        yield return new WaitForSeconds(0.1f);

        print("Description Displayed");

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Then display the total damage taken
        combatManagerReference.damageUIReference.DisplayDamage(damage);

        // Spawn a damage number
        GameObject damageNumber = Instantiate(FindObjectOfType<CombatManagerScript>().damageNumberPrefab, GameObject.Find("Canvas").transform);
        damageNumber.GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMax;
        damageNumber.GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().anchorMin;
        damageNumber.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        

        // Play damage sfx
        if (combatManagerReference.currentPhase == CombatPhase.PLAYERTURN)
        {
            FindObjectOfType<AudioManagerScript>().PlayEntitySFX("GwenDamageStandard");
            SetHealthAnimationFlag(true);
            PlayDamageAnimationPlayer();

            damageNumber.GetComponent<UIDamageNumberScript>().Initialise(damage, true);
        }
        else
        {
            FindObjectOfType<AudioManagerScript>().PlayWeaponSFX("WeaponImpactStandard");
            SetHealthAnimationFlag(true);
            PlayDamageAnimationEnemy();

            damageNumber.GetComponent<UIDamageNumberScript>().Initialise(damage, false);
        }

        print("Damage Displayed");

        // Check if this damage kills
        if (damage >= health)
        {
            damage = health;
        }

        // Then slowly reduce health for a certain amount of ticks
        float healthPerTick = damage / (combatManagerReference.uiDisplayTime / combatManagerReference.uiCoroutineTickTime);

        for (int i = 0; i < Mathf.Ceil(combatManagerReference.uiDisplayTime / combatManagerReference.uiCoroutineTickTime); i++)
        {
            health -= healthPerTick;

            yield return new WaitForSeconds(combatManagerReference.uiCoroutineTickTime);
        }

        // Turn off health animation
        SetHealthAnimationFlag(false);

        // Wait a while for this to register
        yield return new WaitForSeconds(1.0f);

        // Then remove the description and damage ui
        combatManagerReference.RemoveCombatDescription();
        combatManagerReference.damageUIReference.StopDisplay();

        // Handle player/enemy specific things
        if (combatManagerReference.currentPhase == CombatPhase.PLAYERTURN)
        {
            // Check if the player is dead
            if (health <= 0.0f)
            {
                // Check if player is unyielding
                if (HasAugment(AugmentType.UNYIELDING))
                {
                    // Restore player back to 1 hp
                    health = 1.0f;

                    // Handle unyielding charges
                    ((AugUnyieldingScript)GetAugment(AugmentType.UNYIELDING)).ProcessUndeath();

                    // Change Description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle resists death!");

                    yield return new WaitForSeconds(1.5f);

                    // Allow turn to continue
                    combatManagerReference.SetCanTurnProceed(true);
                }
                // Otherwise they have lost
                else
                {
                    combatManagerReference.RemoveCombatDescription();

                    combatManagerReference.DisplayCombatDescription("Gwenaelle falls...", 5.0f, false);

                    // Fade out bgm
                    FindObjectOfType<AudioManagerScript>().FadeOutBGM(2.5f);
                    
                    // Wait a while for this to register
                    yield return new WaitForSeconds(2.5f);

                    combatManagerReference.SpawnGameOverScreen();

                    // Play defeat sfx
                    FindObjectOfType<AudioManagerScript>().PlayCombatSFX("Defeat");

                    // Stop this coroutine
                    StopAllCoroutines();
                }
            }
        }
        else
        {
            // Check if the enemy is dead
            if (health <= 0.0f)
            {

                ((EnemyBaseScript)this).HandleDeath();

                canAct = false;

                combatManagerReference.damageUIReference.StopDisplay();

                // Check if this is final boss for unique death quote
                if (FindObjectOfType<GameManagerScript>().currentAct == 4)
                {
                    combatManagerReference.DisplayCombatDescription(entityName + " is defeated.", 3.0f);
                }
                else
                {
                    combatManagerReference.DisplayCombatDescription(entityName + " vanishes into mist.", 3.0f);

                    // Do death anim
                    GetComponent<Coffee.UIExtensions.UIDissolve>().Play();

                    // Spawn enemy death particle on enemy
                    GameObject deathParticle = Instantiate(combatManagerReference.enemyDeathParticlePrefab, GameObject.Find("Canvas").transform);
                    deathParticle.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                    // Play death sfx
                    FindObjectOfType<AudioManagerScript>().PlayEntitySFX("EnemyDeath");

                    // Check bleed death achievement
                    if (description == "Pain springs from a deep gash")
                    {
                        FindObjectOfType<AchievementManagerScript>().CountEnemyBleedDefeat();
                    }

                    yield return new WaitForSeconds(3.0f);

                    // Make sure to allow combat manager to proceed.
                    combatManagerReference.SetCanTurnProceed(true);
                    tickdownFlag = false;
                    print("Tickdown ending");

                    // Kill coroutines
                    StopAllCoroutines();
                }
            }
        }

        // Resume turn progression if entity is still alive
        if (health > 0)
        {
            combatManagerReference.SetCanTurnProceed(true);
        }

        yield return null;
    }

    // Checks if entity current has specified modifier
    public bool HasModifier(StatType desiredModifier)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].modifierType == desiredModifier)
            {
                return true;
            }
        }
        return false;
    }

    // Checks if entity currently has any modifiers at all
    public bool HasAnyModifiers()
    {
        if (modifiers.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Checks if entity currently has any positive modifiers
    public bool HasAnyPositiveModifiers()
    {
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            // return true if positive
            if (modifiers[i].modifierValue > 0.0f)
            {
                return true;
            }
        }
        return false;
    }

    // Returns the specified modifier
    public ModifierScript GetModifier(StatType desiredModifier)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].modifierType == desiredModifier)
            {
                return modifiers[i];
            }
        }
        return null;
    }

    // Returns the specified modifier in list
    public ModifierScript GetModifierAtIndex(int index)
    {
        return modifiers[index];
    }

    // Returns the first positive modifier
    public ModifierScript GetFirstPositiveModifier()
    {
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            // return true if positive
            if (modifiers[i].modifierValue > 0.0f)
            {
                return modifiers[i];
            }
        }
        return null;
    }
    
    // Returns the amount of modifiers
    public int GetModifierCount()
    {
        return modifiers.Count;
    }

    // Returns amount of negative modifiers
    public int GetNegativeModifierCount()
    {
        int amount = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].modifierValue < 0)
            {
                amount += 1;
            }
        }
        return amount;
    }

    // Flips modifiers
    public void FlipModifiers(bool positiveToNegativeFlip)
    {
        // Check flip type
        if (positiveToNegativeFlip == true)
        {
            // Go through modifiers
            for (int i = 0; i < modifiers.Count; i++)
            {
                // Check if positive
                if (modifiers[i].modifierValue > 0)
                {
                    // Flip if positive
                    modifiers[i].Instantiate(modifiers[i].modifierType, modifiers[i].modifierValue * -1.0f);

                    // Reset the adjuster icon
                    ResetModAdjusterIcon(modifiers[i].modifierType, modifiers[i].modifierValue);
                }

            }
        }
        else
        {
            // Go through modifiers
            for (int i = 0; i < modifiers.Count; i++)
            {
                // Check if negative
                if (modifiers[i].modifierValue < 0)
                {
                    // Flip if negative
                    modifiers[i].Instantiate(modifiers[i].modifierType, modifiers[i].modifierValue * -1.0f);

                    // Reset the adjuster icon
                    ResetModAdjusterIcon(modifiers[i].modifierType, modifiers[i].modifierValue);
                }

            }
        }
    }

    public void AddModifier(StatType desiredModifier, float amount)
    {
        // Check if modifier exists
        if (HasModifier(desiredModifier))
        {
            // Check if this modifier is spd
            if (desiredModifier == StatType.SPD)
            {
                // Remember this value
                spdRefreshPreviousAmount = GetModifier(desiredModifier).modifierValue;
            }

            // Reset the modifier
            GetModifier(desiredModifier).Instantiate(desiredModifier, amount);

            // Reset the adjuster icon
            ResetModAdjusterIcon(desiredModifier, amount);
        }
        else
        {
            // Create the modifier
            ModifierScript newModifier = gameObject.AddComponent<ModifierScript>();
            newModifier.Instantiate(desiredModifier, amount);

            // Add the modifier to the list
            modifiers.Add(newModifier);

            // Add adjuster icon
            AddModAdjusterIcon(desiredModifier, amount);
        }

        // Play sfx
        if (amount > 0.0f)
        {
            FindObjectOfType<AudioManagerScript>().PlayCombatSFX("ModifierBuff");
        }
        else
        {
            FindObjectOfType<AudioManagerScript>().PlayCombatSFX("ModifierDebuff");
        }
    }

    public void RemoveModifier(StatType modifier)
    {
        // Check if augment exists
        if (HasModifier(modifier))
        {
            GetModifier(modifier).DestroyModifier();

            // Destroy modifier icon
            RemoveModAdjusterIcon(modifier);

            // Remove the modifier
            modifiers.Remove(GetModifier(modifier));

            CleanUpAdjusterIcons();
        }
    }

    public void RemoveAllNegativeModifiers()
    {
        // Go through all modifiers
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            // Remove if negative
            if (modifiers[i].modifierValue < 0.0f)
            {
                RemoveModifier(modifiers[i].modifierType);
            }
        }
    }

    public void RemoveAllPositiveModifiers()
    {
        // Go through all modifiers
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            // Remove if positive
            if (modifiers[i].modifierValue > 0.0f)
            {
                RemoveModifier(modifiers[i].modifierType);
            }
        }
    }

    public void MultiplyAllPositiveModifiers(float scale)
    {
        // Go through modifiers
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            // Check if positive
            if (modifiers[i].modifierValue > 0.0f)
            {
                // Double check if this is speed stat
                if (modifiers[i].modifierType == StatType.SPD)
                {
                    // Reset modifier with their values multiplied but clamped to 80
                    AddModifier(modifiers[i].modifierType, Mathf.Clamp(modifiers[i].modifierValue * scale, 0.0f, 80.0f));
                }

                // Also double check if this is def stat
                else  if (modifiers[i].modifierType == StatType.DEF)
                {
                    // Reset modifier with their values multiplied but clamped to 100
                    AddModifier(modifiers[i].modifierType, Mathf.Clamp(modifiers[i].modifierValue * scale, 0.0f, 100.0f));
                }
                
                // Otherwise just multiply it
                else
                {
                    // Reset modifier with their values multiplied
                    AddModifier(modifiers[i].modifierType, modifiers[i].modifierValue * scale);
                }
            }
        }
    }

    // Checks if entity current has specified augment
    public bool HasAugment(AugmentType desiredAugment)
    {
        for (int i = 0; i < augments.Count; i++)
        {
            if (augments[i].augmentType == desiredAugment)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasAnyAugments()
    {
        if (augments.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Returns the specified augment
    public AugmentScript GetAugment(AugmentType desiredAugment)
    {
        for (int i = 0; i < augments.Count; i++)
        {
            if (augments[i].augmentType == desiredAugment)
            {
                return augments[i];
            }
        }
        return null;
    }

    public void AddAugment(AugmentType augmentType)
    {
        // Check if augment exists
        if (HasAugment(augmentType))
        {
            // Reset the duration
            GetAugment(augmentType).ResetDuration();

            // Also reset the adjuster icon
            ResetAugAdjusterIcon(augmentType);
        }
        else
        {
            // Determine which augment to add based on the type
            if (augmentType == AugmentType.BLIND)
            {
                // Create the augment
                AugBlindScript newAugment = gameObject.AddComponent<AugBlindScript>();
                //newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentNegative");
            }
            else if (augmentType == AugmentType.VENOM)
            {
                // Create the augment
                AugVenomScript newAugment = gameObject.AddComponent<AugVenomScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentNegative");
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentNegative");
            }
            else if (augmentType == AugmentType.STUN)
            {
                // Create the augment
                AugStunScript newAugment = gameObject.AddComponent<AugStunScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentNegative");
            }
            else if (augmentType == AugmentType.BLOCK)
            {
                // Create the augment
                AugBlockScript newAugment = gameObject.AddComponent<AugBlockScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentPositive");
            }
            else if (augmentType == AugmentType.UNYIELDING)
            {
                // Create the augment
                AugUnyieldingScript newAugment = gameObject.AddComponent<AugUnyieldingScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentPositive");
            }
            else if (augmentType == AugmentType.BLEED)
            {
                // Create the augment
                AugBleedScript newAugment = gameObject.AddComponent<AugBleedScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentNegative");
            }
            else if (augmentType == AugmentType.BANISH)
            {
                // Create the augment
                AugBanishScript newAugment = gameObject.AddComponent<AugBanishScript>();
                newAugment.Instantiate(this);

                // Make sure to actually banish the right equipment
                // But check that it can be banished first
                if (combatManagerReference.previousUsedEquipment.equipmentBrokenFlag == false)
                {
                    combatManagerReference.previousUsedEquipment.HandleEquipmentBanishing();
                    newAugment.equipmentReference = combatManagerReference.previousUsedEquipment;
                }

                // Add the augment to the list
                augments.Add(newAugment);

                // Play sfx
                FindObjectOfType<AudioManagerScript>().PlayCombatSFX("AugmentNegative");
            }
            else if (augmentType == AugmentType.DYSPHORIA)
            {
                // Create the augment
                AugDysphoriaScript newAugment = gameObject.AddComponent<AugDysphoriaScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);
            }
            else if (augmentType == AugmentType.COUNTER)
            {
                // Create the augment
                AugCounterScript newAugment = gameObject.AddComponent<AugCounterScript>();
                newAugment.Instantiate(this);

                // Add the augment to the list
                augments.Add(newAugment);
            }
            // Add adjuster icon
            AddAugAdjusterIcon(augmentType);
        }
    }

    public int GetAugmentCount()
    {
        return augments.Count;
    }

    public void RemoveAugment(AugmentType augment)
    {
        // Check if augment exists
        if (HasAugment(augment))
        {
            GetAugment(augment).DestroyAugment();

            // Destroy adjuster icon
            RemoveAugAdjusterIcon(augment);

            // Remove the augment
            augments.Remove(GetAugment(augment));

            CleanUpAdjusterIcons();
        }
    }

    public void RemoveAllAdjusters()
    {
        // Remove modifiers
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].DestroyModifier();
            RemoveModAdjusterIcon(modifiers[i].modifierType);
            // Remove the mod
            modifiers.Remove(modifiers[i]);
        }

        // Remove augments
        for (int i = augments.Count - 1; i >= 0; i--)
        {
            augments[i].DestroyAugment();
            RemoveAugAdjusterIcon(augments[i].augmentType);
            // Remove the aug
            augments.Remove(augments[i]);
        }
    }

    public void SetHealthAnimationFlag(bool newAnimState)
    {
        healthAnimatorReference.SetBool("IsHealthDamaging", newAnimState);
    }

    public void PlayDamageAnimationPlayer()
    {
        GetComponent<Animator>().Play("EntityPlayerDamaged");
    }

    public void PlayDamageAnimationEnemy()
    {
        GetComponent<Animator>().Play("EntityEnemyDamaged");
    }

    public void PlayAttackAnimationPlayer()
    {
        GetComponent<Animator>().Play("EntityPlayerAttack");
    }

    public void PlayJudgementAttackAnimationPlayer()
    {
        GetComponent<Animator>().Play("EntityPlayerJudgementAttack");
    }

    public void PlayRangedAttackAnimationPlayer()
    {
        GetComponent<Animator>().Play("EntityPlayerRangedAttack");
    }

    public void PlayRangedJudgementAttackAnimationPlayer()
    {
        GetComponent<Animator>().Play("EntityPlayerRangedJudgementAttack");
    }

    public void PlayAttackAnimationEnemy()
    {
        GetComponent<Animator>().Play("EntityEnemyAttack");
    }

    public void PlayRevengeAttackAnimationEnemy()
    {
        GetComponent<Animator>().Play("EntityEnemyRevengeAttack");
    }

    public void PlayTurnStartAnimation()
    {
        GetComponent<Animator>().Play("EntityTurnStart");
    }

    public void SwapIdleSprite(int spriteNumber)
    {
        // make sure to check if valid first.
        if (entityImageReference)
        {
            if (spriteNumber == 1)
            {
                entityImageReference.sprite = idleSprite1;
            }
            else if (spriteNumber == 2)
            {
                entityImageReference.sprite = idleSprite2;
            }
        }
    }

    public void AddAugAdjusterIcon(AugmentType augmentType)
    {
        // Create the thing
        GameObject newAdjusterIcon = Instantiate(combatManagerReference.adjusterIconPrefab, gameObject.transform);
        newAdjusterIcon.GetComponent<UIAdjusterIconScript>().InitialiseAugment(augmentType);

        // Set reference to this
        newAdjusterIcon.GetComponent<UIAdjusterIconScript>().entityReference = this;

        // Move it to the right location
        newAdjusterIcon.GetComponent<RectTransform>().localPosition = new Vector3(defaultAdjusterIconX, defaultAdjusterIconY - (56.0f * adjusterIcons.Count));

        // Set its parent to canvas so it can overlap things
        newAdjusterIcon.transform.SetParent(GameObject.Find("Canvas").transform);

        // Add it to the list
        adjusterIcons.Add(newAdjusterIcon);
    }

    public void AddModAdjusterIcon(StatType statType, float statValue)
    {
        // Create the thing
        GameObject newAdjusterIcon = Instantiate(combatManagerReference.adjusterIconPrefab, gameObject.transform);
        newAdjusterIcon.GetComponent<UIAdjusterIconScript>().InitialiseModifier(statType, statValue);

        // Set reference to this
        newAdjusterIcon.GetComponent<UIAdjusterIconScript>().entityReference = this;

        // Move it to the right location
        newAdjusterIcon.GetComponent<RectTransform>().localPosition = new Vector3(defaultAdjusterIconX, defaultAdjusterIconY - (56.0f * adjusterIcons.Count));

        // Set its parent to canvas so it can overlap things
        newAdjusterIcon.transform.SetParent(GameObject.Find("Canvas").transform, true);

        // Add it to the list
        adjusterIcons.Add(newAdjusterIcon);
    }

    public void RemoveAugAdjusterIcon(AugmentType augType)
    {
        for (int i = adjusterIcons.Count - 1; i >= 0; i--)
        {
            if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().augmentFlag == true)
            {
                if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().augmentType == augType)
                {
                    Destroy(adjusterIcons[i]);
                    adjusterIcons.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void RemoveModAdjusterIcon(StatType modType)
    {
        for (int i = adjusterIcons.Count - 1; i >= 0; i--)
        {
            if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().augmentFlag == false)
            {
                if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().modifierType == modType)
                {
                    Destroy(adjusterIcons[i]);
                    adjusterIcons.RemoveAt(i);
                    print("Adjuster removed");
                    break;
                }
            }
        }
    }

    private void ResetModAdjusterIcon(StatType desiredModifier, float newModAmount)
    {
        for (int i = 0; i < adjusterIcons.Count; i++)
        {
            if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().augmentFlag == false)
            {
                if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().modifierType == desiredModifier)
                {
                    adjusterIcons[i].GetComponent<UIAdjusterIconScript>().ResetModifierIcon(newModAmount);
                }
            }
        }
    }

    private void ResetAugAdjusterIcon(AugmentType augment)
    {
        for (int i = 0; i < adjusterIcons.Count; i++)
        {
            if (adjusterIcons[i].GetComponent<UIAdjusterIconScript>().augmentFlag == true)
            {
                adjusterIcons[i].GetComponent<UIAdjusterIconScript>().ResetEndingStatus();
            }
        }
    }

    // Used to reorganise adjuster icons after some have been deleted
    public void CleanUpAdjusterIcons()
    {
        for (int i = 0; i < adjusterIcons.Count; i++)
        {
            // Set parents to make local adjustments easy
            adjusterIcons[i].transform.SetParent(gameObject.transform, true);
            adjusterIcons[i].GetComponent<RectTransform>().localPosition = new Vector3(defaultAdjusterIconX, defaultAdjusterIconY - (56.0f * i));
            adjusterIcons[i].transform.SetParent(GameObject.Find("Canvas").transform, true);
        }
    }

    public StatType GenerateRandomStatType()
    {
        float randomStat = Random.Range(0.0f, 100.0f);

        if (randomStat < 25.0f)
        {
            return StatType.ACC;
        }
        else if (randomStat < 50.0f)
        {
            return StatType.DEF;
        }
        else if (randomStat < 75.0f)
        {
            return StatType.DMG;
        }
        else
        {
            return StatType.SPD;
        }
    }
}
