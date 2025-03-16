using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitPositionIndicatorScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    public Text waitPositionTextReference;
    static List<string> positionTitles;
    static bool positionTitlesIntialised = false;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    private void Awake()
    {
        // Set up position titles if not set
        if (positionTitlesIntialised == false)
        {
            positionTitles = new List<string>()
            {
                "1st",
                "2nd",
                "3rd",
                "4th",
                "5th"
            }
            ;
            positionTitlesIntialised = true;
        }
        // Hide self first
        StopReveal();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Called to show the wait position
    public void NotifyReveal(int revealPosition)
    {
        GetComponent<Coffee.UIExtensions.UITransitionEffect>().Show();

        // Set the title
        waitPositionTextReference.text = positionTitles[revealPosition - 1];
    }

    // Hides the wait position
    public void StopReveal()
    {
        GetComponent<Coffee.UIExtensions.UITransitionEffect>().effectFactor = 0;
        GetComponent<Coffee.UIExtensions.UITransitionEffect>().Hide();
    }
}
