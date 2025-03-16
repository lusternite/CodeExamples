using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIComfyModeScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject toolTipReference;
    public GameObject hyperParentReference;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show the toooooltip
        toolTipReference.SetActive(true);

        // Play sfx
        FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
    }

    // Off hover
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide the toooooltip
        toolTipReference.SetActive(false);
    }

    public void ToggleComfyMode(bool shouldPlaySound = true)
    {
        // Stop button selection
        EventSystem.current.SetSelectedGameObject(null);

        // Find the game manager
        GameManagerScript gameManagerReference = FindObjectOfType<GameManagerScript>();

        // Toggle
        if (gameManagerReference.comfyModeFlag == true)
        {
            gameManagerReference.comfyModeFlag = false;
        }
        else
        {
            gameManagerReference.comfyModeFlag = true;
        }

        if (shouldPlaySound)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonClickSoft");
        }
    }
}
