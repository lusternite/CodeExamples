using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIGameOverScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public Button continueButtonReference;

    public Button titleButtonReference;

    public bool isButtonFlag;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {

        if (isButtonFlag == false)
        {
            // Add load sortie function to continue button
            continueButtonReference.onClick.AddListener(FindObjectOfType<GameManagerScript>().LoadSortieScene);
            titleButtonReference.onClick.AddListener(FindObjectOfType<GameManagerScript>().LoadTitleScene);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only play sound if is button
        if (isButtonFlag == true)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only play sound if is button
        if (isButtonFlag == true)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonClickSoft");
        }
    }
}
