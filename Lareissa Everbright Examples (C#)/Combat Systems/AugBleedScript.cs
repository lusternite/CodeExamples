using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugBleedScript : AugmentScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public float bleedDamage = 12.0f;


    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start()
    {
        augmentType = AugmentType.BLEED;
        augmentDuration = 75.0f;
        augmentDamage = 12.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    override public void ResetDuration()
    {
        augmentDuration = 75.0f;
    }

    // Deals damage at the start of every turn
    override public void TickDown(float waitAmount)
    {
        // Damage the entity
        augmentedEntityReference.InflictDamageEntity("Pain springs from a deep gash", augmentDamage);

        base.TickDown(waitAmount);
    }
}
