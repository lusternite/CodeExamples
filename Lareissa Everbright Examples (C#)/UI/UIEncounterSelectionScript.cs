using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEncounterSelectionScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private GameManagerScript gameManagerReference;

    public GameObject encounterDropdownReference;

    private bool dropdownShowing = false;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        gameManagerReference = FindObjectOfType<GameManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleEncounterDropdown()
    {
        if (dropdownShowing == false)
        {
            encounterDropdownReference.SetActive(true);
            dropdownShowing = true;
        }
        else
        {
            encounterDropdownReference.SetActive(false);
            dropdownShowing = false;
        }
    }

    public void SelectEncounter(int encounter)
    {
        gameManagerReference.SetSpecifiedEncounter(GameManagerScript.ConvertIntToEncounter(encounter));
        Invoke("ToggleEncounterDropdown", 0.5f);
    }
}
