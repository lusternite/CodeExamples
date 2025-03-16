using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarnivalManager : MonoBehaviour
{
    enum GameMode { SHOOTING, CANTHROW };

    public GameObject ballPrefab;
    public GameObject canPrefab;
    public GameObject bulletPrefab;

    public Camera cam;
    public int numBalls;
    public int ticketScore = 0;

    GameObject[] cans;
    GameMode currentGame;

    // UI
    public Text ballCount;
    public Text ticketCount;

    GameManager getManager()
    {
        GameObject obj = GameObject.Find("GameManager");
        return obj.GetComponent<GameManager>();
    }

    // Use this for initialization
    void Start()
    {
        currentGame = GameMode.SHOOTING;
        ticketScore = getManager().GetScore();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGame)
        {
            // Update differently depending on which game you are playing
            case GameMode.CANTHROW:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        GameObject ball = GameObject.Find("Ball");
                        if (ball == null)
                            ball = GameObject.Find("Ball(Clone)");
                        ball.GetComponent<Rigidbody>().velocity = ball.transform.forward * 6;
                        Destroy(ball, 2.0f);
                    }

                    ballCount.text = numBalls.ToString();
                    ticketCount.text = ticketScore.ToString();

                    break;
                }
            case GameMode.SHOOTING:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        GameObject bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(-4.4f, 2.7f, 3.4f), Quaternion.identity);
                        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 8;
                        Destroy(bullet, 2.0f);
                    }
                    break;
                }
            default: break;
        }

        // Switching between game modes
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentGame += 1;
            cam.transform.Translate(new Vector3(4.1f, 0.0f, 0.0f));

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentGame -= 1;
            cam.transform.Translate(new Vector3(-4.1f, 0.0f, 0.0f));
        }
    }

    public void SpawnNewBall()
    {
        Instantiate(ballPrefab, new Vector3(-1.0f, 2.0f, 3.4f), Quaternion.identity);
    }

    public void ReturnToMenu()
    {

    }
}
