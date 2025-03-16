using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeDemonScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // Used to track whether was damaged since last turn
    private float healthLastRound;

    // ACTION STATS
    [Header("Wicked Scythe settings")]
    public float wickedScytheDamageLower = 18.0f;
    public float wickedScytheDamageHigher = 28.0f;
    public float wickedScytheAccuracy = 78.0f;
    public float wickedScytheWaitCost = 38;
    public float wickedScytheUseChance = 80.0f;

    [Header("Shriek settings")]
    public float shriekDefReductionValue = -30.0f;
    public float shriekAccuracy = 90.0f;
    public float shriekWaitCost = 44;
    public float shriekIncreasedUseChance = 15.0f;

    [Header("Venomous Reap settings")]
    public float venomousReapDamageLower = 32.0f;
    public float venomousReapDamageHigher = 40.0f;
    public float venomousReapAccuracy = 100.0f;
    public float venomousReapWaitCost = 59.0f;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start () {
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
            StartCoroutine(VenomousReap());
        }
        else
        {
            // Only think about using Shriek if player is not already affected
            if (playerReference.HasModifier(StatType.DEF))
            {
                if (playerReference.GetModifier(StatType.DEF).modifierValue == -30.0f)
                {
                    StartCoroutine(WickedScythe());
                }
                else
                {
                    ExecuteStandardActions();
                }
            }
            
            // Different action use percentages depending on if damaged before this turn
            else
            {
                ExecuteStandardActions();
            }
        }
    }

    private void HandleEndTurn()
    {
        // End turn by recording amount of health
        healthLastRound = health;

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // By default, choose between wicked scythe and shriek,
    // With higher chance for shriek if damaged last turn
    private void ExecuteStandardActions()
    {
        // Check if has been damaged since last round
        if (healthLastRound > health)
        {
            // Higher shriek chance

            // 65% Wicked Scythe
            if (Random.Range(0, 100.0f) < wickedScytheUseChance - shriekIncreasedUseChance)
            {
                StartCoroutine(WickedScythe());
            }
            // 35% Shriek
            else
            {
                StartCoroutine(Shriek());
            }
        }
        else
        {
            // Normal shriek chance

            // 80% Wicked Scythe
            if (Random.Range(0, 100.0f) < wickedScytheUseChance)
            {
                StartCoroutine(WickedScythe());
            }
            // 20% Shriek
            else
            {
                StartCoroutine(Shriek());
            }
        }
    }

    // First action, standard damaging attack
    private IEnumerator WickedScythe()
    {
        print("Centipede Demon uses Wicked Scythe");

        // Change description
        combatManagerReference.DisplayCombatDescription("Centipede Demon swings its Wicked Scythe", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("WickedScythe");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(wickedScytheAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(wickedScytheDamageLower, wickedScytheDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

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
        IncreaseWaitTime(wickedScytheWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, defence lowering ability, higher use chance after taking damage
    private IEnumerator Shriek()
    {
        print("Centipede Demon uses Shriek");

        // Change description
        combatManagerReference.DisplayCombatDescription("Centipede Demon unleashes a piercing Shriek", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("CentipedeShriek");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (90% ACC)
        if (TestAccuracy(shriekAccuracy))
        {
            // It hits, apply modifier
            if (combatManagerReference.ApplyModifierToPlayer(StatType.DEF, shriekDefReductionValue))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle is unnerved, her defence drops!", 1.5f);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle cannot be unnerved!", 1.5f);
            }

            print("Gwenaelle is unnerved, DEF " + shriekDefReductionValue);

            
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
        IncreaseWaitTime(shriekWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, damages and poisons
    private IEnumerator VenomousReap()
    {
        print("Centipede Demon unleashes Venomous Reap");

        // Change description
        combatManagerReference.DisplayCombatDescription("Centipede Demon unleashes Venomous Reap", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("VenomousReap");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (100% ACC)
        if (TestAccuracy(venomousReapAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(venomousReapDamageLower, venomousReapDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Tell combat manager to apply venom to player
            combatManagerReference.ApplyAugmentToPlayer(AugmentType.VENOM);
            print("Gwenaelle is afflicted with venom");
            combatManagerReference.DisplayCombatDescription("Gwenaelle is afflicted with venom", 1.5f);
        }
        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(venomousReapWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
