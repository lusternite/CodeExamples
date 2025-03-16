using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagStaffScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardDmgBuff;
    public float standardSpdBuff;
    public float judgementDmgBuff;
    public float judgementSpdBuff;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E15", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        damageLowerStandard = 8;
        damageHigherStandard = 11;
        waitCostNormal = 31;

        accuracyJudgement = 100;
        waitCostJudgement = 23;

        standardDmgBuff = 25;
        standardSpdBuff = 25;
        judgementDmgBuff = 100;
        judgementSpdBuff = 50;

        // Set up target and target string
        target = TargetType.SingleFront;
        targetJudgement = TargetType.Self;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Does damage and buffs self
    public override void UseEquipment()
    {
        // Stop button selection after press
        DisableButtonSelection();

        // Check to see if it's the player's turn and equipment isn't broken
        if (combatManagerReference.canPlayerAct == true && equipmentBrokenFlag == false)
        {
            // Check if should be judgement or normal
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                StartCoroutine(InspireOneself());
            }
            else
            {
                StartCoroutine(SwingFlagStaff());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Decent dmg with def boost when attacking single targets
    private IEnumerator SwingFlagStaff()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her flag staff", 1.5f, false);

        playerReference.PlayAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("FlagStaffSwing", 1.12f);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(accuracyNormal))
        {
            // It hits, tell combat manager to inflict damage
            combatManagerReference.InflictDamageEnemy(target, damageLowerStandard, damageHigherStandard, playerReference);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Change description for buff
            combatManagerReference.DisplayCombatDescription("Gwenaelle feels empowered", 1.5f);

            // Apply dmg and spd buff to player
            combatManagerReference.ApplyModifierToPlayer(StatType.DMG, standardDmgBuff);
            combatManagerReference.ApplyModifierToPlayer(StatType.SPD, standardSpdBuff);
        }
        else
        {
            print("It misses...");

            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);
        }

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

    // Deals double damage against single targets and heals
    public override void UseJudgement()
    {
        StartCoroutine(InspireOneself());
    }

    // Deals double damage against single targets and heals
    private IEnumerator InspireOneself()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle waves the flag staff and defiantly plants it", 2.0f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("FlagStaffInspire");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Buff dmg and spd
        combatManagerReference.ApplyModifierToPlayer(StatType.DMG, judgementDmgBuff);
        combatManagerReference.ApplyModifierToPlayer(StatType.SPD, judgementSpdBuff);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
