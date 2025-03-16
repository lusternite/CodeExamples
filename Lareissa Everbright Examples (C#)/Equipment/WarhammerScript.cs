using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarhammerScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardSpeedReductionAmount;
    public float judgementSpeedReductionAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E10", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 89;
        damageLowerStandard = 16;
        damageHigherStandard = 22;
        waitCostNormal = 38;

        accuracyJudgement = 89;
        damageLowerJudgement = 45;
        damageHigherJudgement = 48;
        waitCostJudgement = 48;

        standardSpeedReductionAmount = -30;
        judgementSpeedReductionAmount = -60;

        // Set up target and target string
        target = TargetType.SingleFront;
        targetJudgement = TargetType.SingleFront;
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
                StartCoroutine(MassiveImpact());
            }
            else
            {
                StartCoroutine(SwingWarhammer());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Decent dmg with def boost when attacking single targets
    private IEnumerator SwingWarhammer()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her warhammer", 1.5f, false);

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

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // check if target alive
            if (combatManagerReference.CheckTargetAlive(target))
            {
                // Apply spd drop to hit enemies
                combatManagerReference.ApplyModifierToEnemies(target, StatType.SPD, standardSpeedReductionAmount, 100.0f);

                // Change description for def drop
                combatManagerReference.DisplayCombatDescription("Enemy speed has dropped", 1.5f);
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
        StartCoroutine(MassiveImpact());
    }

    // Deals double damage against single targets and heals
    private IEnumerator MassiveImpact()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Jumping into the air, Gwenaelle bares down the full weight of the warhammer", 2.0f, false);

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("MassiveImpact", 0.4f);

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

            // check if target alive
            if (combatManagerReference.CheckTargetAlive(target))
            {
                // Apply spd drop to hit enemies
                combatManagerReference.ApplyModifierToEnemies(target, StatType.SPD, judgementSpeedReductionAmount, 100.0f);

                // Change description for def drop
                combatManagerReference.DisplayCombatDescription("Enemy speed has dropped", 1.5f);
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
