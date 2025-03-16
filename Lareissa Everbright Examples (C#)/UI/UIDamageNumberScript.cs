using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageNumberScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    // Determines if the velocity is going to be left or right
    public bool travelSideRightFlag;

    // Default going to right
    public float damageValue;

    public Vector2 actualVelocity;

    public Vector2 initVelocity;

    public Vector2 targetSlowVelocity;

    public float initScale = 3.0f;

    public float fastTravelDuration = 0.2f;
    public float velocityAdjustDuration = 0.1f;
    public float slowTravelDuration = 1.5f;

    public float alphaFadeDuration = 0.5f;

    public float bigDamageThreshold = 30.0f;
    public int bigFontSize = 72;
    public float hugeDamageThreshold = 50.0f;
    public int hugeFontSize = 75;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        StartCoroutine("DamageNumberAnimationCoroutine");
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Initialise(float damage, bool travelSideRight)
    {
        damageValue = damage;

        if (travelSideRight == false)
        {
            // Flip the velocities
            initVelocity.x *= -1.0f;
            targetSlowVelocity.x *= -1.0f;
        }
        travelSideRightFlag = travelSideRight;
    }

    // The animation
    public IEnumerator DamageNumberAnimationCoroutine()
    {
        // Set text
        GetComponent<Text>().text = damageValue.ToString();
        
        // Check if damage is big
        if (damageValue >= bigDamageThreshold)
        {
            // Check if damage is huge
            if (damageValue >= hugeDamageThreshold)
            {
                // Purple
                GetComponent<Text>().color = new Color(75.0f / 255.0f, 0, 130.0f / 255.0f);
                // Make font larger
                GetComponent<Text>().fontSize = hugeFontSize;
            }
            else
            {
                // Orange
                GetComponent<Text>().color = new Color(1, 140.0f / 255.0f, 0);
                // Make font larger
                GetComponent<Text>().fontSize = bigFontSize;
            }
            
        }

        // First start with fast travel segment

        float animationTimer = 0.0f;

        actualVelocity = initVelocity;

        // Adjust scale settings as well
        GetComponent<RectTransform>().localScale = new Vector3(initScale, initScale);

        while (animationTimer < fastTravelDuration)
        {
            animationTimer += Time.deltaTime;

            // Move with velocity
            GetComponent<RectTransform>().anchoredPosition += actualVelocity * Time.deltaTime;

            // Scale down
            GetComponent<RectTransform>().localScale = new Vector3(Mathf.Lerp(initScale, 1, animationTimer / fastTravelDuration), Mathf.Lerp(initScale, 1, animationTimer / fastTravelDuration));

            yield return new WaitForEndOfFrame();
        }

        // After that, go to the velocity transition segment

        animationTimer = 0.0f;

        while (animationTimer < velocityAdjustDuration)
        {
            animationTimer += Time.deltaTime;

            // Adjust velocity to target slow velocity
            actualVelocity.x = Mathf.Lerp(initVelocity.x, targetSlowVelocity.x, animationTimer / velocityAdjustDuration);
            actualVelocity.y = Mathf.Lerp(initVelocity.y, targetSlowVelocity.y, animationTimer / velocityAdjustDuration);

            // Move with velocity
            GetComponent<RectTransform>().anchoredPosition += actualVelocity * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        // After that is done, go with slow velocity until alpha starts

        animationTimer = 0.0f;

        actualVelocity = targetSlowVelocity;

        while (animationTimer < slowTravelDuration)
        {
            animationTimer += Time.deltaTime;

            // Move with velocity
            GetComponent<RectTransform>().anchoredPosition += actualVelocity * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        // Finally do the alpha fade part

        animationTimer = 0.0f;

        while (animationTimer < alphaFadeDuration)
        {
            animationTimer += Time.deltaTime;

            // Move with velocity
            GetComponent<RectTransform>().anchoredPosition += actualVelocity * Time.deltaTime;

            // Adjust alpha
            GetComponent<Text>().color = new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, Mathf.Lerp(1, 0, animationTimer / alphaFadeDuration));

            yield return new WaitForEndOfFrame();
        }

        // All is done, destroy self and end this coroutine.
        Destroy(gameObject, 0.1f);

        yield return null;
    }
}
