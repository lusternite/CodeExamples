using UnityEngine;
using System.Collections;

public class GiveBear : MonoBehaviour {

    public float GiveThresholdX = -0.85f;
    public float GiveTimer = 2.0f;
    public GameObject FadeInPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(transform.position);
	    if (transform.position.x <= GiveThresholdX)
        {
            GiveTimer -= Time.deltaTime;
            if (GiveTimer <= 0.0f)
            {
                FadeInPrefab.SetActive(true);
                enabled = false;
            }

        }
	}
}
