using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public EntityBaseScript entityReference;

    private Text textReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        textReference = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (textReference)
        {
            textReference.text = Mathf.Round(entityReference.wait).ToString();
        }
    }
}
