using UnityEngine;
using System.Collections;

public class HoopBehaviour : MonoBehaviour {

    public AudioClip HoopSound2;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerExit(Collider Col)
    {
        if (Col.gameObject.tag == "Bullet" && Col.gameObject.transform.position.y < transform.position.y)
        {
            Debug.Log("Scored");
            GetComponent<ParticleSystem>().Play();
            Col.gameObject.tag = "Untagged";
            GameObject.Find("GameManager").GetComponent<GameManager>().SetScore(10);
            AudioSource.PlayClipAtPoint(HoopSound2, transform.position, 3.0f);
        }
    }
}
