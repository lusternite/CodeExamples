using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITitleButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    private AudioManagerScript audioManagerReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        audioManagerReference = FindObjectOfType<AudioManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonClickSoft");

        // Stop button selection
        EventSystem.current.SetSelectedGameObject(null);
    }
}
