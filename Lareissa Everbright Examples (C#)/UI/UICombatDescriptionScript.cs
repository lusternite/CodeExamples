using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICombatDescriptionScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private Text textReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        textReference = GetComponent<Text>();
        textReference.text = "";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayText(string description)
    {
        // Change text and color to match
        textReference.text = description;
    }

    public void StopDisplay()
    {
        textReference.text = "";
    }
}
