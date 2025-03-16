using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncenseScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float judgementDmgReductionAmount;
    public float judgementDefReductionAmount;
    public float judgementRevengeReductionAmount;
    public float standardJudgementIncreaseAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E13", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        waitCostNormal = 10;

        accuracyJudgement = 100;
        waitCostJudgement = 23;

        standardJudgementIncreaseAmount = 20;

        judgementDmgReductionAmount = -30;
        judgementDefReductionAmount = -30;
        judgementRevengeReductionAmount = -50;

        // Set up target and target string
        target = TargetType.Self;
        targetJudgement = TargetType.All;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Minor heal and cures poisons
    public override void UseEquipment()
    {
        // Stop button selection after press
        DisableButtonSelection();

        // Check to see if it's the player's turn and isnt equipment isnt broken
        if (combatManagerReference.canPlayerAct == true && equipmentBrokenFlag == false)
        {
            // Check if should be judgement or normal
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                StartCoroutine(DemonbaneSmoke());
            }
            else
            {
                StartCoroutine(LightIncense());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    public IEnumerator LightIncense()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle lights incense to cleanse herself", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("IncenseLight");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check for incense achievement
        if (playerReference.GetNegativeModifierCount() >= 3)
        {
            FindObjectOfType<AchievementManagerScript>().UnlockAchievement("ACH_INCENSE");
        }

        // Tell player to remove all negative modifiers

        playerReference.RemoveAllNegativeModifiers();

        // Increase judgement meter
        // Check if battlefield placebo is up
        if (playerReference.HasModifier(StatType.DEF) == true)
        {
            if (playerReference.GetModifier(StatType.DEF).modifierValue == 60.0f)
            {
                combatManagerReference.judgementMeter += standardJudgementIncreaseAmount * 3.0f;
            }
            else
            {
                combatManagerReference.judgementMeter += standardJudgementIncreaseAmount;
            }
        }
        else
        {
            combatManagerReference.judgementMeter += standardJudgementIncreaseAmount;
        }
        
        if (combatManagerReference.judgementMeter >= 100.0f)
        {
            print("Judgement meter is full");
            combatManagerReference.judgementMeter = 100.0f;
            combatManagerReference.judgementPureButtonReference.GetComponent<UIJudgementPureButtonScript>().SetJudgementButtonColor(true);
            combatManagerReference.judgementPureButtonReference.image.enabled = true;
        }

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait
        base.UseEquipment();

        yield return null;
    }

    // Grants the player a def bonus
    public override void UseJudgement()
    {
        StartCoroutine(DemonbaneSmoke());
    }

    // Grants the player a def bonus
    private IEnumerator DemonbaneSmoke()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("A white smoke surrounds the battlefield", 2.0f);
        yield return new WaitForSeconds(0.1f);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("DemonbaneSmoke");

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Tell combat manager to remove positive mods from all enemies
        combatManagerReference.RemovePositiveModifiersFromEnemies();

        // Change description
        combatManagerReference.DisplayCombatDescription("All positive enemy modifiers removed!", 1.5f);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Tell combat manager to reduce dmg and def of enemies
        combatManagerReference.ApplyModifierToEnemies(targetJudgement, StatType.DEF, judgementDefReductionAmount, 100.0f);
        combatManagerReference.ApplyModifierToEnemies(targetJudgement, StatType.DMG, judgementDmgReductionAmount, 100.0f);

        // Change description
        combatManagerReference.DisplayCombatDescription("Enemy strength and defences reduced!", 1.5f);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Tell combat manager to reduce revenge
        combatManagerReference.revengeMeter += judgementRevengeReductionAmount;
        combatManagerReference.revengeMeter = Mathf.Clamp(combatManagerReference.revengeMeter, 0.0f, 100.0f);

        // Change description
        combatManagerReference.DisplayCombatDescription("Enemy revenge suppressed!", 1.5f);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
