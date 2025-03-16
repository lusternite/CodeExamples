using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HammerHand : MonoBehaviour {
    Vector3 _LastPosition;
    Vector3 _CurrentPosition;
    float _f = 0.0f;

    public Vector3 _Velocity;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        _f += Time.deltaTime;
        if (_f > 0.05f)
        {
            _f = 0.0f;
            _LastPosition = _CurrentPosition;
            _CurrentPosition = transform.position;
            _Velocity = _CurrentPosition - _LastPosition;
        }
    }
}
