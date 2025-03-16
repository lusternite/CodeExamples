using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Shoot settings")]
    public float shootDamageLower = 10.0f;
    public float shootDamageHigher = 13.0f;
    public float shootAccuracy = 92.0f;
    public float shootWaitCost = 41;

    [Header("Critical Shot settings")]
    public float criticalShotDamageLower = 26.0f;
    public float criticalShotDamageHigher = 33.0f;
    public float criticalShotAccuracy = 100.0f;
    public float criticalShotWaitCost = 61.0f;

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
            StartCoroutine(CriticalShot());
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

    // By default, use shoot, but if health is under 50% use unholy life
    private void ExecuteStandardActions()
    {
        // Just shoot
        StartCoroutine(Shoot());
    }

    // First action, standard damaging attack
    private IEnumerator Shoot()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Archer shoots with its bow", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("BowShoot");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(shootAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(shootDamageLower, shootDamageHigher);

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
        IncreaseWaitTime(shootWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    

    // Lash action, high damage
    private IEnumerator CriticalShot()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Archer launches a critical shot", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("CriticalShot");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(criticalShotAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(criticalShotDamageLower, criticalShotDamageHigher);

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

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(criticalShotWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
