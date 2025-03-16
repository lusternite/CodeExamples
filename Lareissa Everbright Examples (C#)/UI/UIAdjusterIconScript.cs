using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAdjusterIconScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public Text textReference;

    public Image imageReference;

    public EntityBaseScript entityReference;

    // Used to determine if icon is flashing from having negative wt
    public bool endingFlag;

    // This flag is used to determine if this adjuster is an augment or mod
    public bool augmentFlag;

    public AugmentType augmentType;

    public StatType modifierType;

    public GameObject adjusterTooltipPrefab;

    private GameObject adjusterTooltipReference;

    [Header("Modifier Icons")]
    public Sprite modPositiveIcon;

    public Sprite modNegativeIcon;

    [Header("Augment Icons")]
    public Sprite augBleedIcon;

    public Sprite augBlindIcon;

    public Sprite augBlockIcon;

    public Sprite augStunIcon;

    public Sprite augUnyieldingIcon;

    public Sprite augVenomIcon;

    public Sprite augBanishIcon;

    public Sprite augDysphoriaIcon;

    public Sprite augCounterIcon;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start()
    {
        //textReference = GetComponentInChildren<Text>();
        //imageReference = GetComponent<Image>();

        InvokeRepeating("CheckEndingStatus", 1.0f, 0.2f);
    }

    public void InitialiseModifier(StatType mod, float modValue)
    {
        modifierType = mod;
        augmentFlag = false;

        // Check if mod is negative or positive
        if (modValue < 0.0f)
        {
            imageReference.sprite = modNegativeIcon;
        }
        else
        {
            imageReference.sprite = modPositiveIcon;
        }

        // Change the text to match mod type
        if (mod == StatType.ACC)
        {
            textReference.text = "ACC";
        }
        else if (mod == StatType.DEF)
        {
            textReference.text = "DEF";
        }
        else if (mod == StatType.DMG)
        {
            textReference.text = "DMG";
        }
        else if (mod == StatType.FTH)
        {
            textReference.text = "FTH";
        }
        else if (mod == StatType.SPD)
        {
            textReference.text = "SPD";
        }
    }

    public void InitialiseAugment(AugmentType aug)
    {
        augmentType = aug;
        augmentFlag = true;

        // Change the icon to match
        if (aug == AugmentType.BLIND)
        {
            imageReference.sprite = augBlindIcon;
        }
        else if (aug == AugmentType.BLOCK)
        {
            imageReference.sprite = augBlockIcon;
        }
        else if (aug == AugmentType.STUN)
        {
            imageReference.sprite = augStunIcon;
        }
        else if (aug == AugmentType.UNYIELDING)
        {
            imageReference.sprite = augUnyieldingIcon;
        }
        else if (aug == AugmentType.VENOM)
        {
            imageReference.sprite = augVenomIcon;
        }
        else if (aug == AugmentType.BLEED)
        {
            imageReference.sprite = augBleedIcon;
        }
        else if (aug == AugmentType.BANISH)
        {
            imageReference.sprite = augBanishIcon;
        }
        else if (aug == AugmentType.DYSPHORIA)
        {
            imageReference.sprite = augDysphoriaIcon;
        }
        else if (aug == AugmentType.COUNTER)
        {
            imageReference.sprite = augCounterIcon;
        }
    }

    public void ResetModifierIcon(float newValue)
    {
        if (newValue < 0.0f)
        {
            imageReference.sprite = modNegativeIcon;
        }
        else
        {
            imageReference.sprite = modPositiveIcon;
        }
        ResetEndingStatus();
    }

    public void ResetEndingStatus()
    {
        if (endingFlag == true)
        {
            GetComponent<Animator>().Play("AdjusterIdle");
            InvokeRepeating("CheckEndingStatus", 1.0f, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if wt is negative
        if (adjusterTooltipReference)
        {
            SetTooltipWait();
        }
    }

    // What happens when mouse hovers over the icon
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Spawn the tooltip
        adjusterTooltipReference = Instantiate(adjusterTooltipPrefab, transform);

        // Make sure tooltip is in right location
        adjusterTooltipReference.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 84.0f);

        // Set the correct description
        SetTooltipDescription();

        // Set the correct wait remaining
        SetTooltipWait();
    }

    // What happens when mouse hovers over the icon
    public void OnPointerExit(PointerEventData eventData)
    {
        // Remove the tooltip if it exists
        if (adjusterTooltipReference)
        {
            Destroy(adjusterTooltipReference);
        }
    }

    private void SetTooltipDescription()
    {
        // Figure out what this is to be able to set a description

        // Is it a modifier or augment
        if (augmentFlag == true)
        {
            // It's an augment, what type of augment
            if (augmentType == AugmentType.BLEED)
            {
                // Figure out damage about to be dealt
                float incomingDamage = entityReference.GetAugment(AugmentType.BLEED).augmentDamage;

                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Suffers " + incomingDamage.ToString() + " damage at the start of turn.";
            }
            else if (augmentType == AugmentType.BLIND)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Reduces accuracy by 120";
            }
            else if (augmentType == AugmentType.BLOCK)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Prevents next instance of damage then counters by increasing judgement and setting WT to 1.";
            }
            else if (augmentType == AugmentType.STUN)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Skips next turn and adds 50 WT.";
            }
            else if (augmentType == AugmentType.UNYIELDING)
            {
                // Figure out remaining charges
                int remainingCharges = entityReference.GetAugment(AugmentType.UNYIELDING).augmentCharges;

                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Damage cannot drop health below 1. Remaining charges: " + remainingCharges.ToString();
            }
            else if (augmentType == AugmentType.VENOM)
            {
                // Figure out damage about to be dealt
                float incomingDamage = entityReference.GetAugment(AugmentType.VENOM).augmentDamage;

                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Suffers " + incomingDamage.ToString() + " damage at the start of turn. Damage increases every turn.";
            }
            else if (augmentType == AugmentType.BANISH)
            {
                string equipmentName = ((AugBanishScript)(entityReference.GetAugment(AugmentType.BANISH))).equipmentReference.equipmentName;
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = equipmentName + " cannot be used.";
            }
            else if (augmentType == AugmentType.DYSPHORIA)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Will destroy next damaging equipment used.";
            }
            else if (augmentType == AugmentType.COUNTER)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Prevents next instance of damage then counters by increasing revenge and setting WT to 1.";
            }
        }
        else
        {
            // It's a modifier, what type of modifier
            if (modifierType == StatType.ACC)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Accuracy modified by " + entityReference.GetModifier(StatType.ACC).modifierValue.ToString();
            }
            else if (modifierType == StatType.DEF)
            {
                // Figure out if positive or negative
                if (entityReference.GetModifier(StatType.DEF).modifierValue > 0)
                {
                    // Positive
                    // Set the description
                    adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Damage received reduced by " + entityReference.GetModifier(StatType.DEF).modifierValue.ToString() + "%";
                }
                else
                {
                    // Negative
                    // Set the description
                    adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Damage received increased by " + (Mathf.Abs(entityReference.GetModifier(StatType.DEF).modifierValue)).ToString() + "%";
                }
            }
            else if (modifierType == StatType.DMG)
            {
                // Figure out if positive or negative
                if (entityReference.GetModifier(StatType.DMG).modifierValue > 0)
                {
                    // Positive
                    // Set the description
                    adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Damage dealt increased by " + entityReference.GetModifier(StatType.DMG).modifierValue.ToString() + "%";
                }
                else
                {
                    // Negative
                    // Set the description
                    adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Damage dealt reduced by " + Mathf.Abs(entityReference.GetModifier(StatType.DMG).modifierValue).ToString() + "%";
                }
            }
            else if (modifierType == StatType.SPD)
            {
                // Figure out if positive or negative
                if (entityReference.GetModifier(StatType.SPD).modifierValue > 0)
                {
                    // Positive
                    // Set the description
                    adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Wait Cost of actions reduced by " + entityReference.GetModifier(StatType.SPD).modifierValue.ToString() + "%";
                }
                else
                {
                    // Negative
                    // Set the description
                    adjusterTooltipReference.transform.GetChild(0).GetComponent<Text>().text = "Wait Cost of actions increased by " + Mathf.Abs(entityReference.GetModifier(StatType.SPD).modifierValue).ToString() + "%";
                }
            }
        }
    }

    private void SetTooltipWait()
    {
        // Figure out what this is to be able to set a description

        // Is it a modifier or augment
        if (augmentFlag == true)
        {
            // It's an augment, what type of augment
            if (augmentType == AugmentType.BLEED)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.BLEED).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.BLIND)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.BLIND).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.BLOCK)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.BLOCK).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.STUN)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.STUN).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.UNYIELDING)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.UNYIELDING).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.VENOM)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.VENOM).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.BANISH)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.BANISH).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.DYSPHORIA)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.DYSPHORIA).augmentDuration - entityReference.waitStack).ToString();
            }
            else if (augmentType == AugmentType.COUNTER)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetAugment(AugmentType.COUNTER).augmentDuration - entityReference.waitStack).ToString();
            }
        }
        else
        {
            // It's a modifier, what type of modifier
            if (modifierType == StatType.ACC)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetModifier(StatType.ACC).modifierDuration - entityReference.waitStack).ToString();
            }
            else if (modifierType == StatType.DEF)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetModifier(StatType.DEF).modifierDuration - entityReference.waitStack).ToString();
            }
            else if (modifierType == StatType.DMG)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetModifier(StatType.DMG).modifierDuration - entityReference.waitStack).ToString();
            }
            else if (modifierType == StatType.SPD)
            {
                // Set the description
                adjusterTooltipReference.transform.GetChild(1).GetComponent<Text>().text = "WT remaining: " + Mathf.Ceil(entityReference.GetModifier(StatType.SPD).modifierDuration - entityReference.waitStack).ToString();
            }
        }
    }

    private void CheckEndingStatus()
    {
        // Figure out what this is to be able to determine if wt is negative

        // Is it a modifier or augment
        if (augmentFlag == true)
        {
            // It's an augment, what type of augment
            if (augmentType == AugmentType.BLEED)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.BLEED).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.BLIND)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.BLIND).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.BLOCK)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.BLOCK).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.STUN)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.STUN).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.UNYIELDING)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.UNYIELDING).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.VENOM)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.VENOM).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.BANISH)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.BANISH).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.DYSPHORIA)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.DYSPHORIA).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (augmentType == AugmentType.COUNTER)
            {
                // Check if negative
                if (entityReference.GetAugment(AugmentType.COUNTER).augmentDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
        }
        else
        {
            // It's a modifier, what type of modifier
            if (modifierType == StatType.ACC)
            {
                // Check if negative
                if (entityReference.GetModifier(StatType.ACC).modifierDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (modifierType == StatType.DEF)
            {
                // Check if negative
                if (entityReference.GetModifier(StatType.DEF).modifierDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (modifierType == StatType.DMG)
            {
                // Check if negative
                if (entityReference.GetModifier(StatType.DMG).modifierDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
            else if (modifierType == StatType.SPD)
            {
                // Check if negative
                if (entityReference.GetModifier(StatType.SPD).modifierDuration - entityReference.waitStack < 0.0f && entityReference.tickdownFlag == false)
                {
                    endingFlag = true;
                    GetComponent<Animator>().Play("AdjusterBlinking");
                    CancelInvoke();
                }
            }
        }
    }
}
