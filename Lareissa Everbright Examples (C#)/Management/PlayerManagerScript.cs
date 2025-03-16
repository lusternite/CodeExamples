using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class holds all persistent information about the player
public class PlayerManagerScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public List<EquipmentInitialisationData> equipmentInventory;

    // This is the list of 4 items that the player has selected to bring into combat
    public List<EquipmentInitialisationData> combatInventory;

    [Tooltip("The size of the combat inventory")]
    public int maxCombatInventoryCount = 4;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    private void Awake()
    {
        equipmentInventory = new List<EquipmentInitialisationData>();
        combatInventory = new List<EquipmentInitialisationData>();
    }

    // Use this for initialization
    void Start () {
        InitialiseEquipment();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Used at the start of a new game to create equipment inventory
    void InitialiseNewEquipmentState()
    {
        equipmentInventory.Clear();

        // Pole Axe
        EquipmentInitialisationData poleAxe = new EquipmentInitialisationData();
        poleAxe.Initialise(13, "Pole Axe");
        equipmentInventory.Add(poleAxe);

        // Long Sword
        EquipmentInitialisationData longSword = new EquipmentInitialisationData();
        longSword.Initialise(7, "Long Sword");
        equipmentInventory.Add(longSword);

        // Spear
        EquipmentInitialisationData spear = new EquipmentInitialisationData();
        spear.Initialise(11, "Spear");
        equipmentInventory.Add(spear);

        // Shield
        EquipmentInitialisationData shield = new EquipmentInitialisationData();
        shield.Initialise(9, "Shield");
        equipmentInventory.Add(shield);

        // Rosary
        EquipmentInitialisationData rosary = new EquipmentInitialisationData();
        rosary.Initialise(7, "Rosary");
        equipmentInventory.Add(rosary);

        // Medicinal Remedy
        EquipmentInitialisationData medicinalRemedy = new EquipmentInitialisationData();
        medicinalRemedy.Initialise(11, "Medicinal Remedy");
        equipmentInventory.Add(medicinalRemedy);

        // Morningstar
        EquipmentInitialisationData morningstar = new EquipmentInitialisationData();
        morningstar.Initialise(8, "Morningstar");
        equipmentInventory.Add(morningstar);

        // Estoc
        EquipmentInitialisationData estoc = new EquipmentInitialisationData();
        estoc.Initialise(9, "Estoc");
        equipmentInventory.Add(estoc);

        // Claymore
        EquipmentInitialisationData claymore = new EquipmentInitialisationData();
        claymore.Initialise(8, "Claymore");
        equipmentInventory.Add(claymore);

        // Warhammer
        EquipmentInitialisationData warhammer = new EquipmentInitialisationData();
        warhammer.Initialise(12, "Warhammer");
        equipmentInventory.Add(warhammer);

        // Recurve Bow
        EquipmentInitialisationData recurveBow = new EquipmentInitialisationData();
        recurveBow.Initialise(11, "Recurve Bow");
        equipmentInventory.Add(recurveBow);

        // Crossbow
        EquipmentInitialisationData crossbow = new EquipmentInitialisationData();
        crossbow.Initialise(10, "Crossbow");
        equipmentInventory.Add(crossbow);

        // Incense
        EquipmentInitialisationData incense = new EquipmentInitialisationData();
        incense.Initialise(8, "Incense");
        equipmentInventory.Add(incense);

        // Greataxe
        EquipmentInitialisationData greataxe = new EquipmentInitialisationData();
        greataxe.Initialise(9, "Greataxe");
        equipmentInventory.Add(greataxe);

        // Flag Staff
        EquipmentInitialisationData flagStaff = new EquipmentInitialisationData();
        flagStaff.Initialise(10, "Flag Staff");
        equipmentInventory.Add(flagStaff);

        // Everlustre
        EquipmentInitialisationData everlustre = new EquipmentInitialisationData();
        everlustre.Initialise(20, "Everlustre");
        equipmentInventory.Add(everlustre);


        // Double check if comfy mode is on
        if (FindObjectOfType<GameManagerScript>().comfyModeFlag == true)
        {
            // Go ahead and increase durability of all equipment by 20.
            for (int i = 0; i < equipmentInventory.Count; i++)
            {
                equipmentInventory[i].ChangeDurability(equipmentInventory[i].durability + 20);
            }
        }

        print("Equipment init");
    }

    // Used at the start of a new game to create demo equipment inventory
    void InitialiseDemoEquipmentState()
    {
        equipmentInventory.Clear();

        // Long Sword
        EquipmentInitialisationData longSword = new EquipmentInitialisationData();
        longSword.Initialise(6, "Long Sword");
        equipmentInventory.Add(longSword);

        // Spear
        EquipmentInitialisationData spear = new EquipmentInitialisationData();
        spear.Initialise(8, "Spear");
        equipmentInventory.Add(spear);

        // Rosary
        EquipmentInitialisationData rosary = new EquipmentInitialisationData();
        rosary.Initialise(6, "Rosary");
        equipmentInventory.Add(rosary);

        // Estoc
        EquipmentInitialisationData estoc = new EquipmentInitialisationData();
        estoc.Initialise(5, "Estoc");
        equipmentInventory.Add(estoc);

        // Claymore
        EquipmentInitialisationData claymore = new EquipmentInitialisationData();
        claymore.Initialise(6, "Claymore");
        equipmentInventory.Add(claymore);

        // Warhammer
        EquipmentInitialisationData warhammer = new EquipmentInitialisationData();
        warhammer.Initialise(9, "Warhammer");
        equipmentInventory.Add(warhammer);

        // Shield
        EquipmentInitialisationData shield = new EquipmentInitialisationData();
        shield.Initialise(8, "Shield");
        equipmentInventory.Add(shield);

        // Recurve Bow
        EquipmentInitialisationData recurveBow = new EquipmentInitialisationData();
        recurveBow.Initialise(7, "Recurve Bow");
        equipmentInventory.Add(recurveBow);


        // Double check if comfy mode is on
        if (FindObjectOfType<GameManagerScript>().comfyModeFlag == true)
        {
            // Go ahead and increase durability of all equipment by 20.
            for (int i = 0; i < equipmentInventory.Count; i++)
            {
                equipmentInventory[i].ChangeDurability(equipmentInventory[i].durability + 20);
            }
        }

        print("Equipment demo init");
    }

    public void InitialiseEquipment()
    {
        // Check for demo items
        if (FindObjectOfType<GameManagerScript>().IsDemo())
        {
            InitialiseDemoEquipmentState();
        }
        else
        {
            InitialiseNewEquipmentState();
        }
    }

    // Used to add an item to the combat inventory, then returns whether it is full
    public bool AddToCombatInventory(EquipmentInitialisationData equipmentType)
    {
        // Add the item
        combatInventory.Add(equipmentType);

        // Return whether it is full
        return (combatInventory.Count == maxCombatInventoryCount);
    }

    public void RemoveFromCombatInventory(EquipmentInitialisationData equipmentType)
    {
        for (int i = 0; i < combatInventory.Count; i++)
        {
            if (combatInventory[i].equipmentName == equipmentType.equipmentName)
            {
                combatInventory.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveEverlustreFromEquipmentInventory()
    {
        for (int i = 0; i < equipmentInventory.Count; i++)
        {
            if (equipmentInventory[i].equipmentName == "Everlustre")
            {
                equipmentInventory.RemoveAt(i);
                break;
            }
        }
    }

    // Used to remove all items in the combat inventory and also reset highlighting on equipment selection buttons
    public void ResetCombatInventory()
    {
        // Clear the inventory
        combatInventory.Clear();

        // Go find all selected equipment and restore them
        GameObject[] selectedEquipment = GameObject.FindGameObjectsWithTag("SelectedEquipment");
        for (int i = 0; i < selectedEquipment.Length; i++)
        {
            // Make the button interactable again
            selectedEquipment[i].GetComponent<Button>().interactable = true;
            selectedEquipment[i].tag = "Untagged";
            selectedEquipment[i].GetComponent<EquipmentSelectionScript>().ResetSpriteStates();
        }
    }

    public void ResetCombatVariables()
    {
        combatInventory.Clear();
    }

    public bool CombatInventoryContains(string equipmentName)
    {
        for (int i = 0; i < combatInventory.Count; i++)
        {
            if (combatInventory[i].equipmentName == equipmentName)
            {
                return true;
            }
        }

        
        return false;
    }

    public List<int> GetEquipmentDurabilityInfo()
    {
        List<int> durabilityInfo = new List<int>();
        for (int i = 0; i < equipmentInventory.Count; i++)
        {
            durabilityInfo.Add(equipmentInventory[i].durability);
        }

        return durabilityInfo;
    }

    public int GetSpecifiedEquipmentDurability(string equipmentName)
    {
        for (int i = 0; i < equipmentInventory.Count; i++)
        {
            if (equipmentInventory[i].equipmentName == equipmentName)
            {
                return equipmentInventory[i].durability;
            }
        }

        print("Invalid equipment name given when checking durability");
        return 99;
    }

    public void OverwriteEquipmentDurability(List<int> newDurabilityValues)
    {
        for (int i = 0; i < newDurabilityValues.Count; i++)
        {
            equipmentInventory[i].ChangeDurability(newDurabilityValues[i]);
        }
        
    }

    public void OverwriteEquipmentDurability(int newDurabilityValue, string equipmentToOverwrite)
    {
        for (int i = 0; i < equipmentInventory.Count; i++)
        {
            if (equipmentInventory[i].equipmentName == equipmentToOverwrite)
            {
                equipmentInventory[i].ChangeDurability(newDurabilityValue);
            }
        }
    }

    // Used to save all durability information in current combat scene
    public void SaveCurrentEquipmentData()
    {
        EquipmentBaseScript[] equipment = FindObjectsOfType<EquipmentBaseScript>();
        for (int i = 0; i < equipment.Length; i++)
        {
            OverwriteEquipmentDurability(equipment[i].durability, equipment[i].equipmentName);
        }
    }
}
