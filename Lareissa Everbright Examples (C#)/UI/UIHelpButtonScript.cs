using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHelpButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    private AudioManagerScript audioManagerReference;

    public GameObject helpBookPrefab;
    public GameObject basicsPageReference;
    public GameObject statsPageReference;
    public GameObject conditionsPageReference;
    public int buttonPage;
    public bool pageActivated;

    static UIHelpButtonScript activePageReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        audioManagerReference = FindObjectOfType<AudioManagerScript>();

        if (buttonPage == 1)
        {
            pageActivated = true;
            activePageReference = this;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check if this is spawn button or bookmark button
        if (helpBookPrefab)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonHover");
        }
        else if (basicsPageReference)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if this is spawn button or bookmark button
        if (helpBookPrefab)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonClick");
        }
        else if(basicsPageReference)
        {
            //audioManagerReference.PlayUISFX("ButtonClickSoft");
            FindObjectOfType<AudioManagerScript>().PlayUISFX("PageTurn");
        }
    }

    public void SpawnHelpBook()
    {
        // See if overlay Canvas exists before spawning
        if (GameObject.Find("OverlayCanvas") != null)
        {
            // Spawn it
            GameObject helpBook = Instantiate(helpBookPrefab, GameObject.Find("OverlayCanvas").transform);
        }
        else
        {
            // Spawn it
            GameObject helpBook = Instantiate(helpBookPrefab, GameObject.Find("Canvas").transform);
        }

        // ZA WARUDO!
        Time.timeScale = 0.0f;

        FindObjectOfType<NarrativeManagerScript>().narrativeInputDelay = 0.5f;
    }

    public void RemoveHelpBook()
    {
        // Find and kill the parent object
        Destroy(transform.parent.gameObject, 0.1f);

        // Play back sound
        audioManagerReference.PlayUISFX("ButtonBack");

        // TOKI GA UGOKI DASU!
        Time.timeScale = 1.0f;

        FindObjectOfType<NarrativeManagerScript>().narrativeInputDelay = 0.5f;
    }

    public void ChangePage()
    {
        // Check that this page is not already activated
        if (pageActivated == false)
        {
            // First hide pages
            HideAllPages();

            // Then reveal the right page
            if (buttonPage == 1)
            {
                basicsPageReference.SetActive(true);
            }
            else if (buttonPage == 2)
            {
                statsPageReference.SetActive(true);
            }
            else if (buttonPage == 3)
            {
                conditionsPageReference.SetActive(true);
            }

            // Set activated to true
            pageActivated = true;

            // Set last activated page to false
            activePageReference.pageActivated = false;
            activePageReference.GetComponent<Button>().interactable = true;

            // Finally set active page to this
            activePageReference = this;

            // Disable the button
            GetComponent<Button>().interactable = false;
        }
    }

    private void HideAllPages()
    {
        basicsPageReference.SetActive(false);
        statsPageReference.SetActive(false);
        conditionsPageReference.SetActive(false);
    }
}
