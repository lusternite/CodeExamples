using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EverlustreScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardDmgReduction;
    public float judgementHealAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E16", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 200;
        damageLowerStandard = durability * 2;
        damageHigherStandard = durability * 2;
        waitCostNormal = 40;

        accuracyJudgement = 100;
        waitCostJudgement = 35;

        standardDmgReduction = -50;
        if (playerReference)
        {
            judgementHealAmount = playerReference.maxHealth;
        }
        else
        {
            judgementHealAmount = 100;
        }
        

        // Set up target and target string
        target = TargetType.All;
        targetJudgement = TargetType.Self;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Does damage and buffs self
    public override void UseEquipment()
    {
        // Stop button selection after press
        DisableButtonSelection();

        // Check to see if it's the player's turn and equipment isn't broken
        if (combatManagerReference.canPlayerAct == true && equipmentBrokenFlag == false)
        {
            // Check if should be judgement or normal
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                StartCoroutine(RestoringLight());
            }
            else
            {
                StartCoroutine(IncandescentSmite());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Decent dmg with def boost when attacking single targets
    private IEnumerator IncandescentSmite()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("The Everlustre radiates with glowing heat and bombards the area", 3.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("EverlustreSmite");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(accuracyNormal))
        {
            // It hits, tell combat manager to inflict damage
            combatManagerReference.InflictDamageEnemy(target, damageLowerStandard, damageHigherStandard, playerReference);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();
            
            // Apply dmg reduction to player
            if (combatManagerReference.ApplyModifierToPlayer(StatType.DMG, standardDmgReduction))
            {
                // Change description for dmg drop
                combatManagerReference.DisplayCombatDescription("Gwenaelle's strength is sapped!", 1.5f);
            }
            else
            {
                // Change description for dmg drop
                combatManagerReference.DisplayCombatDescription("Gwenaelle prevents her strength from being sapped!", 1.5f);
            }
        }
        else
        {
            print("It misses...");

            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait
        base.UseEquipment();

        // Update damage based on durability
        damageLowerStandard = durability * 2;
        damageHigherStandard = durability * 2;

        yield return null;
    }

    // Deals double damage against single targets and heals
    public override void UseJudgement()
    {
        StartCoroutine(RestoringLight());
    }

    // Deals double damage against single targets and heals
    private IEnumerator RestoringLight()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("The Everlustre shines with an ethereal blue light", 3.0f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("EverlustreRestoringLight");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Heal the player
        combatManagerReference.RestoreHealthPlayer(judgementHealAmount);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle is fully purified", 2.0f, false);

        // Remove all adjusters from the player
        playerReference.RemoveAllAdjusters();

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        // Update damage based on durability
        damageLowerStandard = durability * 2;
        damageHigherStandard = durability * 2;

        yield return null;
    }
}
