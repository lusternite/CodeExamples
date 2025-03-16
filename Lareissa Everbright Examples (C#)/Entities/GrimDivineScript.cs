using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimDivineScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Vesputa settings")]
    public float vesputaDamageLower = 8.0f;
    public float vesputaDamageHigher = 17.0f;
    public float vesputaDefReductionAmount = -50.0f;
    public float vesputaHealAmount = 30.0f;
    public float vesputaAccuracy = 180.0f;
    public float vesputaWaitCost = 26;

    [Header("Epurna settings")]
    public float epurnaWaitCost = 14.0f;
    public bool epurnaUsedFlag = false;

    [Header("Sacrima settings")]
    public float sacrimaDamageLower = 20.0f;
    public float sacrimaDamageHigher = 24.0f;
    public float sacrimaAccuracy = 100.0f;
    public float sacrimaWaitCost = 30;
    public bool sacrimaUsedFlag = false;

    [Header("Langra settings")]
    public float langraDamageLower = 9.0f;
    public float langraDamageHigher = 11.0f;
    public float langraAccuracy = 300.0f;
    public float langraWaitCost = 32;

    [Header("Ovati settings")]
    public float ovatiDmgIncreaseValue = 10000.0f;
    public float ovatiWaitCost = 50;

    [Header("Cylvani")]
    public float cylvaniHealAmount = 25.0f;
    public float cylvaniSpdIncreaseAmount = 50.0f;
    public float cylvaniWaitCost = 12.0f;
    public bool cylvaniUsed = false;

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
            // Check if should used Cylvani or Ovati
            if (health == 1 && cylvaniUsed == false)
            {
                StartCoroutine(Cylvani());
                cylvaniUsed = true;
            }
            else
            {
                // Check if ovati is already active
                if (HasModifier(StatType.DMG))
                {
                    if (GetModifier(StatType.DMG).modifierValue >= 1000)
                    {
                        StartCoroutine(Cylvani());
                        cylvaniUsed = true;
                    }
                    else
                    {
                        StartCoroutine(Ovati());
                    }
                }
                else
                {
                    StartCoroutine(Ovati());
                }
            }
        }
        else
        {
            // Do epurna if not done yet, otherwise do standard actions
            if (epurnaUsedFlag == false)
            {
                StartCoroutine(Epurna());
                epurnaUsedFlag = true;
            }
            else
            {
                ExecuteStandardActions();
            }
            
        }
    }

    private void HandleEndTurn()
    {
        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // Use langra if ovati buff is up, otherwise go for sacrima if not banished, then vesputa, then sacrima then vesputa.
    private void ExecuteStandardActions()
    {
        bool langraUsed = false;

        // Check if damage buffed from ovati
        if (HasModifier(StatType.DMG))
        {
            if (GetModifier(StatType.DMG).modifierValue >= 1000.0f)
            {
                // Use langra
                StartCoroutine(Langra());
                langraUsed = true;
                sacrimaUsedFlag = false;
            }
        }

        // Check between sacrima and vesputa
        if (langraUsed == false)
        {
            // Check if player doesnt have banish
            if (playerReference.HasAugment(AugmentType.BANISH) == false)
            {
                StartCoroutine(Sacrima());
                sacrimaUsedFlag = true;
            }
            
            // Check that player doesnt have def reduction 
            else if (playerReference.HasModifier(StatType.DEF) == false)
            {
                StartCoroutine(Vesputa());
                sacrimaUsedFlag = false;
            }

            // Check that def mod isnt negative
            else if (playerReference.GetModifier(StatType.DEF).modifierValue > 0.0f)
            {
                StartCoroutine(Vesputa());
                sacrimaUsedFlag = false;
            }

            // Check that sacrima wasnt just used
            else if (sacrimaUsedFlag == false)
            {
                StartCoroutine(Sacrima());
                sacrimaUsedFlag = true;
            }

            // Finally used vesputa in all other cases
            else
            {
                StartCoroutine(Vesputa());
                sacrimaUsedFlag = false;
            }
        }
    }

    // First action, damages and reduces def
    private IEnumerator Vesputa()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Grim Divine shapes her hand like a stinger and mutters 'Vesputa'", 2.0f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(vesputaAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(vesputaDamageLower, vesputaDamageHigher);

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
            
            // Check if gwen's def is already dropped
            if (playerReference.HasModifier(StatType.DEF))
            {
                if (playerReference.GetModifier(StatType.DEF).modifierValue == -50.0f)
                {
                    // Heal grim divine from the def drop

                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle's essence is drained from the wound", 1.0f, false);

                    yield return new WaitForSeconds(0.1f);

                    // Wait until turn can proceed
                    while (combatManagerReference.CanTurnProceed() == false)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }

                    // Heal
                    combatManagerReference.RestoreHealthEnemy(1, vesputaHealAmount);

                    yield return new WaitForSeconds(0.1f);

                    // Wait until turn can proceed
                    while (combatManagerReference.CanTurnProceed() == false)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    // Inflict def drop
                    if (combatManagerReference.ApplyModifierToPlayer(StatType.DEF, vesputaDefReductionAmount))
                    {
                        // Change description
                        combatManagerReference.DisplayCombatDescription("Gwenaelle's defences have dropped!", 1.0f, false);
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
                        combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her defences being dropped!", 1.5f, false);
                        yield return new WaitForSeconds(0.1f);

                        // Wait until turn can proceed
                        while (combatManagerReference.CanTurnProceed() == false)
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }
            }
            // Otherwise drop def
            else
            {
                // Inflict def drop
                if (combatManagerReference.ApplyModifierToPlayer(StatType.DEF, vesputaDefReductionAmount))
                {
                    // Change description
                    combatManagerReference.DisplayCombatDescription("Gwenaelle's defences have dropped!", 1.0f, false);
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
                    combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her defences being dropped!", 1.5f, false);
                    yield return new WaitForSeconds(0.1f);

                    // Wait until turn can proceed
                    while (combatManagerReference.CanTurnProceed() == false)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
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
        IncreaseWaitTime(vesputaWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, grants unyielding state
    private IEnumerator Epurna()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Grim Divine spreads her arms and utters 'Epurna'", 2.0f, false);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Change description
        combatManagerReference.DisplayCombatDescription("Her will is unyielding...", 2.0f, false);

        // Grant unyielding
        AddAugment(AugmentType.UNYIELDING);

        yield return new WaitForSeconds(0.1f);

        // Set its duration to 999
        GetAugment(AugmentType.UNYIELDING).augmentDuration = 999.0f;

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(epurnaWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Third action, banishes and reduces equipment durability
    private IEnumerator Sacrima()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Grim Divine closes her eyes and whispers 'Sacrima'", 2.0f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(sacrimaAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(sacrimaDamageLower, sacrimaDamageHigher);

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

            // Check if player is not banished
            if (playerReference.HasAugment(AugmentType.BANISH) == false)
            {
                combatManagerReference.ApplyAugmentToPlayer(AugmentType.BANISH);

                // Change description
                combatManagerReference.DisplayCombatDescription(combatManagerReference.previousUsedEquipment.equipmentName + " is banished!", 2.0f);
                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Otherwise reduce equipment durability
            else
            {
                combatManagerReference.previousUsedEquipment.durability -= 1;
                if (combatManagerReference.previousUsedEquipment.durability <= 0)
                {
                    combatManagerReference.previousUsedEquipment.HandleEquipmentBreaking();
                }

                // Change description
                combatManagerReference.DisplayCombatDescription(combatManagerReference.previousUsedEquipment.equipmentName + " is damaged!", 2.0f);
                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
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
        IncreaseWaitTime(sacrimaWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Fourth action, crazy damage
    private IEnumerator Langra()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Grim Divine raises an open palm and exhales 'Langra'", 2.0f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(langraAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(langraDamageLower, langraDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

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

        // Remove damage buff
        RemoveModifier(StatType.DMG);

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(langraWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, give 1000% damage buff
    private IEnumerator Ovati()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Grim Divine sighs and laments 'Ovati'", 2.0f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Change description
        combatManagerReference.DisplayCombatDescription("Her power is ovewhelming...", 2.0f, false);

        // Give omega buff
        AddModifier(StatType.DMG, ovatiDmgIncreaseValue);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(ovatiWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second Lash action, give speed buff and heal, used when ovati is already up, or when hp is 1
    private IEnumerator Cylvani()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Grim Divine claps and shouts 'Cylvani'", 2.0f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Heal
        combatManagerReference.RestoreHealthEnemy(1, cylvaniHealAmount);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Change description
        combatManagerReference.DisplayCombatDescription("Karmillion is getting serious...", 2.0f, false);

        // Give speed buff
        AddModifier(StatType.SPD, cylvaniSpdIncreaseAmount);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(cylvaniWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
