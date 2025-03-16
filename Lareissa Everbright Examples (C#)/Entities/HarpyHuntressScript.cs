using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpyHuntressScript : EnemyBaseScript
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Snipe settings")]
    public float snipeDamageLower = 9.0f;
    public float snipeDamageHigher = 16.0f;
    public float snipeAccuracy = 84.0f;
    public float snipeWaitCost = 34;
    public float snipeUseChance = 62.0f;

    [Header("Puncture settings")]
    public float punctureDamageLower = 6.0f;
    public float punctureDamageHigher = 10.0f;
    public float punctureBleedChance = 75.0f;
    public float punctureAccuracy = 67.0f;
    public float punctureWaitCost = 32;

    [Header("Pinning Shot settings")]
    public float pinningShotDamageLower = 14.0f;
    public float pinningShotDamageHigher = 20.0f;
    public float pinningShotSpdReductionAmount = -40.0f;
    public float pinningShotAccuracy = 100.0f;
    public float pinningShotWaitCost = 47;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void HandleTurn()
    {
        base.HandleTurn();

        // Decide whether to act or lash

        if (combatManagerReference.revengeMeter == 100.0f)
        {
            StartCoroutine(PinningShot());
        }
        else
        {
            ExecuteStandardActions();
        }
    }

    private void HandleEndTurn()
    {
        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // By default, choose between snipe and puncture
    private void ExecuteStandardActions()
    {
        // Check if the player is already bleeding
        if (playerReference.HasAugment(AugmentType.BLEED))
        {
            // Use swipe
            StartCoroutine(Snipe());
        }
        else
        {
            // 62% Swipe
            if (Random.Range(0, 100.0f) < snipeUseChance)
            {
                StartCoroutine(Snipe());
            }
            // 38% Feather Throw
            else
            {
                StartCoroutine(Puncture());
            }
        }
    }

    // First action, standard damaging attack
    private IEnumerator Snipe()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Harpy Huntress shoots with her bow", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("BowShoot");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(snipeAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(snipeDamageLower, snipeDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

            yield return new WaitForSeconds(0.1f);

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

            // Change description
            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(snipeWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, decent damage with chance to bleed
    private IEnumerator Puncture()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Harpy Huntress fires a puncturing shot", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("Puncture");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(punctureAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(punctureDamageLower, punctureDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Check if player's spd is dropped
            if (TestAccuracy(punctureBleedChance))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle starts bleeding", 1.5f);

                yield return new WaitForSeconds(0.1f);

                // Apply bleed to player
                combatManagerReference.ApplyAugmentToPlayer(AugmentType.BLEED);
            }
        }
        else
        {
            print("It misses...");

            // Change description
            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(punctureWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Judgement attack, does damage and slows
    private IEnumerator PinningShot()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Harpy Huntress shoots a pinning shot", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("PinningShot");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(pinningShotAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(pinningShotDamageLower, pinningShotDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();
            
            // Apply speed reduction to player
            if (combatManagerReference.ApplyModifierToPlayer(StatType.SPD, pinningShotSpdReductionAmount))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle's speed is reduced!", 1.5f);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her speed from being reduced!", 1.5f);
            }

            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            print("It misses...");

            // Change description
            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(pinningShotWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
