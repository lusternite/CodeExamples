using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeManagerScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public bool narrationFlag = false;

    public bool nightFlag = false;

    public int narrationParagraphIndex;

    // Used to track input
    private bool confirmButtonPressed;

    // Used to determine if the final boss dialog has been branched or not
    public bool dialogBranched = false;

    // Used to determine if player is on route a or b
    public bool routeA = false;

    public float narrativeInputDelay;

    public float travelSpriteRevealDuration = 0.5f;

    public float karmillionRevealDuration = 0.5f;

    public float whiteFlashDuration = 0.8f;

    public float portraitSwapDuration = 0.2f;
    public float portraitSwapX = -15.0f;
    private Sprite newPortraitSprite;

    private UITextUnfoldScript scenarioParagraphTextReference;

    private Image portraitImageReference;
    private Sprite currentPortraitSprite;

    private GameManagerScript gameManagerReference;
    private AudioManagerScript audioManagerReference;

    public GameObject badEndPrefab;

    [Header("Portrait sprite references")]
    public Sprite gwenSprite;
    public Sprite noneSprite;
    public Sprite prixtonSprite;
    public Sprite gwenHelmlessSprite;
    public Sprite karmillionSprite;
    public Sprite karmillionDivineSprite;

    [Header("Background sprite references")]
    public Sprite forestTravelSprite;
    public Sprite forestNightTravelSprite;
    public Sprite forestBattleSprite;
    public Sprite forestNightBattleSprite;
    public Sprite plainsTravelSprite;
    public Sprite plainsNightTravelSprite;
    public Sprite plainsBattleSprite;
    public Sprite plainsNightBattleSprite;
    public Sprite swampTravelSprite;
    public Sprite swampBattleSprite;

    [Header("Travel sprite references")]
    public Sprite gwenRidingPrixtonSprite;
    public Sprite gwenCampingSprite;
    public Sprite gwenBattleSprite;
    public Sprite karmillionBattleSprite;
    public Sprite karmillionDivineBattleSprite;

    [Header("Scenario paragraph info")]

    public List<string> narrativeScenarioParagraphs;

    public List<Sprite> narrativeScenarioPortraits;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        gameManagerReference = GetComponent<GameManagerScript>();
        audioManagerReference = GetComponent<AudioManagerScript>();
        currentPortraitSprite = noneSprite;
    }
	
	// Update is called once per frame
	void Update () {
        // Check for button press to continue the dialog
		if (narrationFlag == true)
        {
            if (Input.GetAxisRaw("Confirm") > 0.0f)
            {
                if (confirmButtonPressed == false && Time.timeScale == 1.0f && narrativeInputDelay <= 0.0f)
                {
                    // Handle the dialog change but first test for pausing
                    Invoke("PauseTest", 0.1f);
                    confirmButtonPressed = true;
                }
            }
            else if (confirmButtonPressed == true)
            {
                confirmButtonPressed = false;
            }
        }

        if (narrativeInputDelay > 0.0f)
        {
            narrativeInputDelay -= Time.deltaTime;
        }
	}

    public void InitialiseNarrativeScene(string narrativeID)
    {
        narrationFlag = true;

        // Get the paragraph object
        scenarioParagraphTextReference = FindObjectOfType<UITextUnfoldScript>();

        // Get the portrait object
        portraitImageReference = GameObject.Find("PortraitImage").GetComponent<Image>();

        narrationParagraphIndex = 0;

        // Load paragraphs for this scenario
        CSVReaderScript.ReadNarrativeData(narrativeID, this);
    }

    public void ResetNarrativeVariables()
    {
        narrationFlag = false;
        dialogBranched = false;
        narrationParagraphIndex = 0;

        narrativeScenarioParagraphs.Clear();
        narrativeScenarioPortraits.Clear();

        StopAllCoroutines();

        // Double check for route here
        if (gameManagerReference.currentAct == 4 && gameManagerReference.currentChapter == 1 && dialogBranched == false)
        {
            // Then figure out if its branch a or b
            if (FindObjectOfType<PlayerManagerScript>().GetSpecifiedEquipmentDurability("Everlustre") < 20)
            {
                // Everlustre has been used, go to route a
                routeA = true;
            }
            else
            {
                // Everlustre has not been used, go to true route b
                routeA = false;
                // Remove the everlustre from inventory
                FindObjectOfType<PlayerManagerScript>().RemoveEverlustreFromEquipmentInventory();
            }
        }
    }

    private void PauseTest()
    {
        // Make sure isn't paused before continuing
        if (Time.timeScale == 1.0f && narrativeInputDelay <= 0.0f && narrationFlag == true)
        {
            // Check if ending dialog or not
            if (gameManagerReference.currentAct == 5)
            {
                HandleEndingDialogChange();
            }
            else
            {
                HandleDialogChange();
            }
        }
    }

    private void HandleDialogChange()
    {
        // Check to see if text has been unfolded
        if (scenarioParagraphTextReference.IsUnfoldComplete() == true)
        {
            narrationParagraphIndex += 1;

            // Check to see if this is the last paragraph
            if (narrationParagraphIndex == narrativeScenarioParagraphs.Count)
            {
                // Check for branching dialog
                if (gameManagerReference.currentAct == 4 && gameManagerReference.currentChapter == 1 && dialogBranched == false)
                {
                    // Reset variables
                    ResetNarrativeVariables();

                    // Then figure out if its branch a or b
                    if (FindObjectOfType<PlayerManagerScript>().GetSpecifiedEquipmentDurability("Everlustre") < 20)
                    {
                        // Everlustre has been used, go to route a
                        InitialiseNarrativeScene("4-1a");
                        routeA = true;
                    }
                    else
                    {
                        // Everlustre has not been used, go to true route b
                        InitialiseNarrativeScene("4-1b");
                        routeA = false;
                    }

                    // Make sure to turn flag on
                    dialogBranched = true;

                    // Then display next paragraph
                    DisplayParagraph();

                    
                }

                else
                {
                    // Load the sortie scene
                    GetComponent<GameManagerScript>().LoadSortieScene();

                    GetComponent<GameManagerScript>().SavePlayerProgressionData();

                    // No longer narrating
                    narrationFlag = false;
                }
                
            }
            // Otherwise load the next paragraph
            else
            {
                DisplayParagraph();
            }

            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("NarrativeShortClick");

        }
        // If it hasn't then unfold all immediately
        else
        {
            scenarioParagraphTextReference.ForceUnfoldAll();
        }
    }

    private void HandleEndingDialogChange()
    {
        // Check to see if text has been unfolded
        if (scenarioParagraphTextReference.IsUnfoldComplete() == true)
        {
            narrationParagraphIndex += 1;

            // Check to see if this is the last paragraph
            if (narrationParagraphIndex == narrativeScenarioParagraphs.Count)
            {
                // Check if this is bad end
                if (gameManagerReference.currentChapter == 1)
                {
                    // Spawn the bad end screen
                    Instantiate(badEndPrefab, GameObject.Find("Canvas").transform);

                    // Play defeat sfx
                    audioManagerReference.PlayCombatSFX("Defeat");

                    // Stop narrating
                    narrationFlag = false;

                    // Reset act and chapter
                    gameManagerReference.currentAct = 4;
                    gameManagerReference.currentChapter = 1;

                    // Unlock achievement
                    GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_BAD_END");
                }
                // Or maybe good end
                else if (gameManagerReference.currentChapter == 2)
                {
                    FindObjectOfType<UICreditsAnimationScript>().StartCredits();
                    // Reset act and chapter
                    gameManagerReference.currentAct = 4;
                    gameManagerReference.currentChapter = 1;

                    // Unlock achievement
                    GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_GOOD_END");
                }
                // Otherwise its the true end
                else
                {
                    FindObjectOfType<UICreditsAnimationScript>().StartCredits();
                    // Reset act and chapter
                    gameManagerReference.currentAct = 4;
                    gameManagerReference.currentChapter = 2;

                    // Unlock achievement
                    GetComponent<AchievementManagerScript>().UnlockAchievement("ACH_TRUE_END");
                }

                // Stop narrating
                narrationFlag = false;

                // Reset the text
                scenarioParagraphTextReference.HideText();
            }
            // Otherwise load the next paragraph
            else
            {
                DisplayParagraph();
            }

            // Play click sfx
            FindObjectOfType<AudioManagerScript>().PlayUISFX("NarrativeShortClick");

        }
    }

    public void DisplayParagraph()
    {
        // Change portrait if not same
        if (narrativeScenarioPortraits[narrationParagraphIndex] != portraitImageReference.sprite)
        {
            // Stop previous coroutine if its active
            StopCoroutine("SwapPortrait");

            newPortraitSprite = narrativeScenarioPortraits[narrationParagraphIndex];
            StartCoroutine("SwapPortrait");
        }

        // Check for sprite changes if not index 0
        if (narrationParagraphIndex != 0)
        {
            HandleParagraphSpriteChanges();
        }

        // Change text
        scenarioParagraphTextReference.SetText(narrativeScenarioParagraphs[narrationParagraphIndex]);

        // Reset the text
        scenarioParagraphTextReference.HideText();

        // Display text
        scenarioParagraphTextReference.ShowText();
    }

    // Used to fade in the travelling sprite
    public float RevealTravellingSprite()
    {
        // Start the fade coroutine
        StartCoroutine(TravelSpriteFadeIn());

        // Return the fade duration
        return travelSpriteRevealDuration;
    }

    private IEnumerator TravelSpriteFadeIn()
    {
        // Make sprite black first
        Image gwenReference = GameObject.Find("Gwen").GetComponent<Image>();
        gwenReference.color = new Color(0.0f, 0.0f, 0.0f);

        float animationColour = 0.0f;

        // Then go ahead and start fading out
        while (animationColour < 1.0f)
        {
            animationColour += Time.deltaTime / travelSpriteRevealDuration;
            gwenReference.color = new Color(animationColour, animationColour, animationColour);
            yield return new WaitForEndOfFrame();
        }

        // Finish off
        yield return null;
    }

    private IEnumerator SwapPortrait()
    {
        // Start with fading out and moving left the current sprite
        float animationAlpha = 1.0f;
        RectTransform portraitRectTransformReference = portraitImageReference.GetComponent<RectTransform>();
        float animationPosX = portraitRectTransformReference.anchoredPosition.x;
        float animationEndPosX = portraitRectTransformReference.anchoredPosition.x + portraitSwapX;
        float animationShiftSpeed = portraitSwapX / portraitSwapDuration;

        // Check that its not none sprite first, or skip this step
        if (currentPortraitSprite != noneSprite)
        {
            // Start to fade this sprite and at the same time move the sprite
            
            while (animationAlpha > 0.0f)
            {
                // Alpha
                animationAlpha -= Time.deltaTime / portraitSwapDuration;
                portraitImageReference.color = new Color(portraitImageReference.color.r, portraitImageReference.color.g, portraitImageReference.color.b, animationAlpha);

                // Position
                animationPosX += animationShiftSpeed * Time.deltaTime;
                portraitRectTransformReference.anchoredPosition = new Vector3(animationPosX, portraitRectTransformReference.anchoredPosition.y);
                yield return new WaitForEndOfFrame();
            }
        }

        // Make sure to properly set the alpha and pos before next phase
        portraitRectTransformReference.anchoredPosition = new Vector3(animationEndPosX, portraitRectTransformReference.anchoredPosition.y);
        portraitImageReference.color = new Color(portraitImageReference.color.r, portraitImageReference.color.g, portraitImageReference.color.b, 0);

        animationAlpha = 0.0f;
        animationPosX = animationEndPosX;
        animationShiftSpeed *= -1.0f;

        portraitImageReference.sprite = newPortraitSprite;

        // Check that next is not none sprite, or skip this step
        if (newPortraitSprite != noneSprite)
        {
            // Start to fade this sprite and at the same time move the sprite

            while (animationAlpha < 1.0f)
            {
                // Alpha
                animationAlpha += Time.deltaTime / portraitSwapDuration;
                portraitImageReference.color = new Color(portraitImageReference.color.r, portraitImageReference.color.g, portraitImageReference.color.b, animationAlpha);

                // Position
                animationPosX += animationShiftSpeed * Time.deltaTime;
                portraitRectTransformReference.anchoredPosition = new Vector3(animationPosX, portraitRectTransformReference.anchoredPosition.y);
                yield return new WaitForEndOfFrame();
            }
        }

        // Make sure to properly set the alpha and pos before finishing
        portraitRectTransformReference.anchoredPosition = new Vector3(animationEndPosX - portraitSwapX, portraitRectTransformReference.anchoredPosition.y);
        portraitImageReference.color = new Color(portraitImageReference.color.r, portraitImageReference.color.g, portraitImageReference.color.b, 1);

        currentPortraitSprite = newPortraitSprite;

        yield return null;
    }

    public void AddScenarioPortrait(string portraitName)
    {
        if (portraitName == "Gwen")
        {
            narrativeScenarioPortraits.Add(gwenSprite);
        }
        else if (portraitName == "None")
        {
            narrativeScenarioPortraits.Add(noneSprite);
        }
        else if (portraitName == "Prixton")
        {
            narrativeScenarioPortraits.Add(prixtonSprite);
        }
        else if (portraitName == "GwenHelmless")
        {
            narrativeScenarioPortraits.Add(gwenHelmlessSprite);
        }
        else if (portraitName == "Karmillion")
        {
            narrativeScenarioPortraits.Add(karmillionSprite);
        }
        else if (portraitName == "KarmillionDivine")
        {
            narrativeScenarioPortraits.Add(karmillionDivineSprite);
        }
    }

    public void AddScenarioParagraph(string paragraphText)
    {
        narrativeScenarioParagraphs.Add(paragraphText);
    }

    // Hard coded
    public void HandleParagraphSpriteChanges()
    {
        // Scenario 1-1
        if (gameManagerReference.currentAct == 1 && gameManagerReference.currentChapter == 1)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to night
                GameObject.Find("Background").GetComponent<Image>().sprite = forestNightTravelSprite;

                // Change gwen sprite to camping
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenCampingSprite;

                nightFlag = true;
            }
        }

        // Scenario 1-2
        if (gameManagerReference.currentAct == 1 && gameManagerReference.currentChapter == 2)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to night
                GameObject.Find("Background").GetComponent<Image>().sprite = forestNightTravelSprite;

                // Change gwen sprite to camping
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenCampingSprite;
            }
            else if (narrationParagraphIndex == 6)
            {
                // Change background to day
                GameObject.Find("Background").GetComponent<Image>().sprite = forestTravelSprite;

                // Change gwen sprite to riding
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenRidingPrixtonSprite;

                nightFlag = false;
            }
            
        }

        // Scenario 2-1
        if (gameManagerReference.currentAct == 2 && gameManagerReference.currentChapter == 1)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to night
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsNightTravelSprite;

                // Change gwen sprite to camping
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenCampingSprite;

                nightFlag = true;
            }
        }

        // Scenario 2-2
        if (gameManagerReference.currentAct == 2 && gameManagerReference.currentChapter == 2)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to night
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsNightTravelSprite;

                // Change gwen sprite to camping
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenCampingSprite;
            }
            else if (narrationParagraphIndex == 6)
            {
                // Change background to day
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsTravelSprite;

                // Change gwen sprite to riding
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenRidingPrixtonSprite;

                nightFlag = false;
            }
        }

        // Scenario 2-4
        if (gameManagerReference.currentAct == 2 && gameManagerReference.currentChapter == 4)
        {
            if (narrationParagraphIndex == 10)
            {
                // Change background to night
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsNightTravelSprite;

                // Change gwen sprite to camping
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenCampingSprite;

                nightFlag = true;
            }
        }

        // Scenario 3-1
        if (gameManagerReference.currentAct == 3 && gameManagerReference.currentChapter == 1)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to plains night
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsNightTravelSprite;

                // Change gwen sprite to camping
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenCampingSprite;

                // Also change gwen's position
                GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 36.0f);

                nightFlag = true;
            }
            else if (narrationParagraphIndex == 9)
            {
                // Change gwen sprite to travelling
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenRidingPrixtonSprite;
            }
            else if (narrationParagraphIndex == 10)
            {
                // Change background to swamp
                GameObject.Find("Background").GetComponent<Image>().sprite = swampTravelSprite;

                // Also change gwen's position
                GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(480.0f, 120.0f);

                // Start swamp bgm
                audioManagerReference.PlaySwampBGM();
            }
        }

        // Scenario 4-1
        if (gameManagerReference.currentAct == 4 && gameManagerReference.currentChapter == 1 && dialogBranched == false)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to swamp
                GameObject.Find("Background").GetComponent<Image>().sprite = swampTravelSprite;

                // Also change gwen's position
                GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(480.0f, 120.0f);

                // Play swamp bgm
                audioManagerReference.PlaySwampBGM();
            }
            else if (narrationParagraphIndex == 3)
            {
                // Fade out bgm
                audioManagerReference.FadeOutBGM();
            }
            else if (narrationParagraphIndex == 4)
            {
                // Play fort bgm
                audioManagerReference.PlayFortBGM();

                // Change background to fort
                GameObject.Find("Background").GetComponent<Image>().sprite = gameManagerReference.fortSprite;

                // Also change gwen's position
                GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(-594f, 46.0f);
            }
            else if (narrationParagraphIndex == 5)
            {
                // Reveal karmillion
                StartCoroutine(KarmillionRevealCoroutine());

                // Stop narrative from continuing until coroutine is done
                narrationFlag = false;
            }
            else if (narrationParagraphIndex == 8)
            {
                // Change gwen's sprite to combat
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenBattleSprite;

                // Change size
                GameObject.Find("Gwen").GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);

                // Change position
                GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(-490.0f, 100.0f);
            }
        }

        // Scenario 4-1a
        if (gameManagerReference.currentAct == 4 && gameManagerReference.currentChapter == 1 && dialogBranched == true && routeA == true)
        {
            if (narrationParagraphIndex == 24)
            {
                // Change karmillion's sprite to combat
                GameObject.Find("Karmillion").GetComponent<Image>().sprite = karmillionBattleSprite;
            }
        }

        // Scenario 4-1b
        if (gameManagerReference.currentAct == 4 && gameManagerReference.currentChapter == 1 && dialogBranched == true && routeA == false)
        {
            if (narrationParagraphIndex == 15)
            {
                // Change karmillion's sprite to combat
                GameObject.Find("Karmillion").GetComponent<Image>().sprite = karmillionBattleSprite;
            }
        }

        // Scenario 4-2
        if (gameManagerReference.currentAct == 4 && gameManagerReference.currentChapter == 2)
        {
            if (narrationParagraphIndex == 0)
            {
                // Change background to fort
                GameObject.Find("Background").GetComponent<Image>().sprite = gameManagerReference.fortSprite;

                // Change gwen's sprite to combat
                GameObject.Find("Gwen").GetComponent<Image>().sprite = gwenBattleSprite;

                // Change size
                GameObject.Find("Gwen").GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);

                // Change position
                GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(-490.0f, 100.0f);

                // Play fort bgm
                audioManagerReference.PlayFortBGM();

                // Reveal Karmillion
                StartCoroutine(KarmillionRevealCoroutine());

                // Change karmillion's sprite to combat
                GameObject.Find("Karmillion").GetComponent<Image>().sprite = karmillionBattleSprite;

                // Remove the everlustre from inventory
                FindObjectOfType<PlayerManagerScript>().RemoveEverlustreFromEquipmentInventory();
            }
            if (narrationParagraphIndex == 7)
            {
                // Start grim divine flash coroutine
                StartCoroutine(GrimDivineRevealCoroutine());

                // Play sfx
                audioManagerReference.PlayEntitySFX("ReplicateArsenal");

                // Stop narrative from continuing until coroutine is done
                narrationFlag = false;
            }
        }
        // Scenario 5-2 Good End
        if (gameManagerReference.currentAct == 5 && gameManagerReference.currentChapter == 2)
        {
            if (narrationParagraphIndex == 16)
            {
                // Play forest bgm
                audioManagerReference.PlayForestBGM();
            }
            else if (narrationParagraphIndex == 22)
            {
                // Fade out bgm
                audioManagerReference.FadeOutBGM();
            }
        }
        // Scenario 5-3 True End
        if (gameManagerReference.currentAct == 5 && gameManagerReference.currentChapter == 3)
        {
            if (narrationParagraphIndex == 1)
            {
                // Play fort bgm
                audioManagerReference.PlayFortBGM();
            }
            else if (narrationParagraphIndex == 8)
            {
                // Fade out bgm
                audioManagerReference.FadeOutBGM();
            }
            else if (narrationParagraphIndex == 17)
            {
                // Play credits bgm
                audioManagerReference.PlayCreditsBGM();
            }
        }
    }

    public IEnumerator KarmillionRevealCoroutine()
    {
        Image karmillionReference = GameObject.Find("Karmillion").GetComponent<Image>();

        karmillionReference.color = new Color(0, 0, 0, 1);

        float animationColour = 0.0f;

        // Then go ahead and start fading in
        while (animationColour < 1.0f)
        {
            animationColour += Time.deltaTime / travelSpriteRevealDuration;
            karmillionReference.color = new Color(animationColour, animationColour, animationColour);
            yield return new WaitForEndOfFrame();
        }

        // Finish off

        // Allow narration to continue
        narrationFlag = true;

        yield return null;
    }

    public IEnumerator GrimDivineRevealCoroutine()
    {
        Image karmillionReference = GameObject.Find("Karmillion").GetComponent<Image>();
        Image whiteFlashRefreence = GameObject.Find("WhiteFlash").GetComponent<Image>();

        float animationColour = 0.0f;

        // Then go ahead and start fading in
        while (animationColour < 1.0f)
        {
            if (animationColour > 0.6f)
            {
                animationColour += Time.deltaTime / whiteFlashDuration * 2.0f;
            }
            else
            {
                animationColour += Time.deltaTime / whiteFlashDuration;
            }
            whiteFlashRefreence.color = new Color(1, 1, 1, animationColour);
            yield return new WaitForEndOfFrame();
        }

        // Change stuff while the flash is at max

        // Change karmillion's sprite to grim divine
        karmillionReference.sprite = karmillionDivineBattleSprite;

        // Change karmillion's position
        karmillionReference.GetComponent<RectTransform>().anchoredPosition = new Vector3(520.0f, 175.0f);

        // Change size
        karmillionReference.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 450);

        animationColour = 1.0f;

        // Start fading out flash
        while (animationColour > 0.0f)
        {
            animationColour -= Time.deltaTime / whiteFlashDuration;
            whiteFlashRefreence.color = new Color(1, 1, 1, animationColour);
            yield return new WaitForEndOfFrame();
        }

        // Finish off

        // Allow narration to continue
        narrationFlag = true;

        yield return null;
    }
}
