using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Text tickets;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        tickets.text = GameObject.Find("GameManager").GetComponent<GameManager>().GetScore().ToString();
	}

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void GoToGame(string gameName)
    {
        SceneManager.LoadScene(gameName);
        GameObject.Find("GameManager").GetComponent<GameManager>().SetCurrentLevel(gameName);
    }
}