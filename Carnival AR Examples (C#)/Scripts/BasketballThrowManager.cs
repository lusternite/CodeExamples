using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BasketballThrowManager : MonoBehaviour {

    public GameObject BallPrefab;
    public GameObject CurrentlyHoldingBall;
    public bool BallLoaded;
    public bool BallAimed;
    Vector3 LoadedPosition;
    public float FireThreshholdZ = 0.15f;
    public float ReloadThreshholdZ = 0.05f;
    public float BallSpeed = 11.0f;
    public AudioClip ThrowSound;

    int balls = 20;
    public Text balltext;
    public bool GameOver = false;
    float _fTimer = 0.0f;

	// Use this for initialization
	void Start () {
        BallLoaded = false;
        BallAimed = false;
        balltext.text = balls.ToString();

        //GameObject NewBall = (GameObject)Instantiate(BallPrefab);
        //NewBall.transform.position = transform.position;// + transform.right * 0.16f;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(transform.position.y);
        //if (CurrentlyHoldingBall != null)
        //{
        //    //Make ball stay in hand
        //    CurrentlyHoldingBall.transform.position = transform.position + transform.up * -0.05f + transform.right * 0.05f;
        //}

        if (BallLoaded && BallAimed && transform.position.z >= FireThreshholdZ)
        {
            BallLoaded = false;
            BallAimed = false;
            //GameObject NewBall = (GameObject)Instantiate(BallPrefab);
            //NewBall.transform.position = transform.position;// + transform.right * 0.16f;
            CurrentlyHoldingBall.GetComponent<Rigidbody>().useGravity = true;
            CurrentlyHoldingBall.GetComponent<SphereCollider>().enabled = true;
            CurrentlyHoldingBall.transform.parent = null;
            CurrentlyHoldingBall.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position - LoadedPosition) * BallSpeed;
            CurrentlyHoldingBall.tag = "Bullet";

            //balls--;

            CurrentlyHoldingBall = null;
            if (GameOver == false)
            {
                GameObject NewBall = (GameObject)Instantiate(BallPrefab);
                NewBall.transform.position = new Vector3(0.047f, 0.759f, 0.48f);
            }

            AudioSource.PlayClipAtPoint(ThrowSound, transform.position, 1.5f);

            //balltext.text = balls.ToString();
        }
        if (!BallAimed && BallLoaded && transform.position.z >= ReloadThreshholdZ)
        {
            BallAimed = true;
            LoadedPosition = transform.position;
        }
        else if (!BallLoaded && transform.position.z <= ReloadThreshholdZ && transform.position.y >= 1.0f)
        {
            BallLoaded = true;
        }

        if (balls <= 0 && GameOver == false)
        {
            GameOver = true;
            _fTimer = Time.time;
        }
        if (GameOver == true && Time.time - _fTimer > 5.0f)
        {
            //SceneManager.LoadScene("Shooting");
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ball")
        {
            Debug.Log("Triggered");
            col.gameObject.transform.parent = transform;
            col.GetComponent<Rigidbody>().useGravity = false;
            col.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
            col.GetComponent<SphereCollider>().enabled = false;
            CurrentlyHoldingBall = col.gameObject;
            //BallLoaded = true;
        }
    }
}
