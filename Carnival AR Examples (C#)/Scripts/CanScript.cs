using UnityEngine;
using System.Collections;

public class CanScript : MonoBehaviour
{
    public SphereCollider standCollider;
    public AudioClip HitSound;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    GameManager getManager()
    {
        GameObject obj = GameObject.Find("GameManager");
        return obj.GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {
            GetComponent<ParticleSystem>().Play();
            AudioSource.PlayClipAtPoint(HitSound, transform.position, 0.5f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == standCollider)
        {
            getManager().SetScore(10);
            Destroy(this, 2.0f);
        }
    }
}
