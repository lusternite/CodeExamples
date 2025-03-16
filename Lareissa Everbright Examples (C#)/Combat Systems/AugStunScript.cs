using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugStunScript : AugmentScript {

    public float stunWaitDuration = 50.0f;

	// Use this for initialization
	void Start () {
        augmentType = AugmentType.STUN;
        // Set duration super high so that it will always last longer than the time to entity's next turn
        augmentDuration = 2000.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    override public void ResetDuration()
    {
        augmentDuration = 2000.0f;
    }

    override public void TickDown(float waitAmount)
    {
        // Allow turn to continue
        FindObjectOfType<CombatManagerScript>().SetCanTurnProceed(true);

        base.TickDown(waitAmount);
    }
}
