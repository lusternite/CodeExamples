using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float judgementStunChance;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E04", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        waitCostNormal = 33;

        accuracyJudgement = 100;
        damageLowerJudgement = 10;
        damageHigherJudgement = 18;
        waitCostJudgement = 45;

        judgementStunChance = 35;

        // Set up target and target string
        target = TargetType.Self;
        targetJudgement = TargetType.SingleFront;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Grants block augment
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
                StartCoroutine(HeavySlam());
            }
            else
            {
                StartCoroutine(ReadyShield());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    private IEnumerator ReadyShield()
    {
        print("Gwenaelle readies her shield");

        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle readies her shield", 2.0f);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("ShieldReady");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Add block augment to list
        combatManagerReference.ApplyAugmentToPlayer(AugmentType.BLOCK);

        // Use parent function to decrease durability and increase wait
        base.UseEquipment();

        yield return null;
    }

    public override void UseJudgement()
    {
        StartCoroutine(HeavySlam());
    }

    // Deals some damage, has a stun chance, and gain block
    private IEnumerator HeavySlam()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Like a battering ram, Gwenaelle lunges shield first", 2.0f, false);

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
            combatManagerReference.InflictDamageEnemy(targetJudgement, damageLowerJudgement, damageHigherJudgement, playerReference);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Check if stuns
            if (TestAccuracy(judgementStunChance) && combatManagerReference.CheckTargetAlive(targetJudgement))
            {
                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                // Change description
                combatManagerReference.DisplayCombatDescription("The " + combatManagerReference.GetTargetName(targetJudgement) + " is stunned!", 2.0f);

                combatManagerReference.ApplyAugmentToEnemies(targetJudgement, AugmentType.STUN, 100.0f);

                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Change description
            combatManagerReference.DisplayCombatDescription("Gwenaelle readies her shield after striking", 2.0f);

            // Play sfx
            audioManagerReference.PlayWeaponSFX("ShieldReady");

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Gain block
            combatManagerReference.ApplyAugmentToPlayer(AugmentType.BLOCK);
        }

        // If miss
        else
        {
            print("It misses...");

            combatManagerReference.DisplayCombatDescription("It misses...");

            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
