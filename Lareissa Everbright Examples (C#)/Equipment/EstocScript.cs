using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstocScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float enemyDefReductionAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start ()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E08", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        damageLowerStandard = 18;
        damageHigherStandard = 22;
        waitCostNormal = 44;

        accuracyJudgement = 100;
        damageLowerJudgement = 40;
        damageHigherJudgement = 45;
        waitCostJudgement = 50;

        enemyDefReductionAmount = -20;

        // Set up target and target string
        target = TargetType.Pierce;
        targetJudgement = TargetType.Pierce;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }
	
	// Update is called once per frame
	void Update () {
		
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
                StartCoroutine(DrillThrust());
            }
            else
            {
                StartCoroutine(ThrustEstoc());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Decent dmg with def boost when attacking single targets
    private IEnumerator ThrustEstoc()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle thrusts her estoc", 1.5f, false);

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

            // Check if target is alive
            if (combatManagerReference.CheckTargetAlive(target))
            {
                // Apply def drop to hit enemies
                combatManagerReference.ApplyModifierToEnemies(target, StatType.DEF, enemyDefReductionAmount, 100.0f);

                // Change description for def drop
                combatManagerReference.DisplayCombatDescription("Enemy defenses have dropped", 1.5f);
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
        StartCoroutine(DrillThrust());
    }

    // Deals double damage against single targets and heals
    private IEnumerator DrillThrust()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("With a twisting maneuver, the estoc pierces forward", 2.0f, false);

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("DrillThrust", 1.37f);

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

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // CHeck if target alive
            if (combatManagerReference.CheckTargetAlive(targetJudgement))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("The thrust causes bleeding", 1.5f, false);

                // Apply bleed to enemies
                combatManagerReference.ApplyAugmentToEnemies(TargetType.Pierce, AugmentType.BLEED, 100.0f);

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
