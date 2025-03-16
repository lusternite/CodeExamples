using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISettingsButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private CombatManagerScript combatManagerReference;
    private AudioManagerScript audioManagerReference;

    public GameObject SettingsMenuPrefab;
    public Text volumeStateReference;
    public bool isBGMFlag;
    public bool exitButtonFlag;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start()
    {
        combatManagerReference = FindObjectOfType<CombatManagerScript>();
        audioManagerReference = FindObjectOfType<AudioManagerScript>();

        if (volumeStateReference)
        {
            if (isBGMFlag == true)
            {
                ChangeBGMVolumeStateText();
            }
            else
            {
                ChangeSFXVolumeState();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {

        // Check if this is spawn button or bookmark button
        if (SettingsMenuPrefab)
        {
            // Check if this is title screen or not
            if (FindObjectOfType<GameManagerScript>().currentScene != ESceneType.eTitle)
            {
                FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonHover");
            }
            else
            {
                FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
            }
        }
        else if (volumeStateReference || !exitButtonFlag)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonHoverSoft");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        // Check if this is spawn button or bookmark button
        if (SettingsMenuPrefab)
        {
            // Check if this is title screen or not
            if (FindObjectOfType<GameManagerScript>().currentScene != ESceneType.eTitle)
            {
                FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonClick");
            }
            else
            {
                FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonClickSoft");

                // Stop button selection
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else if (volumeStateReference)
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonClickSoft");
        }
        else
        {
            FindObjectOfType<AudioManagerScript>().PlayUISFX("ButtonBack");
        }
    }

    public void SpawnSettingsMenu()
    {
        // Check that its not already spawned
        if (!GameObject.Find("SettingsPanel"))
        {
            GameObject settingsMenu;

            // See if overlay Canvas exists before spawning
            if (GameObject.Find("OverlayCanvas") != null)
            {
                // Spawn it
                settingsMenu = Instantiate(SettingsMenuPrefab, GameObject.Find("OverlayCanvas").transform);
            }
            else
            {
                // Spawn it
                settingsMenu = Instantiate(SettingsMenuPrefab, GameObject.Find("Canvas").transform);
            }

            // Check if this is title screen or not
            if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eTitle)
            {
                // Set its state to on if necessary
                if (FindObjectOfType<GameManagerScript>().comfyModeFlag == true)
                {
                    settingsMenu.GetComponentInChildren<Toggle>().isOn = true;
                    settingsMenu.GetComponentInChildren<UIComfyModeScript>().ToggleComfyMode();
                }
            }
            else
            {
                // Set the comfy mode button off
                settingsMenu.GetComponentInChildren<UIComfyModeScript>().hyperParentReference.SetActive(false);


            }

            // ZA WARUDO!
            Time.timeScale = 0.0f;

            FindObjectOfType<NarrativeManagerScript>().narrativeInputDelay = 0.5f;
        }

    }

    public void RemoveSettingsMenu()
    {
        // Find and kill the parent object
        Destroy(transform.parent.gameObject, 0.1f);

        // TOKI GA UGOKI DASU!
        Time.timeScale = 1.0f;

        FindObjectOfType<NarrativeManagerScript>().narrativeInputDelay = 0.5f;

        // Save player preferences
        FindObjectOfType<GameManagerScript>().SavePlayerPreferenceData();
    }

    public void ChangeBGMVolume(bool volumeStateIncrease)
    {
        if (audioManagerReference)
        {
            audioManagerReference.ChangeBGMVolume(volumeStateIncrease);
            ChangeBGMVolumeStateText();
        }
    }

    public void ChangeSFXVolume(bool volumeStateIncrease)
    {
        if (audioManagerReference)
        {
            audioManagerReference.ChangeSFXVolume(volumeStateIncrease);
            ChangeSFXVolumeState();
        }
    }

    private void ChangeBGMVolumeStateText()
    {
        if (audioManagerReference)
        {
            if (audioManagerReference.bgmVolumeState == AudioVolumeState.eMUTE)
            {
                volumeStateReference.text = "MUTE";
            }
            else if (audioManagerReference.bgmVolumeState == AudioVolumeState.eLOW)
            {
                volumeStateReference.text = "LOW";
            }
            else if (audioManagerReference.bgmVolumeState == AudioVolumeState.eMEDIUM)
            {
                volumeStateReference.text = "MEDIUM";
            }
            else if (audioManagerReference.bgmVolumeState == AudioVolumeState.eHIGH)
            {
                volumeStateReference.text = "HIGH";
            }
        }
    }

    private void ChangeSFXVolumeState()
    {
        if (audioManagerReference)
        {
            if (audioManagerReference.sfxVolumeState == AudioVolumeState.eMUTE)
            {
                volumeStateReference.text = "MUTE";
            }
            else if (audioManagerReference.sfxVolumeState == AudioVolumeState.eLOW)
            {
                volumeStateReference.text = "LOW";
            }
            else if (audioManagerReference.sfxVolumeState == AudioVolumeState.eMEDIUM)
            {
                volumeStateReference.text = "MEDIUM";
            }
            else if (audioManagerReference.sfxVolumeState == AudioVolumeState.eHIGH)
            {
                volumeStateReference.text = "HIGH";
            }
        }
    }

    public void ChangeCombatDescriptionSpeed(float newSpeed)
    {
        if (combatManagerReference)
        {
            combatManagerReference.combatDescriptionSpeed = newSpeed;
        }
    }

    // Used to return to the sortie screen
    public void ResetEncounter()
    {
        // Make sure current scene is combat
        if (FindObjectOfType<GameManagerScript>().currentScene == ESceneType.eCombat)
        {
            FindObjectOfType<GameManagerScript>().LoadSortieScene();

            // DEFINITELY MAKE SURE TIME RESUMES
            Time.timeScale = 1.0f;
        }
    }

    public void ReturnToTitle()
    {
        FindObjectOfType<GameManagerScript>().LoadTitleScene();

        // DEFINITELY MAKE SURE TIME RESUMES
        Time.timeScale = 1.0f;
    }
}
