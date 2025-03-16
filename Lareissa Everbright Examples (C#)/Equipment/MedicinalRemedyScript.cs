using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicinalRemedyScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float healingLower;
    public float healingHigher;
    public float judgementDefBonus;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E06", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        waitCostNormal = 20;

        accuracyJudgement = 100;
        waitCostJudgement = 25;

        healingLower = 18;
        healingHigher = 22;
        judgementDefBonus = 60;

        // Set up target and target string
        target = TargetType.Self;
        targetJudgement = TargetType.Self;
        equipmentTarget = ConvertTargetTypeToString(target);
        equipmentJudgementTarget = ConvertTargetTypeToString(targetJudgement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Minor heal and cures bleed and venom
    public override void UseEquipment()
    {
        // Stop button selection after press
        DisableButtonSelection();

        // Check to see if it's the player's turn and isnt equipment isnt broken
        if (combatManagerReference.canPlayerAct == true && equipmentBrokenFlag == false)
        {
            // Check if should be judgement or normal
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                StartCoroutine(BattlefieldPlacebo());
            }
            else
            {
                StartCoroutine(TendWounds());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    public IEnumerator TendWounds()
    {
        print("Gwenaelle quickly tends to her wounds");
        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle quickly tends to her wounds", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("TendWounds");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Calculate the healing amount
        float healing = CalculateHealing(healingLower, healingHigher);

        // Tell combat manager to heal the player by the amount
        combatManagerReference.RestoreHealthPlayer(healing);

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        bool bleedRemovedFlag = false;

        // Check for bleed
        if (playerReference.HasAugment(AugmentType.BLEED))
        {
            // Remove venom
            combatManagerReference.RemoveAugmentFromPlayer(AugmentType.BLEED);

            // Change description
            combatManagerReference.DisplayCombatDescription("Bleed has been removed");
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            bleedRemovedFlag = true;
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Check for venom

        if (playerReference.HasAugment(AugmentType.VENOM))
        {
            // Remove venom
            combatManagerReference.RemoveAugmentFromPlayer(AugmentType.VENOM);

            // Change description
            combatManagerReference.DisplayCombatDescription("Venom has been removed");
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Check for medicinal remedy achievement
            if (bleedRemovedFlag == true)
            {
                FindObjectOfType<AchievementManagerScript>().UnlockAchievement("ACH_MEDICINAL_REMEDY");
            }
        }

        // Use parent function to decrease durability and increase wait
        base.UseEquipment();

        yield return null;
    }

    // Grants the player a def bonus
    public override void UseJudgement()
    {
        StartCoroutine(BattlefieldPlacebo());
    }

    // Grants the player a def bonus
    private IEnumerator BattlefieldPlacebo()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("The remedy boosts Gwenaelle's confidence, her defenses have improved", 2.0f);
        yield return new WaitForSeconds(0.1f);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("BattlefieldPlacebo");

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Tell combat manager to give player a def boost
        combatManagerReference.ApplyModifierToPlayer(StatType.DEF, judgementDefBonus);

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
