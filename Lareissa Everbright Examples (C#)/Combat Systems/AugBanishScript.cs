using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugBanishScript : AugmentScript
{
    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public EquipmentBaseScript equipmentReference;


    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start()
    {
        augmentType = AugmentType.BANISH;
        augmentDuration = 80.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    override public void ResetDuration()
    {
        augmentDuration = 80.0f;
    }

    override public void TickDown(float waitAmount)
    {
        // Allow turn to continue
        FindObjectOfType<CombatManagerScript>().SetCanTurnProceed(true);

        base.TickDown(waitAmount);
    }
}
