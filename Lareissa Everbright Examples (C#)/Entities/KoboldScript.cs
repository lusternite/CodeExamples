using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoboldScript : EnemyBaseScript
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Shovel settings")]
    public float shovelDamageLower = 15.0f;
    public float shovelDamageHigher = 22.0f;
    public float shovelAccuracy = 72.0f;
    public float shovelWaitCost = 38;
    public float shovelUseChance = 52.0f;

    [Header("Torch settings")]
    public float torchDamageLower = 10.0f;
    public float torchDamageHigher = 16.0f;
    public float torchAccReductionChance = 20.0f;
    public float torchAccReductionValue = -10.0f;
    public float torchAccuracy = 88.0f;
    public float torchWaitCost = 45;
    public float torchUseChance = 17.0f;

    [Header("Bite settings")]
    public float biteDamageLower = 10.0f;
    public float biteDamageHigher = 13.0f;
    public float biteAccuracy = 92.0f;
    public float biteWaitCost = 21;

    [Header("Wild Flailing settings")]
    public float wildFlailingDamageLower = 20.0f;
    public float wildFlailingDamageHigher = 25.0f;
    public float wildFlailingSpdIncreaseValue = 20.0f;
    public float wildFlailingAccuracy = 100.0f;
    public float wildFlailingWaitCost = 32;

    // This flag is used to decide if this kobold primarily uses torch
    public bool torcherFlag = false;

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
            StartCoroutine(WildFlailing());
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

    // By default, choose between shovel, torch and bite
    // Only use torch if torcher flag is true
    private void ExecuteStandardActions()
    {
        // Check if this kobold is a torcher
        if (torcherFlag == true)
        {
            // Use torch
            StartCoroutine(Torch());
        }
        else
        {
            print("kobold standard actions");
            // Normal action use chances
            float randomAction = Random.Range(0, 100.0f);

            // 52% Shovel
            if (randomAction < shovelUseChance)
            {
                StartCoroutine(Shovel());
            }
            // 17% Torch
            else if (randomAction < shovelUseChance + torchUseChance)
            {
                StartCoroutine(Torch());
            }
            // 21% Bite
            else
            {
                StartCoroutine(Bite());
            }
        }
    }

    // First action, standard damaging attack
    private IEnumerator Shovel()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Kobold swings its shovel", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("BluntSwing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(shovelAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(shovelDamageLower, shovelDamageHigher);

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
        IncreaseWaitTime(shovelWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, damages with chance to decrease accuracy
    private IEnumerator Torch()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Kobold flails its torch", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("TorchSwing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(torchAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(torchDamageLower, torchDamageHigher);

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

            // Check if player's acc is dropped
            if (TestAccuracy(torchAccReductionChance))
            {
                // Apply acc reduction to player
                if (combatManagerReference.ApplyModifierToPlayer(StatType.ACC, torchAccReductionValue))
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle's accuracy is reduced!", 1.5f);
                }
                else
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her accuracy from being reduced!", 1.5f);
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
        IncreaseWaitTime(torchWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // First action, standard damaging attack
    private IEnumerator Bite()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Kobold goes for a bite", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("KoboldBite");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(biteAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(biteDamageLower, biteDamageHigher);

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
        IncreaseWaitTime(biteWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // First action, standard damaging attack
    private IEnumerator WildFlailing()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Kobold wildly swings its shovel and torch", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("WildFlailing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(wildFlailingAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(wildFlailingDamageLower, wildFlailingDamageHigher);

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

            // Increase speed
            AddModifier(StatType.SPD, wildFlailingSpdIncreaseValue);

            // Change description
            combatManagerReference.DisplayCombatDescription("Kobold is aggravated and moving faster!", 1.5f, false);
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

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(wildFlailingWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
