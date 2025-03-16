using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthScript : MonoBehaviour {

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
            textReference.text = Mathf.Clamp(Mathf.Ceil(entityReference.health), 0.0f, entityReference.maxHealth).ToString();
        }
	}

    public float GetCurrentHealth()
    {
        return entityReference.health;
    }
}
