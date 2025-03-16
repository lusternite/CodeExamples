using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//


    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E12", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 80;
        damageLowerStandard = 27;
        damageHigherStandard = 31;
        waitCostNormal = 36;

        accuracyJudgement = 90;
        damageLowerJudgement = 37;
        damageHigherJudgement = 51;
        waitCostJudgement = 47;

        // Set up target and target string
        target = TargetType.SingleRear;
        targetJudgement = TargetType.SingleRear;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Does damage and reduces enemy def
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
                StartCoroutine(HolySplinter());
            }
            else
            {
                StartCoroutine(ShootCrossbow());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Damage front row
    private IEnumerator ShootCrossbow()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle shoots her crossbow", 1.5f, false);

        playerReference.PlayRangedAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("CrossbowShoot", 1.42f);

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
        StartCoroutine(HolySplinter());
    }

    // Damage and heal if kills
    private IEnumerator HolySplinter()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle loads a holy splinter and shoots", 2.0f, false);

        playerReference.PlayRangedJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("HolySplinterShoot", 0.9f);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(accuracyJudgement))
        {
            // It hits, tell combat manager to inflict damage
            combatManagerReference.InflictDamageEnemy(target, damageLowerJudgement, damageHigherJudgement, playerReference);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            combatManagerReference.RemoveCombatDescription();

            // Check if enemy is dead
            if (combatManagerReference.CheckForDeadEnemies())
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("The holy splinter restores Gwenaelle's health", 2.0f, false);

                // Check for crossbow achievement
                if (playerReference.health <= 20.0f)
                {
                    FindObjectOfType<AchievementManagerScript>().UnlockAchievement("ACH_CROSSBOW");
                }

                // Restore health based on damage dealt
                combatManagerReference.RestoreHealthPlayer(combatManagerReference.previousPlayerDamageDealt);

                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
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

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
