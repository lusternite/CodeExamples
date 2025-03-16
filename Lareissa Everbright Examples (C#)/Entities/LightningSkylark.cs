using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSkylark : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Shock Talon settings")]
    public float shockTalonDamageLower = 5.0f;
    public float shockTalonDamageHigher = 14.0f;
    public float shockTalonAccuracy = 95.0f;
    public float shockTalonWaitCost = 31;

    [Header("Lightning Festival settings")]
    public float lightningFestivalDamageLower = 16.0f;
    public float lightningFestivalDamageHigher = 24.0f;
    public float lightningFestivalAccuracy = 100.0f;
    public float lightningFestivalWaitCost = 40.0f;

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
            StartCoroutine(LightningFestival());
        }
        else
        {
            // Use Blaze Dive instead
            StartCoroutine(ShockTalon());
        }
    }

    private void HandleEndTurn()
    {

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // First action, standard damaging attack
    private IEnumerator ShockTalon()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Lightning Skylark strikes with its shock talons", 1.5f, false);

        // Play SFX
        audioManagerReference.PlayEntitySFX("ShockTalon");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(shockTalonAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(shockTalonDamageLower, shockTalonDamageHigher);

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

            // Check if should steal buff
            if (playerReference.HasAnyPositiveModifiers())
            {
                // Get the first modifier
                ModifierScript stolenModifier = playerReference.GetFirstPositiveModifier();

                // Give it to all allies
                combatManagerReference.ApplyModifierToEnemies(TargetType.All, stolenModifier.modifierType, stolenModifier.modifierValue, 100.0f);

                // Then remove it from the player
                playerReference.RemoveModifier(stolenModifier.modifierType);

                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle's modifier was snatched!");
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
        IncreaseWaitTime(shockTalonWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action
    private IEnumerator LightningFestival()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Lightning Skylark unleashes a festival of sparks", 1.5f, false);

        // Play SFX
        audioManagerReference.PlayEntitySFX("LightningFestival");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(shockTalonAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(lightningFestivalDamageLower, lightningFestivalDamageHigher);

            // Check if allies have more modifiers than player
            if (playerReference.GetModifierCount() < combatManagerReference.GetModifierCountOfEnemies(TargetType.All))
            {
                // Change combat description
                combatManagerReference.DisplayCombatDescription("The skylark's confident state causes a brutal thunderstorm to appear!", 2.5f, false);
                yield return new WaitForSeconds(0.1f);

                damage *= 2.0f;

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

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
        IncreaseWaitTime(lightningFestivalWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
