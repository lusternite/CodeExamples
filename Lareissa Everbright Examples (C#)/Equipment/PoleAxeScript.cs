using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleAxeScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    [Tooltip("Make sure this number is negative")]
    public float judgementDefReductionAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start () {
        base.Start();

        CSVReaderScript.ReadEquipmentData("E01", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 80.0f;
        damageLowerStandard = 24.0f;
        damageHigherStandard = 41.0f;
        waitCostNormal = 37.0f;

        accuracyJudgement = 100.0f;
        damageLowerJudgement = 34.0f;
        damageHigherJudgement = 42.0f;
        waitCostJudgement = 55.0f;

        judgementDefReductionAmount = -30.0f;

        // Set up target and target string
        target = TargetType.SingleFront;
        targetJudgement = TargetType.SingleFront;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Just damaging
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
                StartCoroutine(ArmourShatterer());
            }
            else
            {
                StartCoroutine(SwingPoleAxe());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    private IEnumerator SwingPoleAxe()
    {
        print("Gwenaelle swings her pole axe");
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her pole axe", 1.5f, false);

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

    // Hits hard and reduces DEF
    public override void UseJudgement()
    {
        StartCoroutine(ArmourShatterer());
    }

    private IEnumerator ArmourShatterer()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle crushes with full force", 2.0f, false);

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("PowerSwing", 1.37f);

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
                combatManagerReference.DisplayCombatDescription("The " + combatManagerReference.GetTargetName(targetJudgement) + "'s defenses have dropped", 1.5f, false);

                // Also tell combat manager to add -def modifier to enemy
                combatManagerReference.ApplyModifierToEnemies(targetJudgement, StatType.DEF, judgementDefReductionAmount, 100.0f);

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
            print("It misses..");

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
