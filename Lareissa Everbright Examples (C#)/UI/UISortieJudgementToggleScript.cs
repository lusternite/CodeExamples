using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISortieJudgementToggleScript : MonoBehaviour, IPointerEnterHandler {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private CombatManagerScript combatManagerReference;

    private Button judgementButtonReference;

    private Image judgementImageReference;

    public Sprite normalJudgementSprite;

    public Sprite normalJudgementHighlightSprite;

    public Sprite activatedJudgementSprite;

    public Sprite activatedJudgementHighlightSprite;

    private SpriteState normalSpriteState;

    private SpriteState activatedSpriteState;

    private bool judgementActiveFlag;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        combatManagerReference = FindObjectOfType<CombatManagerScript>();
        judgementButtonReference = GetComponent<Button>();
        judgementImageReference = GetComponent<Image>();

        // Set up sprite states
        activatedSpriteState = new SpriteState();
        activatedSpriteState.highlightedSprite = activatedJudgementHighlightSprite;
        activatedSpriteState.pressedSprite = activatedJudgementHighlightSprite;

        normalSpriteState = judgementButtonReference.spriteState;
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
        if (FindObjectOfType<CombatManagerScript>().judgementFlag == true)
        {
            FindObjectOfType<CombatManagerScript>().SetJudgementState(false);

            ResetJudgementBorderSprites();

            // Stop button selection
            EventSystem.current.SetSelectedGameObject(null);

            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("JudgementButtonClickOff");
        }
        else
        {
            FindObjectOfType<CombatManagerScript>().SetJudgementState(true);

            SetActivatedJudgementBorderSprites();

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
}
