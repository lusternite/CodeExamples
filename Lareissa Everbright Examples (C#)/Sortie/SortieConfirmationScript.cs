using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SortieConfirmationScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    PlayerManagerScript playerManagerReference;
    GameManagerScript gameManagerReference;

    public bool isConfirmFlag;

	// Use this for initialization
	void Start () {
        playerManagerReference = FindObjectOfType<PlayerManagerScript>();
        gameManagerReference = FindObjectOfType<GameManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play hover sfx
        FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Figure out if confirm or deny
        if (isConfirmFlag)
        {
            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonClickSoft");
        }
        else
        {
            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonBack");
        }
    }

    // What to do when confirm button pressed
    public void OnConfirm()
    {
        // Change the scene to combat
        gameManagerReference.LoadCombatScene();
    }

    // What to do when deny button pressed
    public void OnDeny()
    {
        // Reset sortie state back to normal
        playerManagerReference.ResetCombatInventory();

        // Destroy the confirmation window
        Destroy(transform.parent.gameObject);
    }
}
