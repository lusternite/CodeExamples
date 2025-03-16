using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpyScript : EnemyBaseScript
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Swipe settings")]
    public float swipeDamageLower = 5.0f;
    public float swipeDamageHigher = 13.0f;
    public float swipeAccuracy = 90.0f;
    public float swipeWaitCost = 26;
    public float swipeUseChance = 62.0f;

    [Header("Feather Throw settings")]
    public float featherThrowDamageLower = 2.0f;
    public float featherThrowDamageHigher = 4.0f;
    public float featherThrowSpdReductionChance = 50.0f;
    public float featherThrowSpdReductionValue = -25.0f;
    public float featherThrowAccuracy = 95.0f;
    public float featherThrowWaitCost = 30;

    [Header("Rake settings")]
    public float rakeDamageLower = 10.0f;
    public float rakeDamageHigher = 19.0f;
    public float rakeBleedChance = 30.0f;
    public float rakeAccuracy = 100.0f;
    public float rakeWaitCost = 41;

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
            StartCoroutine(Rake());
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

    // By default, choose between swipe and feather throw
    private void ExecuteStandardActions()
    {
        // Check if the player is already slowed
        if (playerReference.HasModifier(StatType.SPD))
        {
            if (playerReference.GetModifier(StatType.SPD).modifierValue < 0.0f)
            {
                // Use swipe
                StartCoroutine(Swipe());
            }
            else
            {
                // 62% Swipe
                if (Random.Range(0, 100.0f) < swipeUseChance)
                {
                    StartCoroutine(Swipe());
                }
                // 38% Feather Throw
                else
                {
                    StartCoroutine(FeatherThrow());
                }
            }
        }
        else
        {
            // 62% Swipe
            if (Random.Range(0, 100.0f) < swipeUseChance)
            {
                StartCoroutine(Swipe());
            }
            // 38% Feather Throw
            else
            {
                StartCoroutine(FeatherThrow());
            }
        }
    }

    // First action, standard damaging attack
    private IEnumerator Swipe()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Harpy swipes with its talons", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("PowerSwing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(swipeAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(swipeDamageLower, swipeDamageHigher);

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
        IncreaseWaitTime(swipeWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, small damage with chance to decrease speed
    private IEnumerator FeatherThrow()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Harpy throws several sharp feathers", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("FeatherThrow");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(featherThrowAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(featherThrowDamageLower, featherThrowDamageHigher);

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
            if (TestAccuracy(featherThrowSpdReductionChance))
            {
                // Apply acc reduction to player
                if (combatManagerReference.ApplyModifierToPlayer(StatType.SPD, featherThrowSpdReductionValue))
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
        IncreaseWaitTime(featherThrowWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Judgement attack, does damage and might bleed
    private IEnumerator Rake()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Harpy viciously dives and rakes", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("Rake");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(rakeAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(rakeDamageLower, rakeDamageHigher);

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

            // Check if bleed is applied
            if (TestAccuracy(rakeBleedChance))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle begins to bleed", 1.5f);

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
        IncreaseWaitTime(rakeWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
