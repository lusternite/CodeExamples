using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortieSelectionCounterScript : MonoBehaviour {

    private Text textReference;

	// Use this for initialization
	void Start () {
        textReference = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        textReference.text = FindObjectOfType<PlayerManagerScript>().combatInventory.Count.ToString() + " / 4 Equipment Selected";
	}
}
