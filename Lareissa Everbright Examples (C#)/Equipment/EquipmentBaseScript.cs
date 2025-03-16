using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TargetType
{
    Self,
    SingleFront,
    SingleRear,
    FrontLine,
    RearLine,
    Pierce,
    All
}

// A class that holds minimal information about equipment for inventory use
public class EquipmentInitialisationData
{
    public int durability;
    public string equipmentName;

    public void Initialise(int newDurability, string newEquipmentName)
    {
        durability = newDurability;
        equipmentName = newEquipmentName;
    }

    public void ChangeDurability(int newDurability)
    {
        durability = newDurability;
    }
}

public class EquipmentBaseScript : MonoBehaviour
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public int durability;

    public bool equipmentBrokenFlag;

    public string equipmentName;

    public string equipmentDescription;

    public string equipmentStats;

    public string equipmentTarget;

    public string equipmentJudgementName;

    public string equipmentJudgementStats;

    public string equipmentJudgementTarget;

    public Sprite equipmentIcon;

    public Sprite equipmentJudgementIcon;

    public Image equipmentIconImageReference;

    protected PlayerBehaviourScript playerReference;

    protected CombatManagerScript combatManagerReference;

    protected AudioManagerScript audioManagerReference;

    public TargetType target;

    public TargetType targetJudgement;

    [Header("Standard action settings")]
    public float accuracyNormal;

    public float damageLowerStandard;

    public float damageHigherStandard;

    public float waitCostNormal;

    [Header("Judgement action settings")]
    public float accuracyJudgement;

    public float damageLowerJudgement;

    public float damageHigherJudgement;

    public float waitCostJudgement;



    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    protected virtual void Start()
    {
        playerReference = FindObjectOfType<PlayerBehaviourScript>();
        combatManagerReference = CombatManagerScript.GetCombatManager();
        audioManagerReference = FindObjectOfType<AudioManagerScript>();

        // Find the icon image
        equipmentIconImageReference = GetComponentsInChildren<Image>()[1];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Initialise(int initDurability, string initName)
    {
        durability = initDurability;
        equipmentName = initName;
    }

    public void SetEquipmentIcon()
    {
        equipmentIconImageReference.sprite = equipmentIcon;
    }

    public void SetEquipmentJudgementIcon()
    {
        equipmentIconImageReference.sprite = equipmentJudgementIcon;
    }

    public virtual void UseEquipment()
    {
        durability -= 1;
        GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().NotifyDurabilityChange(durability);
        // Check if equipment is destroyed
        if (durability <= 0)
        {
            HandleEquipmentBreaking();
        }

        // Check to see if player is slowed
        if (playerReference.HasModifier(StatType.SPD))
        {
            if (playerReference.GetModifier(StatType.SPD).modifierDuration != 100)
            {
                playerReference.wait += waitCostNormal * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f);
            }
            else
            {
                // Speed has been recently refreshed
                if (playerReference.spdRefreshPreviousAmount != 0.0f)
                {
                    playerReference.wait += waitCostNormal * ((100.0f - playerReference.spdRefreshPreviousAmount) / 100.0f);
                }
                else
                {
                    playerReference.wait += waitCostNormal;
                }
            }
        }
        else
        {
            playerReference.wait += waitCostNormal;
        }


        // Inform equipment description that durability has changed if necessary
        if (GetComponent<UIEquipmentInformationScript>().isInformationDisplaying)
        {
            GetComponent<UIEquipmentInformationScript>().UpdateDurabilityInformation();
        }

        // Tell combat manager turn is complete
        combatManagerReference.NotifyTurnComplete();
    }

    public virtual void UseJudgement()
    {
        durability -= 1;
        GetComponentInChildren<UIEquipmentDurabilityAnimationScript>().NotifyDurabilityChange(durability);
        // Check if equipment is destroyed
        if (durability <= 0)
        {
            HandleEquipmentBreaking();
        }

        // Check to see if player is slowed
        if (playerReference.HasModifier(StatType.SPD))
        {
            playerReference.wait += waitCostJudgement * ((100.0f - playerReference.GetModifier(StatType.SPD).modifierValue) / 100.0f);
        }
        else
        {
            playerReference.wait += waitCostJudgement;
        }

        // Tell combat manager to reset judgement
        combatManagerReference.NotifyJudgementUsed();

        // Inform equipment description that all information is no longer judgement if neccessary
        if (GetComponent<UIEquipmentInformationScript>().isInformationDisplaying)
        {
            GetComponent<UIEquipmentInformationScript>().ResetEquipmentInformationUI();
        }

        // Tell combat manager this was used
        combatManagerReference.previousUsedEquipment = this;

        // Tell combat manager turn is complete
        combatManagerReference.NotifyTurnComplete();
    }

    public virtual void HandleEquipmentBreaking()
    {
        // Turn flag on
        equipmentBrokenFlag = true;

        // Change equipment icon color to red
        equipmentIconImageReference.color = Color.red;

        // Set achievement
        FindObjectOfType<AchievementManagerScript>().UnlockAchievement("ACH_BROKEN_EQUIP");
    }

    public virtual void DelayedEquipmentBreaking(float delay = 0.2f)
    {
        // Does the equipment break but after some time to avoid bug.
        Invoke("HandleEquipmentBreaking", delay);
    }

    // Does the opposite of equipment breaking
    public virtual void RestoreEquipment()
    {
        // Turn flag off
        equipmentBrokenFlag = false;

        // Change equipment icon color to white
        equipmentIconImageReference.color = Color.white;
    }

    public virtual void HandleEquipmentBanishing()
    {
        // Turn flag on
        equipmentBrokenFlag = true;

        // Change equipment icon color to purple
        equipmentIconImageReference.color = Color.magenta;
    }

    // Used to determine if an action hits or not
    protected virtual bool TestAccuracy(float accuracy)
    {
        float realAccuracy = accuracy;

        // Check if the player is blind
        if (playerReference.HasAugment(AugmentType.BLIND))
        {
            // Reduce accuracy by 120
            realAccuracy -= 120.0f;
        }

        // Check if player has accuracy modifiers
        if (playerReference.HasModifier(StatType.ACC))
        {
            realAccuracy += playerReference.GetModifier(StatType.ACC).modifierValue;
        }

        // Check if hits with accuracy modifiers
        return CombatManagerScript.TestAccuracy(realAccuracy);
    }

    protected virtual float CalculateDamage(float damageLower, float damageHigher)
    {
        float damage = Mathf.Round(Random.Range(damageLower, damageHigher));
        if (playerReference.HasModifier(StatType.DMG))
        {
            damage *= 1 + playerReference.GetModifier(StatType.DMG).modifierValue / 100.0f;
        }
        return damage;
    }

    // Calculate healing scaled by Faith
    protected virtual float CalculateHealing(float healingLower, float healingHigher)
    {
        float healing = Mathf.Floor(Random.Range(healingLower, healingHigher));
        if (playerReference.HasModifier(StatType.FTH))
        {
            healing *= 1 + playerReference.GetModifier(StatType.DMG).modifierValue / 100.0f;
        }
        return healing;
    }

    protected string ConvertTargetTypeToString(TargetType targetType)
    {
        if (targetType == TargetType.Self)
        {
            return "Self";
        }
        else if (targetType == TargetType.SingleFront)
        {
            return "Single Front";
        }
        else if (targetType == TargetType.SingleRear)
        {
            return "Single Rear";
        }
        else if (targetType == TargetType.FrontLine)
        {
            return "Front Line";
        }
        else if (targetType == TargetType.RearLine)
        {
            return "Rear Line";
        }
        else if (targetType == TargetType.Pierce)
        {
            return "Pierce";
        }
        else if (targetType == TargetType.All)
        {
            return "All";
        }
        else
        {
            return "Target data not found. Nyan.";
        }
    }

    // Creates struct of type equipment initialisation data using this object's info
    public EquipmentInitialisationData CreateEquipmentInitialisationData()
    {
        if (equipmentName.Length == 0)
        {
            print("Issue with CreateEquipmentInitialisationData, equipment name does not exist for: " + name);
        }
        EquipmentInitialisationData newData = new EquipmentInitialisationData();
        newData.Initialise(durability, equipmentName);

        return newData;
    }

    public void SetUpEquipmentUseButton()
    {
        // Add listener to button so that equipment is used on click
        GetComponent<Button>().onClick.AddListener(UseEquipment);

        print("Button click linked!");
    }

    protected void DisableButtonSelection()
    {
        // Stop button selection
        EventSystem.current.SetSelectedGameObject(null);
    }
}
