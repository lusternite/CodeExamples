using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float travelSpeed;
    bool isSpinning = false;
    float rotationleft = 360;
    float rotationspeed = 1200;
    public AudioClip HitSound;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Rigidbody>().velocity = transform.right * travelSpeed;
        if ((travelSpeed > 0 && transform.position.x > 1.6) || (travelSpeed < 0 && transform.position.x < -1))
        {
            travelSpeed *= -1;
        }

        if (isSpinning)
        {
            float rotation = rotationspeed * Time.deltaTime;
            if (rotationleft > rotation)
            {
                rotationleft -= rotation;
            }
            else
            {
                rotation = rotationleft;
                rotationleft = 0;
            }
            transform.Rotate(rotation, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }

    GameManager getManager()
    {
        GameObject obj = GameObject.Find("GameManager");
        return obj.GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            rotationleft += 360;
            isSpinning = true;
            AudioSource.PlayClipAtPoint(HitSound, transform.position, 0.5f);
            GetComponent<ParticleSystem>().Play();
            collision.gameObject.GetComponent<SphereCollider>().enabled = false;

            if (this.gameObject.name == "Target 1")
                getManager().SetScore(5);
            else if (this.gameObject.name == "Target 2")
                getManager().SetScore(10);
            else if (this.gameObject.name == "Target 3")
                getManager().SetScore(20);
        }
    }
}
