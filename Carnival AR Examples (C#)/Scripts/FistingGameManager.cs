using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FistingGameManager : MonoBehaviour {

    public int BallsRemaining = 20;
    public Text BallText;
    public HandPunch RightPalm;
    public HandPunch LeftPalm;
    public bool GameOver = false;
    public float SceneTransitionTimer = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        BallText.text = BallsRemaining.ToString();
	    if (BallsRemaining <= 0)
        {
            GameOver = true;
            RightPalm.GetComponent<HandPunch>().GameOver = true;
            LeftPalm.GetComponent<HandPunch>().GameOver = true;
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
