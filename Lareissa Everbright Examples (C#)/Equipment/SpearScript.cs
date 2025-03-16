using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardBlindChance;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E03", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        damageLowerStandard = 8;
        damageHigherStandard = 15;
        waitCostNormal = 15;

        accuracyJudgement = 100;
        damageLowerJudgement = 36;
        damageHigherJudgement = 50;
        waitCostJudgement = 23;

        standardBlindChance = 20;

        // Set up target and target string
        target = TargetType.Pierce;
        targetJudgement = TargetType.SingleRear;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Fast attack with chance to blind
    public override void UseEquipment()
    {
        // Stop button selection after press
        DisableButtonSelection();

        // Check to see if it's the player's turn
        if (combatManagerReference.canPlayerAct == true && equipmentBrokenFlag == false)
        {
            // Check if should be judgement or normal
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                StartCoroutine(FatalThrow());
            }
            else
            {
                StartCoroutine(SpearThrust());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Fast attack with chance to blind
    private IEnumerator SpearThrust()
    {
        print("Gwenaelle thrusts her spear");

        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle thrusts her spear", 1.5f, false);

        playerReference.PlayAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("SpearThrust", 1.12f);

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

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Chance to inflict blind
            combatManagerReference.ApplyAugmentToEnemies(target, AugmentType.BLIND, standardBlindChance);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
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

        // Use parent function to decrease durability and increase wait
        base.UseEquipment();

        yield return null;
    }

    // Shatters the weapon but always blinds
    public override void UseJudgement()
    {
        StartCoroutine(FatalThrow());
    }

    // Shatters the weapon but always blinds
    private IEnumerator FatalThrow()
    {
        // Change description
        if (playerReference.HasAugment(AugmentType.BLIND))
        {
            combatManagerReference.DisplayCombatDescription("Gwenaelle throws her spear blindly", 2.0f, false);
        }
        else
        {
            combatManagerReference.DisplayCombatDescription("Gwenaelle throws her spear with deadly precision", 2.0f, false);
        }

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("SpearThrow", 1.37f);

        // Decrease durability to 1
        durability = 1;

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // ALWAYS HITS (unless blind)
        if (TestAccuracy(accuracyJudgement))
        {
            // It hits, tell combat manager to inflict damage
            combatManagerReference.InflictDamageEnemy(targetJudgement, damageLowerJudgement, damageHigherJudgement, playerReference);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Check if enemy alive
            if (combatManagerReference.CheckTargetAlive(targetJudgement))
            {
                // Inflict blind
                combatManagerReference.ApplyAugmentToEnemies(targetJudgement, AugmentType.BLIND, 100.0f);

                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            
            // Change description
            combatManagerReference.DisplayCombatDescription("The spear shatters...", 1.5f, false);

            // Check for spear achievement
            if (durability == 1)
            {
                FindObjectOfType<AchievementManagerScript>().UnlockAchievement("ACH_SPEAR");
            }

            yield return new WaitForSeconds(0.1f);
        }
        else
        {
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
