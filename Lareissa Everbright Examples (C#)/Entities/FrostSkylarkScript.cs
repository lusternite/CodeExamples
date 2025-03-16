using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostSkylarkScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Icy Winds settings")]
    public float icyWindsDamageLower = 13.0f;
    public float icyWindsDamageHigher = 17.0f;
    public float icyWindsSlowChance = 60.0f;
    public float icyWindsSlowAmount = -30.0f;
    public float icyWindsAccuracy = 86.0f;
    public float icyWindsWaitCost = 46;

    [Header("Frost Dance settings")]
    public float frostDanceDmgReduction = -50.0f;
    public float frostDanceAccuracy = 100.0f;
    public float frostDanceWaitCost = 37.0f;

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
            StartCoroutine(FrostDance());
        }
        else
        {
            // Use Blaze Dive instead
            StartCoroutine(IcyWinds());
        }
    }

    private void HandleEndTurn()
    {

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // First action, standard damaging attack
    private IEnumerator IcyWinds()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Frost Skylark causes icy winds", 1.5f, false);

        // Play SFX
        audioManagerReference.PlayEntitySFX("IcyWind");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(icyWindsAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(icyWindsDamageLower, icyWindsDamageHigher);

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

            // Check if should slow
            if (TestAccuracy(icyWindsSlowChance))
            {
                playerReference.AddModifier(StatType.SPD, icyWindsSlowAmount);

                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle is slowed from the cold!");
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
        IncreaseWaitTime(icyWindsWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, damages and high chance to stun
    private IEnumerator FrostDance()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Frost Skylark performs a dance of ice", 1.5f, false);

        // Play SFX
        audioManagerReference.PlayEntitySFX("FrostDance");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Decrease player damage

        playerReference.AddModifier(StatType.DMG, frostDanceDmgReduction);

        // Change combat description
        combatManagerReference.DisplayCombatDescription("Gwenaelle's damage is reduced!", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(frostDanceWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
