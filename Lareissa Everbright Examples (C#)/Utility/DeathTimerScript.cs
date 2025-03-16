using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimerScript : MonoBehaviour {

    public float timeTilDeath = 1.0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, timeTilDeath);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
