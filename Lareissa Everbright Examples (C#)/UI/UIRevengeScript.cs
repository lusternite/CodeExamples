using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevengeScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private CombatManagerScript combatManagerReference;

    public float textChangeSpeed = 50.0f;

    public Image borderImageReference;

    public Sprite normalBorderSprite;

    public Sprite maximumBorderSprite;

    private Text textReference;

    private float currentMeterValue;

    private bool maximumChargeFlag;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        combatManagerReference = FindObjectOfType<CombatManagerScript>();
        textReference = GetComponent<Text>();
        currentMeterValue = combatManagerReference.revengeMeter;
        textReference.text = "Revenge: " + Mathf.Round(currentMeterValue).ToString() + " / 100";
    }
	
	// Update is called once per frame
	void Update () {
        if (currentMeterValue != combatManagerReference.revengeMeter)
        {
            float meterChange = Time.deltaTime * textChangeSpeed;
            if (Mathf.Abs(currentMeterValue - combatManagerReference.revengeMeter) > 15.0f)
            {
                meterChange *= 4.0f;
            }
            if (Mathf.Abs(currentMeterValue - combatManagerReference.revengeMeter) < meterChange)
            {
                currentMeterValue = combatManagerReference.revengeMeter;
                if (currentMeterValue == 100.0f)
                {
                    FindObjectOfType<AudioManagerScript>().PlayCombatSFX("RevengeFilled");

                    // Play particle effect
                    GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                currentMeterValue += Mathf.Sign(combatManagerReference.revengeMeter - currentMeterValue) * meterChange;
            }

            textReference.text = "Revenge: " + Mathf.Floor(currentMeterValue).ToString() + " / 100";

            if (maximumChargeFlag == false)
            {
                if (currentMeterValue == 100.0f)
                {
                    borderImageReference.sprite = maximumBorderSprite;
                    maximumChargeFlag = true;
                }
            }
            else
            {
                borderImageReference.sprite = normalBorderSprite;
                maximumChargeFlag = false;
            }
        }
    }

    public float GetMeterValue()
    {
        return currentMeterValue;
    }
}
