using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AugmentType
{
    BLIND,
    VENOM,
    BLOCK,
    STUN,
    UNYIELDING,
    BLEED,
    BANISH,
    DYSPHORIA,
    COUNTER
}

// This is a base class for all augments in the game.
public class AugmentScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // The type of the augment, kinda like an ID
    public AugmentType augmentType;

    // How long the augment lasts in WT.
    public float augmentDuration;

    // How much damage this augment does if it does damage
    public float augmentDamage;

    // How many charges this augment has if it does have charges
    public int augmentCharges;

    // A reference to the entity that this augment is attached to
    protected EntityBaseScript augmentedEntityReference;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Instantiate(EntityBaseScript entityReference)
    {
        augmentedEntityReference = entityReference;
    }

    // What happens during the augmented's turn
    public virtual void TickDown(float waitAmount)
    {
        augmentDuration -= waitAmount;
    }

    public virtual void ResetDuration()
    {
        augmentDuration = 0.0f;
    }

    public void DestroyAugment()
    {
        Destroy(this, 0.5f);
    }
}
