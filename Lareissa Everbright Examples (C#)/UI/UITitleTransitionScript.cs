using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitleTransitionScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    public List<GameObject> titleButtons;
    public GameObject titleHeaderReference;
    public Animator titleGwenAnimatorReference;
    public Animator titleBreathAnimatorReference;
    private Image selfImageReference;

    public float alphaFadeRate = 1;
    private float animationAlpha = 1;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {

        // First off, set title header to be invisible
        titleHeaderReference.GetComponent<Coffee.UIExtensions.UIDissolve>().effectFactor = 1;

        // Also demo text if it exists
        if (titleHeaderReference.transform.GetChild(0))
        {
            titleHeaderReference.transform.GetChild(0).GetComponent<Coffee.UIExtensions.UIDissolve>().effectFactor = 1;
        }

        // Get self
        selfImageReference = GetComponent<Image>();

        // Set self to be visible
        selfImageReference.color = new Color(selfImageReference.color.r, selfImageReference.color.g, selfImageReference.color.b, 1);

        // Start the coroutine that handles the animation
        StartCoroutine(TitleTransitionAnimationCoroutine());
	}
	
	private IEnumerator TitleTransitionAnimationCoroutine()
    {
        // Wait for some time for just purely dark.
        yield return new WaitForSeconds(1.33f);

        // Force set buttons to be invisible
        for (int i = 0; i < titleButtons.Count; i++)
        {
            titleButtons[i].GetComponent<Animator>().Play("TitleButtonInvisible");
        }

        // Then go ahead and start fading out
        while (animationAlpha > 0.0f)
        {
            animationAlpha -= Time.deltaTime * alphaFadeRate;
            selfImageReference.color = new Color(selfImageReference.color.r, selfImageReference.color.g, selfImageReference.color.b, animationAlpha);
            yield return new WaitForEndOfFrame();
        }

        // After fade out is done, wait a while longer
        yield return new WaitForSeconds(0.33f);

        // Then start the header animation
        titleHeaderReference.GetComponent<Coffee.UIExtensions.UIDissolve>().Play();

        // And reveal gwen
        titleGwenAnimatorReference.Play("TitleGwenUnrevealedIdle");

        // Also demo text if it exists
        if (titleHeaderReference.transform.GetChild(0))
        {
            titleHeaderReference.transform.GetChild(0).GetComponent<Coffee.UIExtensions.UIDissolve>().Play();
        }

        yield return new WaitForEndOfFrame();

        // Wait for this animation to finish
        while (titleHeaderReference.GetComponent<Coffee.UIExtensions.UIDissolve>().effectFactor != 0)
        {
            yield return new WaitForEndOfFrame();
        }

        // Wait just a bit more again
        yield return new WaitForSeconds(0.33f);

        // Finally fade in the buttons
        for (int i = 0; i < titleButtons.Count; i++)
        {
            titleButtons[i].GetComponent<Animator>().Play("TitleButtonFadeIn");
        }

        // And start breath anim
        titleBreathAnimatorReference.Play("TitleBreathLoop");

        // Wait just a bit more again
        yield return new WaitForSeconds(1.0f);

        // All is done, end self
        Destroy(gameObject);
    }
}
