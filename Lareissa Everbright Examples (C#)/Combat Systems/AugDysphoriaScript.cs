using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugDysphoriaScript : AugmentScript
{
    // Use this for initialization
    void Start()
    {
        augmentType = AugmentType.DYSPHORIA;
        augmentDuration = 40.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    override public void ResetDuration()
    {
        augmentDuration = 40.0f;
    }

    override public void TickDown(float waitAmount)
    {
        // Allow turn to continue
        FindObjectOfType<CombatManagerScript>().SetCanTurnProceed(true);

        base.TickDown(waitAmount);
    }
}
