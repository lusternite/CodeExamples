using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoldierScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private bool unholyLifeUsed;

    // ACTION STATS
    [Header("Slash settings")]
    public float slashDamageLower = 6.0f;
    public float slashDamageHigher = 12.0f;
    public float slashAccuracy = 73.0f;
    public float slashWaitCost = 34;

    [Header("Unholy Life settings")]
    public float unholyLifeDefIncreaseValue = 50.0f;
    public float unholyLifeWaitCost = 52;

    [Header("Bone Rattle settings")]
    public float boneRattleDamageLower = 10.0f;
    public float boneRattleDamageHigher = 14.0f;
    public float boneRattleStunChance = 80.0f;
    public float boneRattleAccuracy = 100.0f;
    public float boneRattleWaitCost = 56.0f;

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
            StartCoroutine(BoneRattle());
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

    // By default, use slash, but if health is under 50% use unholy life
    private void ExecuteStandardActions()
    {
        // Check if should use unholy life
        if (health < maxHealth / 2 && unholyLifeUsed == false)
        {
            StartCoroutine(UnholyLife());
            unholyLifeUsed = true;
        }
        // Otherwise use slash
        else
        {
            StartCoroutine(Slash());
        }
    }

    // First action, standard damaging attack
    private IEnumerator Slash()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Soldier slashes with its sword", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("HeavySwordSwing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(slashAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(slashDamageLower, slashDamageHigher);

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
        IncreaseWaitTime(slashWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, grants 50% def
    private IEnumerator UnholyLife()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Soldier wraps itself with Unholy Life", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("UnholyAid");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Give self 50% def buff
        AddModifier(StatType.DEF, unholyLifeDefIncreaseValue);

        // Change combat description
        combatManagerReference.DisplayCombatDescription("Its defence has increased!", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(unholyLifeWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, damages and high chance to stun
    private IEnumerator BoneRattle()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Soldier launches a bone rattling strike", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("BoneRattle");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(boneRattleAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(boneRattleDamageLower, boneRattleDamageHigher);

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

            // Check if player is stunned
            if (TestAccuracy(boneRattleStunChance))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwenaelle cannot move from the blunt force!", 1.5f);

                yield return new WaitForSeconds(0.1f);

                // Apply stun to enemy
                combatManagerReference.ApplyAugmentToPlayer(AugmentType.STUN);
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

        // Increase wait cost
        IncreaseWaitTime(boneRattleWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
