using UnityEngine;
using System.Collections;

public class GestureControlManager : MonoBehaviour {

    public float PauseTimer = 1.0f;
    public float ExitToMenuTimer = 1.0f;
    public float ResumeTimer = 1.0f;

    public bool PauseFlag = false;
    public bool ExitToMenuFlag = false;
    public bool ResumeFlag = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HandleGestureFlags();
    }

    public void GesturePause()
    {
        if (ResumeFlag)
        {
            ResumeFlag = false;
        }
        else
        {
            Debug.Log("PAUSED");
            if (GameObject.Find("GameManager").transform.FindChild("PauseGameCanvas").gameObject.activeSelf == false && PauseFlag == false)
            {
                PauseFlag = true;
            }
            else
            {
                PauseFlag = false;
            }
        }
    }

    public void GestureExitToMenu()
    {
        Debug.Log("Yes");
        if (GameObject.Find("GameManager").transform.FindChild("PauseGameCanvas").gameObject.activeSelf == true && ExitToMenuFlag == false)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().GoToMenu();
            ExitToMenuTimer = 1.0f;
            ExitToMenuFlag = false;
        }
        else
        {
            ExitToMenuFlag = false;
        }
    }

    public void GestureResume()
    {
        Debug.Log("Yes");
        if (GameObject.Find("GameManager").transform.FindChild("PauseGameCanvas").gameObject.activeSelf == true && ResumeFlag == false)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().TogglePause();
            ResumeTimer = 1.0f;
            ResumeFlag = true;
        }
        else
        {
            ResumeFlag = false;
        }
    }

    void HandleGestureFlags()
    {
        if (PauseFlag)
        {
            PauseTimer -= Time.deltaTime;
            if (PauseTimer < 0.0f)
            {
                PauseTimer = 1.0f;
                PauseFlag = false;
                GameObject.Find("GameManager").GetComponent<GameManager>().TogglePause();
            }
        }

        if (ExitToMenuFlag)
        {
            ExitToMenuTimer -= Time.deltaTime;
            if (ExitToMenuTimer < 0.0f)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().GoToMenu();
                ExitToMenuTimer = 1.0f;
                ExitToMenuFlag = false;
            }
        }

        if (ResumeFlag)
        {
            ResumeTimer -= Time.deltaTime;
            if (ResumeTimer < 0.0f)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().TogglePause();
                ResumeTimer = 1.0f;
                ResumeFlag = false;
            }
        }
    }
}
