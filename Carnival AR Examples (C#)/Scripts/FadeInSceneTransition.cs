using UnityEngine;
using System.Collections;

public class FadeInSceneTransition : MonoBehaviour {

    public float TransitionDelay = 2.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        TransitionDelay -= Time.deltaTime;
        if (TransitionDelay <= 0.0f)
        {
            Application.LoadLevel(0);
        }
	}
}
