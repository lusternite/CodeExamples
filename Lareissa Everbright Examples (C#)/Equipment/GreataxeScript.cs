using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreataxeScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardHealingPerHitEnemy;
    public float judgementDmgBuffValue;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E14", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 82;
        damageLowerStandard = 26;
        damageHigherStandard = 31;
        waitCostNormal = 43;

        accuracyJudgement = 90;
        damageLowerJudgement = 42;
        damageHigherJudgement = 50;
        waitCostJudgement = 44;

        standardHealingPerHitEnemy = 10;
        judgementDmgBuffValue = 50;

        // Set up target and target string
        target = TargetType.RearLine;
        targetJudgement = TargetType.RearLine;
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
                StartCoroutine(WideSwing());
            }
            else
            {
                StartCoroutine(SwingGreataxe());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Decent dmg with def boost when attacking single targets
    private IEnumerator SwingGreataxe()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle swings her greataxe", 1.5f, false);

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

            // Change description for def drop
            combatManagerReference.DisplayCombatDescription("Gwenaelle is revitalised", 1.5f);

            // heal depending on number of enemies hit
            float healingAmount = combatManagerReference.previousHitEnemyCount * standardHealingPerHitEnemy;
            combatManagerReference.RestoreHealthPlayer(healingAmount);

            // Track greataxe achievement
            FindObjectOfType<AchievementManagerScript>().CountGreataxeHealing((int)healingAmount);
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
        StartCoroutine(WideSwing());
    }

    // Deals double damage against single targets and heals
    private IEnumerator WideSwing()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle grits her teeth and goes for a hefty swing", 2.0f, false);

        playerReference.PlayJudgementAttackAnimationPlayer();

        // Play SFX
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

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Change description
            combatManagerReference.DisplayCombatDescription("Gwenaelle's strength increases", 1.5f, false);

            // Apply dmg buff to player
            combatManagerReference.ApplyModifierToPlayer(StatType.DMG, judgementDmgBuffValue);

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

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
