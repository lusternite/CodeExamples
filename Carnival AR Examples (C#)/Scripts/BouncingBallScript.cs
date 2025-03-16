using UnityEngine;
using System.Collections;

public class BouncingBallScript : MonoBehaviour {

    bool _PointsAvalible = true;
    bool _HasHitFloor = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    GameManager getManager()
    {
        GameObject obj = GameObject.Find("GameManager");
        return obj.GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            _HasHitFloor = true;
            Debug.Log("floooooor ");
        }

        if (other.gameObject.tag == "Bucket" && _PointsAvalible == true && _HasHitFloor == true)
        {
            Debug.Log("+5");
            _PointsAvalible = false;
            getManager().SetScore(5);

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);

            //AudioSource.PlayClipAtPoint(HitSound, transform.position, 0.3f);
            //GetComponent<ParticleSystem>().Play();
        }

    }
}
