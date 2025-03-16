using UnityEngine;
using System.Collections;

public class RingScript : MonoBehaviour {

    bool _PointsAvalible = true;
    public AudioClip RingSound;

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

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Peg" && _PointsAvalible == true)
        {

            Debug.Log("+5");
            Debug.Log(transform.position);
            Debug.Log(other.gameObject.transform.position);
            _PointsAvalible = false;
            other.GetComponent<ParticleSystem>().Play();
            getManager().SetScore(5);

            AudioSource.PlayClipAtPoint(RingSound, transform.position, 3.5f);

            //AudioSource.PlayClipAtPoint(HitSound, transform.position, 0.3f);
            //GetComponent<ParticleSystem>().Play();
        }

    }

}
