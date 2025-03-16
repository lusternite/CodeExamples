using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugBlockScript : AugmentScript {

	// Use this for initialization
	void Start () {
        augmentType = AugmentType.BLOCK;
        augmentDuration = 90.0f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    override public void ResetDuration()
    {
        augmentDuration = 90.0f;
        Debug.Log("This happened");
    }

    override public void TickDown(float waitAmount)
    {
        // Allow turn to continue
        FindObjectOfType<CombatManagerScript>().SetCanTurnProceed(true);

        base.TickDown(waitAmount);
    }
}
