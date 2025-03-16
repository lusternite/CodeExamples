using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugVenomScript : AugmentScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // The base damage that venom does
    public float venomDamage = 5.0f;

    // The multiplier for the damage after each tick
    public float venomScaling = 2.1f;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        augmentType = AugmentType.VENOM;
        augmentDuration = 120.0f;
        augmentDamage = 5.0f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Deals damage that increases every turn.
    override public void TickDown(float waitAmount)
    {
        // Damage the entity
        augmentedEntityReference.InflictDamageEntity("The venom courses through", augmentDamage);
        // Scale the damage for next time
        augmentDamage *= venomScaling;

        // Round the damage
        augmentDamage = Mathf.Round(augmentDamage);

        base.TickDown(waitAmount);
    }

    override public void ResetDuration()
    {
        augmentDuration = 120.0f;
    }
}
