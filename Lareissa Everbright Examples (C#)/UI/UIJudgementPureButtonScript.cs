using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIJudgementPureButtonScript : MonoBehaviour, IPointerEnterHandler {

    private CombatManagerScript combatManagerReference;

    private UIJudgementScript otherJudgementUIReference;

    private Button judgementButtonReference;

    private Image judgementImageReference;

    public Sprite normalJudgementSprite;

    public Sprite normalJudgementHighlightSprite;

    public Sprite activatedJudgementSprite;

    public Sprite activatedJudgementHighlightSprite;

    private SpriteState normalSpriteState;

    private SpriteState activatedSpriteState;

    private Color previewJudgementColor;

    // Use this for initialization
    void Start () {
        combatManagerReference = FindObjectOfType<CombatManagerScript>();
        otherJudgementUIReference = FindObjectOfType<UIJudgementScript>();
        judgementButtonReference = GetComponent<Button>();
        judgementImageReference = GetComponent<Image>();

        // Set up sprite states
        activatedSpriteState = new SpriteState();
        activatedSpriteState.highlightedSprite = activatedJudgementHighlightSprite;
        activatedSpriteState.pressedSprite = activatedJudgementHighlightSprite;

        normalSpriteState = judgementButtonReference.spriteState;

        // Turn image gray
        previewJudgementColor = new Color(0.7f, 0.7f, 0.7f);
        judgementImageReference.color = previewJudgementColor;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play hover sfx
        FindObjectOfType<AudioManagerScript>().PlayUISFX("EquipmentButtonHover");
    }

    // What to do when the judgement button is pressed
    public void ToggleJudgementState()
    {
        // Flip the judgement flag
        if (combatManagerReference.judgementFlag == true)
        {
            combatManagerReference.SetJudgementState(false);

            ResetJudgementBorderSprites();

            if (combatManagerReference.judgementMeter == 100.0f)
            {
                // Change sprites for other judgement ui
                otherJudgementUIReference.ResetJudgementBorderSprites();
            }
            else
            {
                // Turn on interactivity for all equipment buttons since judgement isn't charged.
                UIEquipmentInformationScript[] combatEquipment = FindObjectsOfType<UIEquipmentInformationScript>();

                for (int i = 0; i < 4; i++)
                {
                    combatEquipment[i].GetComponent<Button>().interactable = true;
                }
            }

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);

            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("JudgementButtonClickOff");
        }
        else
        {
            combatManagerReference.SetJudgementState(true);

            SetActivatedJudgementBorderSprites();

            if (combatManagerReference.judgementMeter == 100.0f)
            {
                // Change sprites for other judgement ui
                otherJudgementUIReference.SetActivatedJudgementBorderSprites();
            }
            else
            {
                // Turn off interactivity for all equipment buttons since judgement isn't charged.
                UIEquipmentInformationScript[] combatEquipment = FindObjectsOfType<UIEquipmentInformationScript>();

                for (int i = 0; i < 4; i++)
                {
                    combatEquipment[i].GetComponent<Button>().interactable = false;
                    print("DISABLED");
                }
            }

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);

            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("JudgementButtonClick");
        }
    }

    public void SetActivatedJudgementBorderSprites()
    {
        // Change the border sprites to activated
        judgementImageReference.sprite = activatedJudgementSprite;
        judgementButtonReference.spriteState = activatedSpriteState;
    }

    public void ResetJudgementBorderSprites()
    {
        // Change the sprites back to normal
        judgementImageReference.sprite = normalJudgementSprite;
        judgementButtonReference.spriteState = normalSpriteState;
    }

    public void SetJudgementButtonColor(bool judgementCharged)
    {
        if (judgementCharged)
        {
            judgementImageReference.color = Color.white;
        }
        else
        {
            judgementImageReference.color = previewJudgementColor;
        }
    }
}
