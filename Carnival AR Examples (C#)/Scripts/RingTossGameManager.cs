using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RingTossGameManager : MonoBehaviour {

    public int RingsRemaining = 20;
    public Text RingText;
    public RingThrow RightPalm;
    public RingThrow LeftPalm;
    public bool GameOver = false;
    public float SceneTransitionTimer = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        RingText.text = RingsRemaining.ToString();
        if (RingsRemaining <= 0)
        {
            GameOver = true;
            RightPalm.GetComponent<RingThrow>().GameOver = true;
            LeftPalm.GetComponent<RingThrow>().GameOver = true;
        }
        else
        {

        }
        if (GameOver)
        {
            SceneTransitionTimer -= Time.deltaTime;
            if (SceneTransitionTimer < 0.0f)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
	}
}
