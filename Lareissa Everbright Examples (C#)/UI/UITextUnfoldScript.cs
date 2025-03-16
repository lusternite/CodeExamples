using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextUnfoldScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private Text textReference;

    private Color initialColor;

    public string actualText;

    [Tooltip("This is calculated as characters revealed per second")]
    public float unfoldSpeed = 30.0f;

    // Used to determined the total amount of characters in this text block
    private int textLength;

    private int currentTextCount;

    // Used to determine if current unfold contains unfinished colour indicator
    private bool unfinishedColourFlag = false;

    public bool shouldStartHidden = true;

    private bool unfoldCompletionFlag = false;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
        textReference = GetComponent<Text>();
        initialColor = textReference.color;
        if (actualText == "")
        {
            actualText = textReference.text;
        }
        unfoldSpeed = 1.0f / unfoldSpeed;
        textLength = actualText.Length;

        if (shouldStartHidden)
        {
            HideText();
        }
        else
        {
            ShowText();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Used to reset the text
    public void SetText(string newText)
    {
        actualText = newText;
        textLength = actualText.Length;
        unfoldCompletionFlag = false;
    }
    
    // The calling function to unfold the text
    public void ShowText()
    {
        // Reveal text
        textReference.color = initialColor;

        // Then invoke unfold function
        InvokeRepeating("UnfoldText", 0.0f, unfoldSpeed);
    }

    // NOTE TO SELF: ^ means positive adjustment so orange, * means negative adjustment so blue, # is end of adjustment
    // Actual invoked function that unfolds text
    private void UnfoldText()
    {
        // See if should invoke
        if (textReference.color == initialColor)
        {
            textReference.text = actualText.Remove(currentTextCount);

            // Check to see if colour adjustment is starting
            if (textReference.text.EndsWith("^") || textReference.text.EndsWith("*"))
            {
                // Mark the unfinished colour flag and advance one letter
                unfinishedColourFlag = true;
                currentTextCount += 1;
            }

            // Check to see if colour adjustment is ending
            if (textReference.text.EndsWith("#"))
            {
                // Mark unfinished colour flag as false and advance one letter
                unfinishedColourFlag = false;
                currentTextCount += 1;
            }

            // Replace all colour adjustment symbols
            textReference.text = textReference.text.Replace("^", "<color=orange>");
            textReference.text = textReference.text.Replace("*", "<color=blue>");
            textReference.text = textReference.text.Replace("#", "</color>");

            // If currently unfinished colour, add correct ending to the colour
            if (unfinishedColourFlag == true)
            {
                textReference.text += "</color>";
            }

            // Finally finish off the text by hiding the rest of the text
            textReference.text += "<color=#00000000>" + actualText.Substring(currentTextCount) + "</color>";

            currentTextCount += 1;
            if (currentTextCount >= textLength)
            {
                textReference.text = actualText;

                // Replace all colour adjustment symbols
                textReference.text = textReference.text.Replace("^", "<color=orange>");
                textReference.text = textReference.text.Replace("*", "<color=blue>");
                textReference.text = textReference.text.Replace("#", "</color>");

                unfoldCompletionFlag = true;
                CancelInvoke();
            }
        }
        
    }

    // Simply hides text by turning alpha to 0 and deleting text
    public void HideText()
    {
        // Hide text
        textReference.color = Color.clear;
        textReference.text = "";
        unfinishedColourFlag = false;

        // Stop invoking
        CancelInvoke();

        // Set up stuff for unfolding
        currentTextCount = 0;

    }

    // Used to immediately display all text
    public void ForceUnfoldAll()
    {
        textReference.text = actualText;
        unfoldCompletionFlag = true;
        CancelInvoke();
    }

    public bool IsUnfoldComplete()
    {
        return unfoldCompletionFlag;
    }
}
