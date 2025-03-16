using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronborneScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private bool cleaving = true;

    // ACTION STATS
    [Header("Cleave settings")]
    public float cleaveDamageLower = 22.0f;
    public float cleaveDamageHigher = 31.0f;
    public float cleaveAccuracy = 84.0f;
    public float cleaveWaitCost = 46;

    [Header("Bludgeon settings")]
    public float bludgeonDamageLower = 11.0f;
    public float bludgeonDamageHigher = 22.0f;
    public float bludgeonSpdReductionValue = -25.0f;
    public float bludgeonDmgReductionValue = -25.0f;
    public float bludgeonAccuracy = 92.0f;
    public float bludgeonWaitCost = 51;

    [Header("Mass of Metal settings")]
    public float massOfMetalDamageLower = 50;
    public float massOfMetalDamageHigher = 50;
    public float massOfMetalAccuracy = 100.0f;
    public float massOfMetalWaitCost = 66;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void HandleTurn()
    {
        base.HandleTurn();

        // Decide whether to act or lash

        if (combatManagerReference.revengeMeter == 100.0f)
        {
            StartCoroutine(MassOfMetal());
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

    // By default, rotate between cleave and bludgeon
    private void ExecuteStandardActions()
    {
        // Check which to use
        if (cleaving == true)
        {
            StartCoroutine(Cleave());
            cleaving = false;
        }
        else
        {
            StartCoroutine(Bludgeon());
            cleaving = true;
        }
    }

    // First action, standard damaging attack
    private IEnumerator Cleave()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Ironborne cleaves with its giant sword", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("IronborneCleave");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(cleaveAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(cleaveDamageLower, cleaveDamageHigher);

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
        IncreaseWaitTime(cleaveWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, small damage with chance to decrease speed
    private IEnumerator Bludgeon()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Ironborne slams with the side of its massive sword", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("IronborneBludgeon");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(bludgeonAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(bludgeonDamageLower, bludgeonDamageHigher);

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
            
            // Apply spd reduction to player
            combatManagerReference.ApplyModifierToPlayer(StatType.SPD, bludgeonSpdReductionValue);

            // Apply dmg reduction to player
            if (combatManagerReference.ApplyModifierToPlayer(StatType.DMG, bludgeonDmgReductionValue))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle's speed and damage is reduced!", 1.5f);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her speed and damage from being reduced!", 1.5f);
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
        IncreaseWaitTime(bludgeonWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Judgement attack, does damage and makes negative buffs positive
    private IEnumerator MassOfMetal()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Ironborne swings its mass of metal", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("IronborneMass");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(massOfMetalAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(massOfMetalDamageLower, massOfMetalDamageHigher);

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

            if (GetNegativeModifierCount() > 0)
            {
                // Flip negative buffs
                FlipModifiers(false);

                // Play buff sound
                audioManagerReference.PlayCombatSFX("ModifierBuff");

                // Change description
                combatManagerReference.DisplayCombatDescription("Ironborne's negative modifiers become positive!", 1.5f, false);
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
        IncreaseWaitTime(massOfMetalWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
