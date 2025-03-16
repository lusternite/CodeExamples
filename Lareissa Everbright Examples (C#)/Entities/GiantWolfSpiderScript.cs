using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantWolfSpiderScript : EnemyBaseScript
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // Used to track whether was damaged since last turn
    private float healthLastRound;

    // ACTION STATS
    [Header("Blood Curdle settings")]
    public float bloodCurdleDamageLower = 7.0f;
    public float bloodCurdleDamageHigher = 9.0f;
    public float bloodCurdleBleedChance = 10.0f;
    public float bloodCurdleAccuracy = 82.0f;
    public float bloodCurdleWaitCost = 27;
    public float bloodCurdleUseChance = 75.0f;

    [Header("Shoot Web settings")]
    public float shootWebSpdReductionValue = -20.0f;
    public float shootWebAccuracy = 95.0f;
    public float shootWebWaitCost = 42;
    public float shootWebIncreasedUseChance = 20.0f;

    [Header("Shrill Howl settings")]
    public float shrillHowlDmgBuffValue = 50.0f;
    public float shrillHowlWaitCost = 32.0f;

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
            StartCoroutine(ShrillHowl());
        }
        else
        {
            // Different action use percentages depending on if damaged before this turn
            ExecuteStandardActions();
        }
    }

    private void HandleEndTurn()
    {
        // End turn by recording amount of health
        healthLastRound = health;

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // By default, choose between blood curdle and shoot web,
    // With higher chance for shoot web if damaged last turn
    private void ExecuteStandardActions()
    {
        // Check to see if gwen is slowed already
        if (playerReference.HasModifier(StatType.SPD))
        {
            if (playerReference.GetModifier(StatType.SPD).modifierValue < 0.0f)
            {
                // Already slowed so just blood curdle
                StartCoroutine(BloodCurdle());
            }
            else
            {
                CheckStandardActions();
            }
        }
        else
        {
            CheckStandardActions();
        }
    }

    private void CheckStandardActions()
    {
        // Check if has been damaged since last round
        if (healthLastRound > health)
        {
            // Higher web chance

            // 55% Blood Curdle
            if (Random.Range(0, 100.0f) < bloodCurdleUseChance - shootWebIncreasedUseChance)
            {
                StartCoroutine(BloodCurdle());
            }
            // 45% Shoot Web
            else
            {
                StartCoroutine(ShootWeb());
            }
        }
        else
        {
            // Normal Web chance

            // 75% Blood Curdle
            if (Random.Range(0, 100.0f) < bloodCurdleUseChance)
            {
                StartCoroutine(BloodCurdle());
            }
            // 25% Shoot Web
            else
            {
                StartCoroutine(ShootWeb());
            }
        }
    }

    // First action, standard damaging attack
    private IEnumerator BloodCurdle()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Giant Wolf Spider bares its Blood-Curdling fangs and lunges", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("BloodCurdle");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(bloodCurdleAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(bloodCurdleDamageLower, bloodCurdleDamageHigher);

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

            // Check if player bleeds
            if (TestAccuracy(bloodCurdleBleedChance))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle suffers a severe wound", 1.5f);

                yield return new WaitForSeconds(0.1f);

                // Apply bleed to enemy
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
        IncreaseWaitTime(bloodCurdleWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, speed lowering ability, higher use chance after taking damage
    private IEnumerator ShootWeb()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Giant Wolf Spider tries to trap Gwenaelle in web", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("WebShoot");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (90% ACC)
        if (TestAccuracy(shootWebAccuracy))
        {
            // It hits, apply modifier
            if (combatManagerReference.ApplyModifierToPlayer(StatType.SPD, shootWebSpdReductionValue))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle is caught, her speed drops", 1.5f);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle cannot be caught by the web", 1.5f);
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
        IncreaseWaitTime(shootWebWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, buffs all enemy dmg
    private IEnumerator ShrillHowl()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Giant Wolf Spider unleashes a Shrill Howl", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("ShrillHowl");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Apply damage boost to all enemies
        combatManagerReference.ApplyModifierToEnemies(TargetType.All, StatType.DMG, shrillHowlDmgBuffValue, 100.0f);

        // Change description
        combatManagerReference.DisplayCombatDescription("All enemies howl in unison, becoming more ferocious", 1.5f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(shrillHowlWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
