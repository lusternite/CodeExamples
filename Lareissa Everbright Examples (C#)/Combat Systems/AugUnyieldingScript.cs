using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugUnyieldingScript : AugmentScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    [Tooltip("The amount of times death can be resisted")]
    public int unyieldingCharges = 3;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        augmentType = AugmentType.UNYIELDING;
        augmentDuration = 150.0f;
        augmentCharges = unyieldingCharges;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Handles resisting death, checks to see if this augment should be dispelled
    public void ProcessUndeath()
    {
        // Deduct unyielding charges
        augmentCharges -= 1;

        // Check if there are charges remaining
        if (augmentCharges <= 0)
        {
            // Remove this augment
            augmentedEntityReference.RemoveAugment(AugmentType.UNYIELDING);
        }
    }

    override public void ResetDuration()
    {
        augmentDuration = 150.0f;
    }

    override public void TickDown(float waitAmount)
    {
        // Allow turn to continue
        FindObjectOfType<CombatManagerScript>().SetCanTurnProceed(true);

        base.TickDown(waitAmount);
    }
}
