using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaymoreScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    [Header("Weapon specific settings")]
    public float judgementDefGainPerEnemy;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E09", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 90;
        damageLowerStandard = 30;
        damageHigherStandard = 33;
        waitCostNormal = 36;

        accuracyJudgement = 100;
        damageLowerJudgement = 31;
        damageHigherJudgement = 35;
        waitCostJudgement = 39;
        judgementDefGainPerEnemy = 10.0f;

        // Set up target and target string
        target = TargetType.FrontLine;
        targetJudgement = TargetType.All;
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
                StartCoroutine(Whirlwind());
            }
            else
            {
                StartCoroutine(SwingClaymore());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Damage front row
    private IEnumerator SwingClaymore()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her claymore", 1.5f, false);

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
        StartCoroutine(Whirlwind());
    }

    // Damage all enemies
    private IEnumerator Whirlwind()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Spinning the claymore, Gwenaelle cleaves through all", 2.0f, false);

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
            int enemiesHit = combatManagerReference.GetEnemyCount();

            // It hits, tell combat manager to inflict damage
            combatManagerReference.InflictDamageEnemy(targetJudgement, damageLowerJudgement, damageHigherJudgement, playerReference);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Increase def for each enemy hit

            // Apply def buff to self
            combatManagerReference.ApplyModifierToPlayer(StatType.DEF, judgementDefGainPerEnemy * enemiesHit);

            // Change description for def gain
            combatManagerReference.DisplayCombatDescription("Gwenaelle enters a defensive stance", 1.5f);
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
