using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HandPunch : MonoBehaviour {

    public GameObject BallPrefab;
    public bool BallLoaded;
    public bool BallAimed;
    Vector3 LoadedPosition;
    public float FireThreshholdZ = 0.15f;
    public float ReloadThreshholdZ = 0.05f;
    public AudioClip ThrowSound;
    public float _fspeed = 10.0f;
    float _ftimer = 0.0f;
    bool _timerOn = false;

    public FistingGameManager gameManager;
    public bool GameOver = false;

    // Use this for initialization
    void Start () {
        BallLoaded = false;
        BallAimed = false;
        //GameObject.Find("GameManager").GetComponent<GameManager>().ActivateStartPopup();
    }
	
	// Update is called once per frame
	void Update () {
        if (_timerOn == true)
        {
            _ftimer += Time.deltaTime;

        }

        if (BallLoaded && BallAimed && transform.position.z >= FireThreshholdZ && GameOver == false)
        {
            //Debug.Log("End threshhold");
            BallLoaded = false;
            BallAimed = false;
            GameObject NewBall = (GameObject)Instantiate(BallPrefab);
            NewBall.transform.position = transform.position;// + transform.right * 0.16f;
            NewBall.transform.localEulerAngles = new Vector3(-20.0f, 0.0f, 0.0f);
            NewBall.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position - LoadedPosition) * _fspeed;

            gameManager.BallsRemaining -= 1;

            AudioSource.PlayClipAtPoint(ThrowSound, transform.position, 0.5f);
            _timerOn = false;
        }
        else if (_ftimer >= 0.75f && _timerOn)
        {
            //Debug.Log("MISSED TIMER");

            BallLoaded = false;
            BallAimed = false;

            _timerOn = false;
        }

        if (!BallAimed && BallLoaded && transform.position.z >= ReloadThreshholdZ)
        {
            //Debug.Log("Start threshhold");
            BallAimed = true;
            LoadedPosition = transform.position;
            _timerOn = true;
            _ftimer = 0.0f;
        }
        else if (!BallLoaded && transform.position.z <= ReloadThreshholdZ)
        {
            //Debug.Log("Reloaded");
            BallLoaded = true;
        }
    }
}
