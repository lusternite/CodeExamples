using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEnemyPreviewScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public GameObject equipmentInfoPanelReference;
    public GameObject judgementToggleButtonReference;
    public GameObject equipmentButtonPanelReference;

    private CombatManagerScript combatManagerReference;
    public UITextUnfoldScript enemyPreviewDescriptionReference;

    public bool enemyPreviewShowing = false;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        Invoke("InitialiseEnemyPreview", 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play hover sfx
        FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonHover");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Play click sfx
        FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonClick");
    }

    private void InitialiseEnemyPreview()
    {
        combatManagerReference = FindObjectOfType<CombatManagerScript>();

        // Spawn enemies
        combatManagerReference.SpawnEnemies();

        // Spawn enemy placement indicators
        combatManagerReference.SpawnEnemyPlacementIndicators();

        // Hide them
        combatManagerReference.DisplayEnemies(false);

        enemyPreviewDescriptionReference.SetText(FindObjectOfType<GameManagerScript>().GetEnemyPreviewInfo());
    }

    public void ToggleEnemyPreview()
    {
        // Check if should toggle on or off
        if (enemyPreviewShowing == false)
        {
            // Toggle on

            // Hide all equipment elements
            equipmentInfoPanelReference.SetActive(false);
            equipmentButtonPanelReference.SetActive(false);
            judgementToggleButtonReference.SetActive(false);

            // Display enemy preview elements
            combatManagerReference.DisplayEnemies(true);
            enemyPreviewDescriptionReference.ShowText();

            enemyPreviewShowing = true;
        }
        else
        {
            // Toggle off

            // Show all equipment elements
            equipmentInfoPanelReference.SetActive(true);
            equipmentButtonPanelReference.SetActive(true);
            judgementToggleButtonReference.SetActive(true);

            // Display enemy preview elements
            combatManagerReference.DisplayEnemies(false);
            enemyPreviewDescriptionReference.HideText();

            enemyPreviewShowing = false;
        }

        // Stop button selection
        EventSystem.current.SetSelectedGameObject(null);
    }
}
