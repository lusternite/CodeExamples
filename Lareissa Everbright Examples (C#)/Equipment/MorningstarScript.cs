using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningstarScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardStunChance;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E07", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 85;
        damageLowerStandard = 25;
        damageHigherStandard = 28;
        waitCostNormal = 41;

        accuracyJudgement = 65;
        damageLowerJudgement = 51;
        damageHigherJudgement = 73;
        waitCostJudgement = 38;

        standardStunChance = 35;

        // Set up target and target string
        target = TargetType.SingleFront;
        targetJudgement = TargetType.SingleFront;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Strong attack that has stun chance
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
                StartCoroutine(RisingSun());
            }
            else
            {
                StartCoroutine(SwingMorningstar());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Strong attack that has stun chance
    private IEnumerator SwingMorningstar()
    {
        print("Gwenaelle swings her morningstar");
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her morningstar", 1.5f, false);

        playerReference.PlayAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("BluntSwing", 1.12f);

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

            // See if stuns
            if (TestAccuracy(standardStunChance) && combatManagerReference.CheckTargetAlive(target))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("The " + combatManagerReference.GetTargetName(target) + " is stunned!", 1.5f, false);

                yield return new WaitForSeconds(0.1f);

                // Apply stun to enemy
                combatManagerReference.ApplyAugmentToEnemies(TargetType.SingleFront, AugmentType.STUN, 100.0f);

                // Do achievement counting
                FindObjectOfType<AchievementManagerScript>().CountStunMorningstar();
            }
            else
            {
                // Reset achievement counting
                FindObjectOfType<AchievementManagerScript>().ResetMorningstarStun();
            }
        }
        else
        {
            print("It misses...");

            combatManagerReference.DisplayCombatDescription("It misses...");

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

    // Very low accuracy powerful strike with guaranteed stun
    public override void UseJudgement()
    {
        StartCoroutine(RisingSun());
    }

    // Very low accuracy powerful strike with guaranteed stun
    private IEnumerator RisingSun()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Spinning to build up speed, Gwenaelle unleashes a powerful skyward swing", 2.0f, false);

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("RisingSun", 0.2f);

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

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Check if target is alive
            if (combatManagerReference.CheckTargetAlive(targetJudgement))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("The " + combatManagerReference.GetTargetName(target) + "is stunned!");

                // Do achievement counting
                FindObjectOfType<AchievementManagerScript>().CountStunMorningstar();

                // Inflict stun
                combatManagerReference.ApplyAugmentToEnemies(TargetType.SingleFront, AugmentType.STUN, 100.0f);

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
            combatManagerReference.DisplayCombatDescription("It misses...");

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
