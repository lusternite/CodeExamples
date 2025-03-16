using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonScript : MonoBehaviour
{
    public Button soundButton;
    public Sprite soundSprite;
    public Sprite muteSprite;
    bool sound = true;

    // Use this for initialization
    void Start ()
    {
	
	}

    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) => { OnLevelLoaded(); };
    }

    void OnLevelLoaded()
    {
        if (getManager().GetSoundOn())
        {
            Debug.Log("sound on");
            soundButton.GetComponent<Image>().sprite = soundSprite;
        }
        else
        {
            Debug.Log("sound off");
            soundButton.GetComponent<Image>().sprite = muteSprite;
        }
    }

    // Update is called once per frame
    void Update ()
    {
	    
	}

    GameManager getManager()
    {
        GameObject obj = GameObject.Find("GameManager");
        return obj.GetComponent<GameManager>();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ToggleSound()
    {
        getManager().ToggleSound();
        if (getManager().GetSoundOn())
        {
            soundButton.GetComponent<Image>().sprite = soundSprite;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = muteSprite;
        }
    }
}