using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HandShooting : MonoBehaviour {

    public GameObject BulletPrefab;

    public bool GameOver = false;
    float _fTimer = 0.0f;
    public AudioClip ShootSound;
    bool hasStarted = false;
    public ShootingGameManager gameManager;

    // Use this for initialization
    void Start ()
    {
        //GameObject.Find("GameManager").GetComponent<GameManager>().ActivateStartPopup();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (GameOver == true && Time.time - _fTimer > 3.0f)
        //{
        //    GameObject.Find("GameManager").GetComponent<GameManager>().ActivatePopup();
        //    GameOver = false;
        //    _fTimer = 0.0f;
        //}
    }

    public void ShootBullet()
    {
        if (GameOver == false)
        {
            GameObject NewBullet = (GameObject)Instantiate(BulletPrefab);
            if (name == "L_Palm")
            {
                NewBullet.transform.position = transform.position - transform.right * 0.16f;
                NewBullet.GetComponent<Rigidbody>().velocity = transform.right * -10.0f;
            }
            else
            {
                NewBullet.transform.position = transform.position + transform.right * 0.16f;
                NewBullet.GetComponent<Rigidbody>().velocity = transform.right * 10.0f;
            }
            
            AudioSource.PlayClipAtPoint(ShootSound, transform.position, 0.5f);
            gameManager.BulletsRemaining -= 1;
        }     
    }
}
