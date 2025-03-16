using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShootingGameManager : MonoBehaviour {

    public int BulletsRemaining = 15;
    public Text BulletText;
    public HandShooting RightPalm;
    public HandShooting LeftPalm;
    public bool GameOver = false;
    public float SceneTransitionTimer = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        BulletText.text = BulletsRemaining.ToString();
        if (BulletsRemaining <= 0)
        {
            GameOver = true;
            RightPalm.GetComponent<HandShooting>().GameOver = true;
            LeftPalm.GetComponent<HandShooting>().GameOver = true;
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
