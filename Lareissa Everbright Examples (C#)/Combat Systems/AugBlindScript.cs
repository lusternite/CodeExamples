using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugBlindScript : AugmentScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//



    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        augmentType = AugmentType.BLIND;
        augmentDuration = 55.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    override public void ResetDuration()
    {
        augmentDuration = 55.0f;
    }

    override public void TickDown(float waitAmount)
    {
        // Allow turn to continue
        FindObjectOfType<CombatManagerScript>().SetCanTurnProceed(true);

        base.TickDown(waitAmount);
    }
}
