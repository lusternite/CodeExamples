using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AurochsScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private bool readyUsed = false;

    // ACTION STATS
    [Header("Ready settings")]
    public float readyDmgIncreaseValue = 100.0f;
    public float readyAccIncreaseValue = 50;
    public float readyWaitCost = 45;

    [Header("Charge settings")]
    public float chargeDamageLower = 17;
    public float chargeDamageHigher = 30;
    public float chargeAccuracy = 100;
    public float chargeWaitCost = 51;

    [Header("Gigaton Rush settings")]
    public float gigatonRushDamageLower = 20.0f;
    public float gigatonRushDamageHigher = 22.0f;
    public float gigatonRushSpdIncreaseValue = 50.0f;
    public float gigatonRushAccuracy = 100.0f;
    public float gigatonRushWaitCost = 66;

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
            StartCoroutine(GigatonRush());
            readyUsed = false;
        }
        else
        {
            // Different action use percentages depending on if damaged before this turn
            ExecuteStandardActions();
        }
    }

    private void HandleEndTurn()
    {

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // By default, use ready then use charge
    private void ExecuteStandardActions()
    {
        // Check if should use ready
        if (readyUsed == false)
        {
            StartCoroutine(Ready());
            readyUsed = true;
        }
        // Otherwise use charge
        else
        {
            StartCoroutine(Charge());
            readyUsed = false;
        }
    }

    // First action, buffs damage
    private IEnumerator Ready()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Aurochs readies itself to charge", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("AurochsReady");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Give self 100% dmg buff
        AddModifier(StatType.DMG, readyDmgIncreaseValue);

        // Give self 50% acc buff
        AddModifier(StatType.ACC, readyAccIncreaseValue);

        // Change combat description
        combatManagerReference.DisplayCombatDescription("Its damage and accuracy has increased!", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(readyWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, does loads of damage
    private IEnumerator Charge()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Aurochs charges!", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("PowerSwing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(chargeAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(chargeDamageLower, chargeDamageHigher);

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
        
        // Remove dmg buff
        if (HasModifier(StatType.DMG))
        {
            RemoveModifier(StatType.DMG);
        }

        // Increase wait cost
        IncreaseWaitTime(chargeWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, damages and raises party speed
    private IEnumerator GigatonRush()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Fuming with heat, Aurochs does a gigaton rush", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("GigatonRush");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(gigatonRushAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(gigatonRushDamageLower, gigatonRushDamageHigher);

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

            // Buff party speed
            combatManagerReference.ApplyModifierToEnemies(TargetType.All, StatType.SPD, gigatonRushSpdIncreaseValue, 100.0f);

            // Change description
            combatManagerReference.DisplayCombatDescription("All allies speed increased!", 1.5f, false);
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

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Remove dmg buff
        if (HasModifier(StatType.DMG))
        {
            RemoveModifier(StatType.DMG);
        }

        // Increase wait cost
        IncreaseWaitTime(gigatonRushWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
