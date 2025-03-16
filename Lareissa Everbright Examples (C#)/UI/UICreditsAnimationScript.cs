using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsAnimationScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public GameObject lareissaSplashReference;
    public Text creditsTextReference;
    public Text line1TextReference;
    public Text line2TextReference;
    public Text line3TextReference;
    public Text line4TextReference;
    public Text line5TextReference;

    float animationAlpha = 0.0f;
    public float animationFadeRate = 2.5f;
    public float animationWaitRate = 5.0f;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartCredits()
    {
        StartCoroutine("CreditsCoroutine");
    }

    private IEnumerator CreditsCoroutine()
    {
        // Set splash alpha back to 1
        lareissaSplashReference.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        // Play credits music
        FindObjectOfType<AudioManagerScript>().PlayCreditsBGM();
        
        // Start the dissolve for splash
        lareissaSplashReference.GetComponent<Coffee.UIExtensions.UIDissolve>().Play();

        yield return new WaitForSeconds(5.0f);

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            creditsTextReference.color = new Color(creditsTextReference.color.r, creditsTextReference.color.g, creditsTextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade out
        yield return new WaitForSeconds(animationWaitRate);

        animationAlpha = 1.0f;

        // Start fading out the credits
        while (animationAlpha > 0.0f)
        {
            animationAlpha -= Time.deltaTime / animationFadeRate;
            creditsTextReference.color = new Color(creditsTextReference.color.r, creditsTextReference.color.g, creditsTextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait half the time until next text is ready
        yield return new WaitForSeconds(animationWaitRate);

        // Do each line of text fade in
        line1TextReference.text = "Special thanks";

        animationAlpha = 0.0f;

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line1TextReference.color = new Color(line1TextReference.color.r, line1TextReference.color.g, line1TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade in next line
        yield return new WaitForSeconds(animationWaitRate / 2.0f);

        line2TextReference.text = "My very supportive family";

        animationAlpha = 0.0f;

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line2TextReference.color = new Color(line2TextReference.color.r, line2TextReference.color.g, line2TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade in next line
        yield return new WaitForSeconds(animationWaitRate / 2.0f);

        line3TextReference.text = "Mark Croucher";

        animationAlpha = 0.0f;

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line3TextReference.color = new Color(line3TextReference.color.r, line3TextReference.color.g, line3TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade in next line
        yield return new WaitForSeconds(animationWaitRate / 2.0f);

        line4TextReference.text = "Josh Savage";

        animationAlpha = 0.0f;

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line4TextReference.color = new Color(line4TextReference.color.r, line4TextReference.color.g, line4TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade in next line
        yield return new WaitForSeconds(animationWaitRate / 2.0f);

        line5TextReference.text = "Mun Hou Yong";

        animationAlpha = 0.0f;

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line5TextReference.color = new Color(line5TextReference.color.r, line5TextReference.color.g, line5TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade out
        yield return new WaitForSeconds(animationWaitRate);

        animationAlpha = 1.0f;

        // Start fading out the credits
        while (animationAlpha > 0.0f)
        {
            animationAlpha -= Time.deltaTime / animationFadeRate;
            line1TextReference.color = new Color(line1TextReference.color.r, line1TextReference.color.g, line1TextReference.color.b, animationAlpha);
            line2TextReference.color = new Color(line2TextReference.color.r, line2TextReference.color.g, line2TextReference.color.b, animationAlpha);
            line3TextReference.color = new Color(line3TextReference.color.r, line3TextReference.color.g, line3TextReference.color.b, animationAlpha);
            line4TextReference.color = new Color(line4TextReference.color.r, line4TextReference.color.g, line4TextReference.color.b, animationAlpha);
            line5TextReference.color = new Color(line5TextReference.color.r, line5TextReference.color.g, line5TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait half the time until next text is ready
        yield return new WaitForSeconds(animationWaitRate);

        animationAlpha = 0.0f;

        // Change text to thanks for playing
        line2TextReference.text = "And thank you for playing.";

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line2TextReference.color = new Color(line2TextReference.color.r, line2TextReference.color.g, line2TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade in next line
        yield return new WaitForSeconds(animationWaitRate);

        animationAlpha = 0.0f;

        line4TextReference.text = "See you around.";

        // Start fading in the credits
        while (animationAlpha < 1.0f)
        {
            animationAlpha += Time.deltaTime / animationFadeRate;
            line4TextReference.color = new Color(line4TextReference.color.r, line4TextReference.color.g, line4TextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Wait until ready to fade out
        yield return new WaitForSeconds(animationWaitRate);

        animationAlpha = 1.0f;

        // Start fading out the credits and splash
        while (animationAlpha > 0.0f)
        {
            animationAlpha -= Time.deltaTime / animationFadeRate;
            line2TextReference.color = new Color(line2TextReference.color.r, line2TextReference.color.g, line2TextReference.color.b, animationAlpha);
            line4TextReference.color = new Color(line4TextReference.color.r, line4TextReference.color.g, line4TextReference.color.b, animationAlpha);
            lareissaSplashReference.GetComponent<Image>().color = new Color(creditsTextReference.color.r, creditsTextReference.color.g, creditsTextReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // Fade out bgm
        FindObjectOfType<AudioManagerScript>().FadeOutBGM();

        // Wait until end
        yield return new WaitForSeconds(animationWaitRate);

        // Seamless transition to title
        FindObjectOfType<GameManagerScript>().LoadTitleSceneSeamless();
    }
}
