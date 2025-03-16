using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSelectionScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private PlayerManagerScript playerManagerReference;
    private AudioManagerScript audioManagerReference;
    private Button buttonReference;
    private Image imageReference;

    public GameObject confirmationWindowPrefab;
    public Sprite equipmentSelectionSpriteNormal;
    public Sprite equipmentSelectionSpriteDark;
    public Sprite equipmentSelectionSpriteDisabled;
    public bool equipmentSelectedFlag = false;

    static private SpriteState unselectedSpriteState;
    static private SpriteState selectedSpriteState;
    static public bool spriteStatesSet;

    public Text durabilityTextReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        playerManagerReference = FindObjectOfType<PlayerManagerScript>();
        audioManagerReference = FindObjectOfType<AudioManagerScript>();

        buttonReference = GetComponent<Button>();
        imageReference = GetComponent<Image>();

        if (spriteStatesSet == false)
        {
            // Set up sprite states
            selectedSpriteState = new SpriteState
            {
                highlightedSprite = equipmentSelectionSpriteDisabled,
                pressedSprite = equipmentSelectionSpriteDark
            };

            unselectedSpriteState = buttonReference.spriteState;

            spriteStatesSet = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play hover sfx
        audioManagerReference.PlayUISFX("EquipmentButtonHover");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Play click sfx
        audioManagerReference.PlayUISFX("EquipmentButtonClick");
    }

    // Called when this button is clicked, adds this equipment to the player manager's
    // combat inventory and spawns a confirmation button if the inventory is full
    // Deselects if already selected
    public void OnSelect()
    {
        // Check if button is selected or not
        if (equipmentSelectedFlag == false)
        {
            // Add item to combat inventory and check if full
            if (playerManagerReference.AddToCombatInventory(GetComponent<EquipmentBaseScript>().CreateEquipmentInitialisationData()))
            {
                // If full, spawn confirmation button and wait for response
                Instantiate(confirmationWindowPrefab, GameObject.Find("Canvas").transform);
            }

            // Also add a tag so that selected equipment can be easily found
            tag = "SelectedEquipment";

            equipmentSelectedFlag = true;

            // Finally set the sprite states
            imageReference.sprite = equipmentSelectionSpriteDisabled;
            buttonReference.spriteState = selectedSpriteState;

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            // Remove the item from combat inventory
            playerManagerReference.RemoveFromCombatInventory(GetComponent<EquipmentBaseScript>().CreateEquipmentInitialisationData());

            // Remove the tag
            tag = "Untagged";

            equipmentSelectedFlag = false;

            // Finally set the sprite states
            imageReference.sprite = equipmentSelectionSpriteNormal;
            buttonReference.spriteState = unselectedSpriteState;

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }

    public void ResetSpriteStates()
    {
        imageReference.sprite = equipmentSelectionSpriteNormal;
        buttonReference.spriteState = unselectedSpriteState;

        equipmentSelectedFlag = false;
    }

    public void SetEquipmentDurabilityText(int durability)
    {
        durabilityTextReference.text = "x" + durability.ToString();
    }
}
