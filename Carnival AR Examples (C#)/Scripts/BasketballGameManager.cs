using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BasketballGameManager : MonoBehaviour {

    //This game is on a timer
    public float GameTimeRemaining = 30.0f;
    public Text TimeText;
    public BasketballThrowManager RightPalm;
    public BasketballThrowManager LeftPalm;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameTimeRemaining -= Time.deltaTime;
        if (GameTimeRemaining < 0.0f)
        {
            RightPalm.GameOver = true;
            LeftPalm.GameOver = true;
            TimeText.text = "0";
            if (GameTimeRemaining < -5.0f)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
        else
        {
            TimeText.text = (Mathf.Ceil(GameTimeRemaining)).ToString();
        }
	}
}
