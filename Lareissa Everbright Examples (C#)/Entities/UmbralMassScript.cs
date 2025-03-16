using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbralMassScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    // ACTION STATS
    [Header("Grasping Arms settings")]
    public float graspingArmsDamageLower = 7.0f;
    public float graspingArmsDamageHigher = 10.0f;
    public float graspingArmsStatReductionChance = 20.0f;
    public float graspingArmsStatReductionAmount = -20.0f;
    public float graspingArmsAccuracy = 100.0f;
    public float graspingArmsWaitCost = 18;

    [Header("Macabre Ward settings")]
    public float macabreWardDefIncreaseValue = 50f;
    public float macabreWardWaitCost = 30f;
    public bool macabreWardUsedFlag = false;

    [Header("Dysphoria settings")]
    public float dysphoriaWaitCost = 40f;

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
            StartCoroutine(Dysphoria());
        }
        else
        {
            ExecuteStandardActions();
        }
    }

    private void HandleEndTurn()
    {
        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // By default, grasping arms, or macabre ward if under 50% hp
    private void ExecuteStandardActions()
    {
        // Check if health is less than half and macabre ward not used yet
        if (health <= maxHealth * 0.5f && macabreWardUsedFlag == false)
        {
            // Check if already buffed
            if (HasModifier(StatType.DEF))
            {
                if (GetModifier(StatType.DEF).modifierValue < 0.0f)
                {
                    // Not buffed so do macabre ward
                    StartCoroutine(MacabreWard());
                }
                else
                {
                    StartCoroutine(GraspingArms());
                }
            }
            else
            {
                StartCoroutine(MacabreWard());
            }
        }
        else
        {
            StartCoroutine(GraspingArms());
        }
    }

    // First action, standard damaging attack
    private IEnumerator GraspingArms()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Umbral Mass reaches with its grasping arms", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("GraspingHands");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(graspingArmsAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(graspingArmsDamageLower, graspingArmsDamageHigher);

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

            // See if stat drops
            if (TestAccuracy(graspingArmsStatReductionChance))
            {
                // Drop a random stat
                if (combatManagerReference.ApplyModifierToPlayer(GenerateRandomStatType(), graspingArmsStatReductionAmount))
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle is horrified!", 1.5f, false);
                    yield return new WaitForSeconds(0.1f);

                    // Wait until turn can proceed
                    while (combatManagerReference.CanTurnProceed() == false)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle resists being horrified!", 1.5f, false);
                    yield return new WaitForSeconds(0.1f);

                    // Wait until turn can proceed
                    while (combatManagerReference.CanTurnProceed() == false)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
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
        IncreaseWaitTime(graspingArmsWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, small damage with chance to decrease speed
    private IEnumerator MacabreWard()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Umbral Mass erects a macabre ward", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("MacabreWard");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Increase def
        AddModifier(StatType.DEF, macabreWardDefIncreaseValue);

        // Change description
        combatManagerReference.DisplayCombatDescription("It becomes harder to slay", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(macabreWardWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Judgement attack, eats equipment
    private IEnumerator Dysphoria()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Umbral Mass wallows in dysphoria", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("Dysphoria");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Add dysphoria augment
        AddAugment(AugmentType.DYSPHORIA);

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(dysphoriaWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
