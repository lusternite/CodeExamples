using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResolutionScaleAdjustmentScript : MonoBehaviour {

    static int DEFAULTRESOLUTIONX = 1024;
    static int DEFAULTRESOLUTIONY = 764;

	// Use this for initialization
	void Start () {
        GetComponent<RectTransform>().localScale = new Vector3((float)DEFAULTRESOLUTIONX / (float)Screen.currentResolution.width,
             (float)DEFAULTRESOLUTIONY / (float)Screen.currentResolution.height);

        print(Screen.currentResolution.width);
        print(Screen.currentResolution.height);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
