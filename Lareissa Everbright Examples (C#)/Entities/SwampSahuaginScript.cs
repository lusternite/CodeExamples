using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampSahuaginScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Spear Thrust settings")]
    public float spearThrustDamageLower = 11.0f;
    public float spearThrustDamageHigher = 17.0f;
    public float spearThrustBleedChance = 12f;
    public float spearThrustAccuracy = 76.0f;
    public float spearThrustWaitCost = 33;
    public float spearThrustUseChance = 67.0f;

    [Header("Spear Toss settings")]
    public float spearTossDamageLower = 15.0f;
    public float spearTossDamageHigher = 18.0f;
    public float spearTossVenomChance = 12f;
    public float spearTossAccuracy = 82.0f;
    public float spearTossWaitCost = 41;
    public float spearTossUseChance = 67.0f;

    [Header("Mud Spit settings")]
    public float mudSpitDamageLower = 6.0f;
    public float mudSpitDamageHigher = 9.0f;
    public float mudSpitDmgReductionChance = 20f;
    public float mudSpitDmgReductionAmount = -25f;
    public float mudSpitAccuracy = 72.0f;
    public float mudSpitWaitCost = 45;

    [Header("Frenzy settings")]
    public float frenzyDamageLower = 15.0f;
    public float frenzyDamageHigher = 20.0f;
    public float frenzyDmgIncreaseValue = 100.0f;
    public float frenzySpdIncreaseValue = 50.0f;
    public float frenzyWaitCost = 40;

    // This flag is used to decide if this sahuagin mainly throws spear
    public bool rearFlag = false;

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
            StartCoroutine(Frenzy());
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

    // By default, choose between a spear attack and mud spit depending on rear flag
    // Only use spearToss if spearTosser flag is true
    private void ExecuteStandardActions()
    {
        float randomAction = Random.Range(0, 100.0f);

        // Check if this sahuagin is a Spear Tosser
        if (rearFlag == true)
        {
            // Check if player has reduced damage
            if (playerReference.HasModifier(StatType.DMG))
            {
                if (playerReference.GetModifier(StatType.DMG).modifierValue < 0)
                {
                    // Already debuffed, only use spear
                    StartCoroutine(SpearToss());
                }
                else
                {
                    if (randomAction < spearTossUseChance)
                    {
                        StartCoroutine(SpearToss());
                    }
                    else
                    {
                        StartCoroutine(MudSpit());
                    }
                }
            }
            else
            {
                if (randomAction < spearTossUseChance)
                {
                    StartCoroutine(SpearToss());
                }
                else
                {
                    StartCoroutine(MudSpit());
                }
            }
        }
        else
        {
            // Check if player has reduced damage
            if (playerReference.HasModifier(StatType.DMG))
            {
                if (playerReference.GetModifier(StatType.DMG).modifierValue < 0)
                {
                    // Already debuffed, only use spear
                    StartCoroutine(SpearThrust());
                }
                else
                {
                    if (randomAction < spearTossUseChance)
                    {
                        StartCoroutine(SpearThrust());
                    }
                    else
                    {
                        StartCoroutine(MudSpit());
                    }
                }
            }
            else
            {
                if (randomAction < spearTossUseChance)
                {
                    StartCoroutine(SpearThrust());
                }
                else
                {
                    StartCoroutine(MudSpit());
                }
            }
        }
    }

    // First action, standard damaging attack
    private IEnumerator SpearThrust()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Sahuagin thrusts its spear", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("SpearThrust");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(spearThrustAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(spearThrustDamageLower, spearThrustDamageHigher);

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

            // Check if player is bled
            if (TestAccuracy(spearThrustBleedChance))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle starts bleeding!", 1.5f);

                yield return new WaitForSeconds(0.1f);

                // Apply blind to player
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
        IncreaseWaitTime(spearThrustWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, damages with chance to venom
    private IEnumerator SpearToss()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Sahuagin tosses its spear", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("SahuaginSpearToss");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(spearTossAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(spearTossDamageLower, spearTossDamageHigher);

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

            // Check if player is venomed
            if (TestAccuracy(spearTossVenomChance))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle is venomed!", 1.5f);

                yield return new WaitForSeconds(0.1f);

                // Apply venom to player
                combatManagerReference.ApplyAugmentToPlayer(AugmentType.VENOM);
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
        IncreaseWaitTime(spearTossWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // First action, standard damaging attack
    private IEnumerator MudSpit()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Sahuagin spits mud", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("MudSpit");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(mudSpitAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(mudSpitDamageLower, mudSpitDamageHigher);

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

            // Check if player is debuffed
            if (TestAccuracy(mudSpitDmgReductionChance))
            {
                // Apply dmg debuff to player
                if (combatManagerReference.ApplyModifierToPlayer(StatType.DMG, mudSpitDmgReductionAmount))
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle's damage is reduced!", 1.5f);
                }
                else
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her damage from being reduced!", 1.5f);
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
        IncreaseWaitTime(mudSpitWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, hurts self for damage
    private IEnumerator Frenzy()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Sahuagin goes into a frenzy", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("Frenzy");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Change description
        combatManagerReference.DisplayCombatDescription("It hurts itself in its rage", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // calculate damage
        float damage = CalculateDamage(frenzyDamageLower, frenzyDamageHigher);

        // Tell combat manager to inflict damage to self
        combatManagerReference.InflictDamageEnemySpecified(this, damage);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase speed
        AddModifier(StatType.SPD, frenzySpdIncreaseValue);

        // Increase damage
        AddModifier(StatType.DMG, frenzyDmgIncreaseValue);

        // Change description
        combatManagerReference.DisplayCombatDescription("Sahuagin is stronger and faster!", 1.5f);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(frenzyWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
