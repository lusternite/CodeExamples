using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    DMG,
    DEF,
    SPD,
    ACC,
    FTH
}

public class ModifierScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // Name of the modifier, used to determine the stat
    public StatType modifierType;

    // How long the modifier lasts in WT.
    public float modifierDuration;

    // How much % this modifier gives
    public float modifierValue;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void TickDown(float waitAmount)
    {
        modifierDuration -= waitAmount;
    }

    public virtual void Instantiate(StatType statType, float value)
    {
        modifierType = statType;
        modifierValue = value;
        modifierDuration = GetModifierDuration();
    }

    public void ResetDuration()
    {
        modifierDuration = GetModifierDuration();
    }

    public void DestroyModifier()
    {
        Destroy(this, 0.5f);
    }

    // Determines the standard duration for each of the modifier types
    private float GetModifierDuration()
    {
        if (modifierType == StatType.ACC)
        {
            return 90.0f;
        }
        else if (modifierType == StatType.DEF)
        {
            return 120.0f;
        }
        else if (modifierType == StatType.DMG)
        {
            return 80.0f;
        }
        else if (modifierType == StatType.FTH)
        {
            return 75.0f;
        }
        else if (modifierType == StatType.SPD)
        {
            return 100.0f;
        }
        return 0;
    }
}
