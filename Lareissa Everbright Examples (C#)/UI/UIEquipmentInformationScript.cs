using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEquipmentInformationScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public bool isInformationDisplaying;

    private EquipmentBaseScript equipmentReference;
    private PlayerBehaviourScript playerReference;

    private UITextUnfoldScript descriptionTextReference;
    private UITextUnfoldScript statsTextReference;
    private UITextUnfoldScript durabilityTextReference;
    private UITextUnfoldScript targetTextReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        equipmentReference = gameObject.GetComponent<EquipmentBaseScript>();
        playerReference = FindObjectOfType<PlayerBehaviourScript>();
        descriptionTextReference = GameObject.Find("EquipmentDescription").GetComponent<UITextUnfoldScript>();
        statsTextReference = GameObject.Find("EquipmentStats").GetComponent<UITextUnfoldScript>();
        durabilityTextReference = GameObject.Find("EquipmentDurability").GetComponent<UITextUnfoldScript>();
        targetTextReference = GameObject.Find("EquipmentTarget").GetComponent<UITextUnfoldScript>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // What happens when mouse hovers over the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check if judgement is on
        if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
        {
            // Set all the text information
            descriptionTextReference.SetText(equipmentReference.equipmentJudgementName);
            statsTextReference.SetText(equipmentReference.equipmentJudgementStats);
            durabilityTextReference.SetText("Durability: " + equipmentReference.durability.ToString());
            targetTextReference.SetText("Target: " + equipmentReference.equipmentJudgementTarget);
        }
        else
        {
            // Set all the text information
            descriptionTextReference.SetText(equipmentReference.equipmentDescription);
            statsTextReference.SetText(equipmentReference.equipmentStats);
            durabilityTextReference.SetText("Durability: " + equipmentReference.durability.ToString());
            targetTextReference.SetText("Target: " + equipmentReference.equipmentTarget);
        }

        // Adjust stats for mods if in combat
        if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eCombat)
        {
            AdjustEquipmentStatsWithModifiers();

            // And also turn on durability animation
            if (equipmentReference.durability <= 3)
            {
                GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().PlayLowDurabilityBounceAnimation();
            }
            else
            {
                GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().PlayNormalBounceAnimation();
            }
        }

        // Show the text
        descriptionTextReference.ShowText();
        statsTextReference.ShowText();
        durabilityTextReference.ShowText();
        targetTextReference.ShowText();

        isInformationDisplaying = true;

        // Show enemy targets with animation if in battle
        if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eCombat && FindObjectOfType<CombatManagerScript>().canPlayerAct == true)
        {
            // Make sure to first get rid of existing animations in case of oof
            FindObjectOfType<CombatManagerScript>().SetEnemyTargetedAnimations(false, TargetType.All);

            Invoke("SetEnemyTargetedAnimationsOn", 0.1f);
        }

        // Play sfx
        FindObjectOfType<AudioManagerScript>().PlayUISFX("EquipmentButtonHover");

        print("Hovering Equipment Button: " + equipmentReference.equipmentName);
    }

    // Used in case durability information needs to be updated
    public void UpdateDurabilityInformation()
    {
        Invoke("ChangeDurabilityInformation", 0.1f);
    }

    // Used to reset the information if needed
    public void ResetEquipmentInformationUI()
    {
        // Deactivate the text
        descriptionTextReference.HideText();
        statsTextReference.HideText();
        durabilityTextReference.HideText();
        targetTextReference.HideText();

        // Show the text
        descriptionTextReference.ShowText();
        statsTextReference.ShowText();
        durabilityTextReference.ShowText();
        targetTextReference.ShowText();
    }

    // NOTE TO SELF: ^ means positive adjustment so orange, * means negative adjustment so blue, # is colour finished
    private void AdjustEquipmentStatsWithModifiers()
    {
        // Check if mods exist and this action is affected by mods
        if (playerReference.HasAnyModifiers() == true || playerReference.HasAugment(AugmentType.BLIND) == true)
        {
            // Check if this action has damage and accuracy or just wait
            if (statsTextReference.actualText[0] != 'W')
            {
                // Used for each mod consideration
                string newStats = "";

                // Used to determine the final string
                string finalStats = "";

                // First check for accuracy mods
                if (playerReference.HasModifier(StatType.ACC) == true || playerReference.HasAugment(AugmentType.BLIND) == true)
                {
                    //Create new accuracy value as string

                    float newAccuracy;
                    float normalAccuracy;

                    // Check if judgement or not
                    if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
                    {
                        newAccuracy = equipmentReference.accuracyJudgement;
                        normalAccuracy = equipmentReference.accuracyJudgement;
                    }
                    else
                    {
                        newAccuracy = equipmentReference.accuracyNormal;
                        normalAccuracy = equipmentReference.accuracyNormal;
                    }

                    // Check if has acc modifier
                    if (playerReference.HasModifier(StatType.ACC) == true)
                    {
                        newAccuracy += playerReference.GetModifier(StatType.ACC).modifierValue;
                    }

                    // Check if blinded
                    if (playerReference.HasAugment(AugmentType.BLIND) == true)
                    {
                        // Reduce the amount by 120
                        newAccuracy -= 120.0f;
                    }

                    // Clamp to min 0
                    newAccuracy = Mathf.Clamp(newAccuracy, 0.0f, 900.0f);

                    // Make the string
                    newStats += newAccuracy.ToString() + "% ACC";

                    // Check if end result is positive or negative
                    if (newAccuracy > normalAccuracy)
                    {
                        // Positive so make the text green
                        newStats = newStats.Insert(0, "^");
                    }
                    else
                    {
                        // Negative so make the text red
                        newStats = newStats.Insert(0, "*");
                    }

                    // Make sure to add the colour end symbol
                    newStats += "# | ";
                }

                // No accuracy mods so just make it normal
                else
                {
                    newStats += statsTextReference.actualText.Substring(0, statsTextReference.actualText.IndexOf("|", 0) + 2);
                }

                // Make sure to update the final string
                finalStats += newStats;

                // Then check for DMG mods
                if (playerReference.HasModifier(StatType.DMG) == true)
                {
                    // Figure out the actual damage value from mods and also enemy def mods
                    float actualDamageLower;
                    float actualDamageHigher;
                    float normalDamageLower;
                    float normalDamageHigher;

                    // First check if this damage is from judgement or not
                    if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
                    {
                        actualDamageLower = equipmentReference.damageLowerJudgement;
                        actualDamageHigher = equipmentReference.damageHigherJudgement;
                        normalDamageLower = equipmentReference.damageLowerJudgement;
                        normalDamageHigher = equipmentReference.damageHigherJudgement;
                    }
                    else
                    {
                        actualDamageLower = equipmentReference.damageLowerStandard;
                        actualDamageHigher = equipmentReference.damageHigherStandard;
                        normalDamageLower = equipmentReference.damageLowerStandard;
                        normalDamageHigher = equipmentReference.damageHigherStandard;
                    }

                    // Then modify it by the dmg mod
                    actualDamageLower *= 1 + playerReference.GetModifier(StatType.DMG).modifierValue / 100.0f;
                    actualDamageHigher *= 1 + playerReference.GetModifier(StatType.DMG).modifierValue / 100.0f;

                    // After that, check if the first enemy hit has def mods
                    EnemyBaseScript firstEnemy;
                    // Check if judgement or not
                    if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
                    {
                        firstEnemy = FindObjectOfType<CombatManagerScript>().GetTargetedEnemies(equipmentReference.targetJudgement)[0];
                    }
                    else
                    {
                        firstEnemy = FindObjectOfType<CombatManagerScript>().GetTargetedEnemies(equipmentReference.target)[0];
                    }
                    if (firstEnemy.HasModifier(StatType.DEF) == true)
                    {
                        // Multiply by the def value
                        actualDamageLower = actualDamageLower * (100.0f - firstEnemy.GetModifier(StatType.DEF).modifierValue) / 100.0f;
                        actualDamageHigher = actualDamageHigher * (100.0f - firstEnemy.GetModifier(StatType.DEF).modifierValue) / 100.0f;
                    }

                    // Finally round this number
                    actualDamageLower = Mathf.Round(actualDamageLower);
                    actualDamageHigher = Mathf.Round(actualDamageHigher);

                    // Create new dmg value as string
                    newStats = actualDamageLower.ToString() + "-" + actualDamageHigher.ToString() + " DMG";

                    // Check if final result is positive or negative
                    if (actualDamageLower > normalDamageLower)
                    {
                        // Positive so make the text green
                        newStats = newStats.Insert(0, "^");
                    }
                    else
                    {
                        // Negative so make the text red
                        newStats = newStats.Insert(0, "*");
                    }

                    // Make sure to add the colour end symbol
                    newStats += "# | ";
                }

                // No DMG mods so just make it normal
                else
                {
                    newStats = statsTextReference.actualText.Substring(statsTextReference.actualText.IndexOf("|") + 2, statsTextReference.actualText.IndexOf("|", statsTextReference.actualText.IndexOf("|") + 1) - statsTextReference.actualText.IndexOf("|"));
                }

                // Make sure to update the final string
                finalStats += newStats;

                // Finally, check for speed mods
                if (playerReference.HasModifier(StatType.SPD) == true)
                {
                    // Create new speed value as string

                    // Check if judgement or not
                    if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
                    {
                        newStats = "WT+ " + Mathf.Round(equipmentReference.waitCostJudgement * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f)).ToString();
                    }
                    else
                    {
                        newStats = "WT+ " + Mathf.Round(equipmentReference.waitCostNormal * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f)).ToString();
                    }

                    // Check if its positive or negative
                    if (playerReference.GetModifier(StatType.SPD).modifierValue > 0)
                    {
                        // Positive so make the text green
                        newStats = newStats.Insert(0, "^");
                    }
                    else
                    {
                        // Negative so make the text red
                        newStats = newStats.Insert(0, "*");
                    }

                    // Make sure to add the colour end symbol
                    newStats += "# | ";
                }

                // No speed mods so just make it normal
                else
                {
                    newStats = statsTextReference.actualText.Substring(statsTextReference.actualText.IndexOf("WT"), 5) + " | ";
                }

                // Make sure to update the final string
                finalStats += newStats;

                print(finalStats);

                // And at the end of it all, add the rest of the text if there is any
                if (statsTextReference.actualText.LastIndexOf("|") > statsTextReference.actualText.IndexOf("WT"))
                {
                    finalStats += statsTextReference.actualText.Substring(statsTextReference.actualText.IndexOf("|", statsTextReference.actualText.IndexOf("WT")) + 2);
                }
                // Otherwise remove the final |
                else
                {
                    finalStats = finalStats.Remove(finalStats.IndexOf("|", finalStats.IndexOf("WT")));
                }

                // Overwrite the actual text
                statsTextReference.SetText(finalStats);
            }

            // Otherwise check if the character has speed mod
            else if (playerReference.HasModifier(StatType.SPD))
            {
                string newStats = "";

                // Create new speed value as string

                // Check if judgement or not
                if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
                {
                    newStats = "WT+ " + Mathf.Round(equipmentReference.waitCostJudgement * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f)).ToString();
                }
                else
                {
                    newStats = "WT+ " + Mathf.Round(equipmentReference.waitCostNormal * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f)).ToString();
                }

                // Check if its positive or negative
                if (playerReference.GetModifier(StatType.SPD).modifierValue > 0)
                {
                    // Positive so make the text green
                    newStats = newStats.Insert(0, "^");
                }
                else
                {
                    // Negative so make the text red
                    newStats = newStats.Insert(0, "*");
                }

                // Make sure to add the colour end symbol
                newStats += "# | ";

                // Add in the rest of the text
                newStats += statsTextReference.actualText.Substring(statsTextReference.actualText.IndexOf("|", statsTextReference.actualText.IndexOf("WT")) + 2);

                // Overwrite the actual text
                statsTextReference.SetText(newStats);
            }
        }
    }

    private void ChangeDurabilityInformation()
    {
        durabilityTextReference.SetText("Durability: " + equipmentReference.durability.ToString());
        durabilityTextReference.HideText();
        durabilityTextReference.ShowText();

        // Also change the animation
        GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().NotifyDurabilityChange(equipmentReference.durability);
    }

    // What happens when mouse leaves
    public void OnPointerExit(PointerEventData eventData)
    {
        // Deactivate the text
        descriptionTextReference.HideText();
        statsTextReference.HideText();
        durabilityTextReference.HideText();
        targetTextReference.HideText();

        isInformationDisplaying = false;

        // Show enemy targets with animation if in battle
        if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eCombat)
        {
            // Check if judgement is on
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                FindObjectOfType<CombatManagerScript>().SetEnemyTargetedAnimations(false, equipmentReference.targetJudgement);
            }
            else
            {
                FindObjectOfType<CombatManagerScript>().SetEnemyTargetedAnimations(false, equipmentReference.target);
            }

            // Hide wait position indicators
            FindObjectOfType<CombatManagerScript>().SetWaitPositionIndicators(false, 0);

            // Stop durability animation
            GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().StopAnimation();
        }
    }

    private void SetEnemyTargetedAnimationsOn()
    {
        // Used to prevent off hover fucking with the animations

        // Only do this if info is still displaying
        if (isInformationDisplaying == true)
        {
            // Check if judgement is on
            if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
            {
                FindObjectOfType<CombatManagerScript>().SetEnemyTargetedAnimations(true, equipmentReference.targetJudgement);

                // Show wait position indicators
                FindObjectOfType<CombatManagerScript>().SetWaitPositionIndicators(true, equipmentReference.waitCostJudgement);
            }
            else
            {
                FindObjectOfType<CombatManagerScript>().SetEnemyTargetedAnimations(true, equipmentReference.target);

                // Show wait position indicators
                FindObjectOfType<CombatManagerScript>().SetWaitPositionIndicators(true, equipmentReference.waitCostNormal);
            }
        }
    }
}
