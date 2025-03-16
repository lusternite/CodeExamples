using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSkylarkScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    // ACTION STATS
    [Header("Blaze Dive settings")]
    public float blazeDiveDamageLower = 5.0f;
    public float blazeDiveDamageHigher = 11.0f;
    public float blazeDiveBuffChance = 50.0f;
    public float blazeDiveSpdBuffAmount = 30.0f;
    public float blazeDiveDmgBuffAmount = 50.0f;
    public float blazeDiveAccuracy = 90.0f;
    public float blazeDiveWaitCost = 43;

    [Header("Fire Song settings")]
    public float fireSongBuffMultiplier = 2.0f;
    public float fireSongAccuracy = 100.0f;
    public float fireSongWaitCost = 22.0f;

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
            StartCoroutine(FireSong());
        }
        else
        {
            // Use Blaze Dive instead
            StartCoroutine(BlazeDive());
        }
    }

    private void HandleEndTurn()
    {

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // First action, standard damaging attack
    private IEnumerator BlazeDive()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Fire Skylark takes a blazing dive", 1.5f, false);

        // Play SFX
        audioManagerReference.PlayEntitySFX("BlazeDive");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(blazeDiveAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(blazeDiveDamageLower, blazeDiveDamageHigher);

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

            // Check if should self buff and also not already omega buffed
            if (TestAccuracy(blazeDiveBuffChance))
            {
                if (HasModifier(StatType.DMG))
                {
                    if (GetModifier(StatType.DMG).modifierValue > blazeDiveDmgBuffAmount)
                    {
                        // Has omega buff, just reset it
                        AddModifier(StatType.SPD, GetModifier(StatType.SPD).modifierValue);
                        AddModifier(StatType.DMG, GetModifier(StatType.DMG).modifierValue);
                    }
                    else
                    {
                        // Do the normal buff
                        AddModifier(StatType.SPD, blazeDiveSpdBuffAmount);
                        AddModifier(StatType.DMG, blazeDiveDmgBuffAmount);
                    }
                }
                else
                {
                    // Do the normal buff
                    AddModifier(StatType.SPD, blazeDiveSpdBuffAmount);
                    AddModifier(StatType.DMG, blazeDiveDmgBuffAmount);
                }

                // Change description
                combatManagerReference.DisplayCombatDescription("Fire Skylark is wreathed in flames");
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
        IncreaseWaitTime(blazeDiveWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, damages and high chance to stun
    private IEnumerator FireSong()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Fire Skylark sings a song of flames", 1.5f, false);

        // Play SFX
        audioManagerReference.PlayEntitySFX("FireSong");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Go through all allies and double their positive buffs

        // Front row
        for (int i = 0; i < combatManagerReference.enemiesFront.Count; i++)
        {
            combatManagerReference.enemiesFront[i].MultiplyAllPositiveModifiers(fireSongBuffMultiplier);
        }

        // Rear row
        for (int i = 0; i < combatManagerReference.enemiesRear.Count; i++)
        {
            combatManagerReference.enemiesRear[i].MultiplyAllPositiveModifiers(fireSongBuffMultiplier);
        }

        // Change combat description
        combatManagerReference.DisplayCombatDescription("All ally modifiers doubled!", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(fireSongWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
