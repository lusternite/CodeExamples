using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongSwordScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float singleTargetDefBonus;
    public float judgementHealing;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E02", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        damageLowerStandard = 10;
        damageHigherStandard = 13;
        waitCostNormal = 22;

        accuracyJudgement = 100;
        damageLowerJudgement = 24;
        damageHigherJudgement = 26;
        waitCostJudgement = 41;

        singleTargetDefBonus = 50;
        judgementHealing = 50;

        // Set up target and target string
        target = TargetType.SingleFront;
        targetJudgement = TargetType.SingleFront;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Does damage and boosts def if target is alone
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
                StartCoroutine(Duelist());
            }
            else
            {
                StartCoroutine(SwingLongSword());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Decent dmg with def boost when attacking single targets
    private IEnumerator SwingLongSword()
    {
        print("Gwenaelle swings her long sword");

        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her long sword", 1.5f, false);

        playerReference.PlayAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("HeavySwordSwing", 1.12f);

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

            if (combatManagerReference.GetEnemyCount() == 1)
            {
                // Grant def bonus (will need to be revised to check for single targets)
                combatManagerReference.ApplyModifierToPlayer(StatType.DEF, singleTargetDefBonus);

                // Change description for def bonus (revise this too later)
                combatManagerReference.DisplayCombatDescription("Gwenaelle enters a defensive stance", 1.5f);
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

    // Deals double damage against single targets and heals
    public override void UseJudgement()
    {
        StartCoroutine(Duelist());
    }

    // Deals double damage against single targets and heals
    private IEnumerator Duelist()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("In a flash of steel, Gwenaelle displays her skill as a duelist", 2.0f, false);

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("HeavySwordSwingSpecial", 1.37f);

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

            // But check for single target first
            if (combatManagerReference.GetEnemyCount() == 1)
            {
                // Doubled damage
                combatManagerReference.InflictDamageEnemy(target, damageLowerJudgement * 2, damageHigherJudgement * 2, playerReference);
            }
            else
            {
                // Normal damage
                combatManagerReference.InflictDamageEnemy(target, damageLowerJudgement, damageHigherJudgement, playerReference);
            }
            

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Check if should heal

            if (combatManagerReference.GetEnemyCount() == 1)
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle breathes a second wind", 1.5f, false);

                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                // Heal the player against single targets (check for single targets later)
                combatManagerReference.RestoreHealthPlayer(judgementHealing);

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
