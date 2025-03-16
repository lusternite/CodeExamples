using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecurveBowScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardSpeedGainAmount;
    public float standardAccuracyGainAmount;
    public float judgementAccuracyGainAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E11", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        damageLowerStandard = 12;
        damageHigherStandard = 15;
        waitCostNormal = 25;

        accuracyJudgement = 100;
        waitCostJudgement = 10;

        standardSpeedGainAmount = 20;
        standardAccuracyGainAmount = 15;
        judgementAccuracyGainAmount = 50;

        // Set up target and target string
        target = TargetType.SingleRear;
        targetJudgement = TargetType.Self;
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
                StartCoroutine(UnparallelledPrecision());
            }
            else
            {
                StartCoroutine(ShootRecurveBow());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Damage rear
    private IEnumerator ShootRecurveBow()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle shoots her recurve bow", 1.5f, false);

        playerReference.PlayRangedAttackAnimationPlayer();

        // Play sfx
        audioManagerReference.PlayWeaponSFX("BowShoot", 0.6f);

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

            // Apply spd buff to self
            combatManagerReference.ApplyModifierToPlayer(StatType.SPD, standardSpeedGainAmount);

            // Apply acc buff to self
            combatManagerReference.ApplyModifierToPlayer(StatType.ACC, standardAccuracyGainAmount);

            // Change description for def drop
            combatManagerReference.DisplayCombatDescription("Gwenaelle's speed and accuracy has increased", 1.5f);
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
        StartCoroutine(UnparallelledPrecision());
    }

    // Grants acc buff and removes blind
    private IEnumerator UnparallelledPrecision()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle exhales and focuses her aim", 2.0f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("UnparallelledPrecision");

        // Reset achievement tracker
        FindObjectOfType<AchievementManagerScript>().ResetDebuffAvoided();

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Grant self acc buff
        combatManagerReference.ApplyModifierToPlayer(StatType.ACC, judgementAccuracyGainAmount);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        // Set judgement back to 50
        combatManagerReference.judgementMeter = 50.0f;

        yield return null;
    }
}
