using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private Text textReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        textReference = GetComponent<Text>();
        textReference.text = "";
        textReference.color = Color.red;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayDamage(float damageValue)
    {
        // Change text and color to match
        textReference.text = "It deals " + damageValue.ToString() + " damage!";
        textReference.color = Color.red;
    }

    public void DisplayHealing(float healingValue)
    {
        // Change text and color to match
        textReference.text = "It heals " + healingValue.ToString() + " health!";
        textReference.color = Color.green;
    }

    public void StopDisplay()
    {
        textReference.text = "";
    }
}
