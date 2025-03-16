using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : EntityBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//



    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {

    }

    // Determine the amount of judgment scaling from missing health
    public float GetJudgementHealthScaling()
    {
        if (health >= 90.0f)
        {
            return 1.0f;
        }
        else if (health >= 70.0f)
        {
            return 1.15f;
        }
        else if (health >= 62.0f)
        {
            return 1.30f;
        }
        else if (health >= 45.0f)
        {
            return 1.5f;
        }
        else if (health >= 15.0f)
        {
            return 1.7f;
        }
        else
        {
            return 2.0f;
        }
    }
}
