using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSkylarkScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Spectral Wings settings")]
    public float spectralWingsDamageLower = 9.0f;
    public float spectralWingsDamageHigher = 14.0f;
    public float spectralWingsHealingLower = 16.0f;
    public float spectralWingsHealingHigher = 20.0f;
    public float spectralWingsAccuracy = 84.0f;
    public float spectralWingsWaitCost = 45;

    [Header("Shadow Harvest settings")]
    public float shadowHarvestDamageLower = 5.0f;
    public float shadowHarvestDamageHigher = 8.0f;
    public float shadowHarvestAccuracy = 100.0f;
    public float shadowHarvestWaitCost = 50.0f;

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
            StartCoroutine(ShadowHarvest());
        }
        else
        {
            // Use Blaze Dive instead
            StartCoroutine(SpectralWings());
        }
    }

    private void HandleEndTurn()
    {

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // First action, standard damaging attack
    private IEnumerator SpectralWings()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Shadow Skylark sweeps with spectral wings", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("SpectralWings");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(spectralWingsAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(spectralWingsDamageLower, spectralWingsDamageHigher);

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

            // Change description
            combatManagerReference.DisplayCombatDescription("Shadow Skylark is cloaked in vitality", 1.5f, false);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Heal self
            // Calculate the healing amount
            float healing = Mathf.Floor(Random.Range(spectralWingsHealingLower, spectralWingsHealingHigher));

            // Check if there is 1 or 2 enemies remaining at the rear
            if (combatManagerReference.enemiesRear.Count == 1)
            {
                combatManagerReference.RestoreHealthEnemy(3, healing);
            }
            else
            {
                combatManagerReference.RestoreHealthEnemy(4, healing);
            }

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
        IncreaseWaitTime(spectralWingsWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, damages and high chance to stun
    private IEnumerator ShadowHarvest()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Shadow Skylark seeks to harvest", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("ShadowHarvest");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(shadowHarvestAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(shadowHarvestDamageLower, shadowHarvestDamageHigher);

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

            // Make sure previous equipment isnt broken
            if (combatManagerReference.previousUsedEquipment.equipmentBrokenFlag == false)
            {
                // Change description
                combatManagerReference.DisplayCombatDescription(combatManagerReference.previousUsedEquipment.equipmentName + " is banished!", 1.5f, false);

                // Apply the augment
                playerReference.AddAugment(AugmentType.BANISH);

                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                
            }
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(shadowHarvestWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
