using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIJudgementScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private CombatManagerScript combatManagerReference;

    private UIJudgementPureButtonScript otherJudgementUIReference;

    public float textChangeSpeed = 45.0f;

    private Text textReference;

    private float currentMeterValue;

    private Image judgementBorderImageReference;

    private Button judgementButtonReference;

    public Sprite normalJudgementBorderSprite;

    public Sprite normalJudgementBorderHighlightSprite;

    public Sprite activatedJudgementBorderSprite;

    public Sprite activatedJudgementBorderHighlightSprite;

    private SpriteState normalSpriteState;

    private SpriteState activatedSpriteState;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        combatManagerReference = FindObjectOfType<CombatManagerScript>();
        otherJudgementUIReference = FindObjectOfType<UIJudgementPureButtonScript>();
        textReference = GetComponent<Text>();
        currentMeterValue = combatManagerReference.judgementMeter;
        textReference.text = "Judgement: " + Mathf.Round(currentMeterValue).ToString() + " / 100";
        judgementBorderImageReference = GetComponentInParent<Image>();
        judgementButtonReference = GetComponentInParent<Button>();

        // Set up sprite states
        activatedSpriteState = new SpriteState();
        activatedSpriteState.highlightedSprite = activatedJudgementBorderHighlightSprite;
        activatedSpriteState.pressedSprite = activatedJudgementBorderHighlightSprite;

        normalSpriteState = judgementButtonReference.spriteState;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentMeterValue != combatManagerReference.judgementMeter)
        {
            float meterChange = Time.deltaTime * textChangeSpeed;
            if (Mathf.Abs(currentMeterValue - combatManagerReference.judgementMeter) > 15.0f)
            {
                meterChange *= 4.0f;
            }
            if (Mathf.Abs(currentMeterValue - combatManagerReference.judgementMeter) < meterChange)
            {
                currentMeterValue = combatManagerReference.judgementMeter;
                if (currentMeterValue == 100.0f)
                {
                    FindObjectOfType<AudioManagerScript>().PlayCombatSFX("JudgementFilled");

                    // Also play particles
                    GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                currentMeterValue += Mathf.Sign(combatManagerReference.judgementMeter - currentMeterValue) * meterChange;
            }

            textReference.text = "Judgement: " + Mathf.Floor(currentMeterValue).ToString() + " / 100";
        }
    }

    // What to do when the judgement button is pressed
    public void ToggleJudgementState()
    {
        // Flip the judgement flag
        if (combatManagerReference.judgementFlag == true)
        {
            combatManagerReference.SetJudgementState(false);

            ResetJudgementBorderSprites();

            // Change sprites for other judgement ui
            otherJudgementUIReference.ResetJudgementBorderSprites();

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            combatManagerReference.SetJudgementState(true);

            SetActivatedJudgementBorderSprites();

            // Change sprites for other judgement ui
            otherJudgementUIReference.SetActivatedJudgementBorderSprites();

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SetActivatedJudgementBorderSprites()
    {
        // Change the border sprites to activated
        judgementBorderImageReference.sprite = activatedJudgementBorderSprite;
        judgementButtonReference.spriteState = activatedSpriteState;
    }

    public void ResetJudgementBorderSprites()
    {
        // Change the sprites back to normal
        judgementBorderImageReference.sprite = normalJudgementBorderSprite;
        judgementButtonReference.spriteState = normalSpriteState;
    }

    public float GetMeterValue()
    {
        return currentMeterValue;
    }
}
