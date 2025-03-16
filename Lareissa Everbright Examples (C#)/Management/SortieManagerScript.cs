using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortieManagerScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private PlayerManagerScript playerManagerReference;

    private GameObject equipmentPanelReference;

    public GameObject equipmentSelectionButtonPrefab;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        playerManagerReference = GetComponent<PlayerManagerScript>();

        //if (FindObjectsOfType<SortieManagerScript>().Length <= 1)
        //{
        //    SpawnPlayerEquipmentInventory();
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnPlayerEquipmentInventory()
    {
        print("Loading equipment...");
        equipmentPanelReference = GameObject.Find("EquipmentButtonPanel");

        float posX = -660.0f;
        float posY = 400.0f;

        for (int i = 0; i < playerManagerReference.equipmentInventory.Count; i++)
        {
            SpawnEquipmentSelectionButton(playerManagerReference.equipmentInventory[i], posX, posY);

            posY -= 80.0f;

            if (i == 7)
            {
                posX = 720.0f;
                posY = 400.0f;
            }
        }
    }

    // Used to create an equipment button prefab and properly initialise it
    public void SpawnEquipmentSelectionButton(EquipmentInitialisationData equipmentDetails, float spawnX, float spawnY)
    {
        // Create the new button prefab
        GameObject newEquipmentButton = GameObject.Instantiate(equipmentSelectionButtonPrefab, equipmentPanelReference.transform);

        // Create the equipment component and add to the prefab
        InstantiateEquipmentFromName(newEquipmentButton, equipmentDetails);

        // Add an information script to the button
        newEquipmentButton.AddComponent<UIEquipmentInformationScript>();

        // Change button text to the name of the equipment
        newEquipmentButton.GetComponentInChildren<Text>().text = equipmentDetails.equipmentName;

        // Set the button's location
        newEquipmentButton.GetComponent<RectTransform>().localPosition = new Vector3(spawnX, spawnY, 0.0f);

        newEquipmentButton.GetComponent<Animator>().Play("EquipmentButtonInvisible");

        // Set the button's durability
        newEquipmentButton.GetComponent<EquipmentSelectionScript>().SetEquipmentDurabilityText(equipmentDetails.durability);
    }

    // Creates a new equipment component using its name string, attaches it to supplied gameobject
    static public void InstantiateEquipmentFromName(GameObject buttonReference, EquipmentInitialisationData equipmentDetails)
    {
        if (equipmentDetails.equipmentName == "Pole Axe")
        {
            PoleAxeScript newEquipment = buttonReference.AddComponent<PoleAxeScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Long Sword")
        {
            LongSwordScript newEquipment = buttonReference.AddComponent<LongSwordScript>();
            newEquipment.durability = equipmentDetails.durability;
            print("Yes?");
        }
        else if (equipmentDetails.equipmentName == "Spear")
        {
            SpearScript newEquipment = buttonReference.AddComponent<SpearScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Shield")
        {
            ShieldScript newEquipment = buttonReference.AddComponent<ShieldScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Rosary")
        {
            RosaryScript newEquipment = buttonReference.AddComponent<RosaryScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Medicinal Remedy")
        {
            MedicinalRemedyScript newEquipment = buttonReference.AddComponent<MedicinalRemedyScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Morningstar")
        {
            MorningstarScript newEquipment = buttonReference.AddComponent<MorningstarScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Estoc")
        {
            EstocScript newEquipment = buttonReference.AddComponent<EstocScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Claymore")
        {
            ClaymoreScript newEquipment = buttonReference.AddComponent<ClaymoreScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Warhammer")
        {
            WarhammerScript newEquipment = buttonReference.AddComponent<WarhammerScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Recurve Bow")
        {
            RecurveBowScript newEquipment = buttonReference.AddComponent<RecurveBowScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Crossbow")
        {
            CrossbowScript newEquipment = buttonReference.AddComponent<CrossbowScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Incense")
        {
            IncenseScript newEquipment = buttonReference.AddComponent<IncenseScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Greataxe")
        {
            GreataxeScript newEquipment = buttonReference.AddComponent<GreataxeScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Flag Staff")
        {
            FlagStaffScript newEquipment = buttonReference.AddComponent<FlagStaffScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else if (equipmentDetails.equipmentName == "Everlustre")
        {
            EverlustreScript newEquipment = buttonReference.AddComponent<EverlustreScript>();
            newEquipment.durability = equipmentDetails.durability;
        }
        else
        {
            print("Sortie Manager has failed to convert equipment name to a suitable class");
        }
    }
}
