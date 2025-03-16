using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentDurabilityAnimationScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public Text durabilityTextReference;
    public Animator animatorReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NotifyDurabilityChange(int newDurabilityValue)
    {
        durabilityTextReference.text = "x" + newDurabilityValue.ToString();
    }

    public void PlayNormalBounceAnimation()
    {
        animatorReference.Play("DurabilityNormalBounce");
    }

    public void PlayLowDurabilityBounceAnimation()
    {
        animatorReference.Play("DurabilityFastBounce");
    }

    public void StopAnimation()
    {
        animatorReference.Play("DurabilityIdle");
    }
}
