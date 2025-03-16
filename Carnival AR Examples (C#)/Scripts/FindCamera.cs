using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FindCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        	
	}
	
    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) => { OnLevelLoaded(); };
    }

    void OnLevelLoaded()
    {
        this.gameObject.GetComponent<Canvas>().worldCamera = GameObject.Find("FPSController/Camera").GetComponent<Camera>();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
