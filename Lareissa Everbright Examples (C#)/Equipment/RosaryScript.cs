using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosaryScript : EquipmentBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Header("Weapon specific settings")]
    public float standardHealAmount;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        // Set up equipment information
        CSVReaderScript.ReadEquipmentData("E05", this);

        // Set the icon
        SetEquipmentIcon();

        // Initialise all of this weapon's stats
        accuracyNormal = 100;
        waitCostNormal = 30;

        accuracyJudgement = 100;
        waitCostJudgement = 21;

        standardHealAmount = 50;

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

    // Recover lots of health
    public override void UseEquipment()
    {
        // Stop button selection after press
        DisableButtonSelection();

        // Check to see if it's the player's turn
        if (combatManagerReference.canPlayerAct == true && equipmentBrokenFlag == false)
        {
            // Check if should be judgement or normal
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                StartCoroutine(Epiphany());
            }
            else
            {
                StartCoroutine(SolemnPrayer());
            }

            // Tell combat manager this was used
            combatManagerReference.previousUsedEquipment = this;

            combatManagerReference.NotifyPlayerActionTaken();
        }
    }

    // Recover lots of health
    private IEnumerator SolemnPrayer()
    {
        print("Gwenaelle solemnly prays");

        // Change description
        combatManagerReference.DisplayCombatDescription("Gwenaelle solemnly prays", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("SolemnPrayer");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Restore health
        combatManagerReference.RestoreHealthPlayer(CalculateHealing(standardHealAmount, standardHealAmount));

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait
        base.UseEquipment();

        yield return null;
    }

    // Grants unyielding state
    public override void UseJudgement()
    {
        StartCoroutine(Epiphany());
    }

    // Grants unyielding state
    private IEnumerator Epiphany()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Clutching the rosary, Gwenaelle receives unyielding strength from above", 2.0f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("Epiphany");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Give player the unyielding augment
        combatManagerReference.ApplyAugmentToPlayer(AugmentType.UNYIELDING);
        
        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Use parent function to decrease durability and increase wait, reset judgement
        base.UseJudgement();

        yield return null;
    }
}
