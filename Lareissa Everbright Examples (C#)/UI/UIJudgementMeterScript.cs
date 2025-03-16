using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIJudgementMeterScript : MonoBehaviour {

    public UIJudgementScript judgementReference;

    public float maxWidth = 465;

    private RectTransform transformReference;

	// Use this for initialization
	void Start () {
        //judgementReference = GetComponentInChildren<UIJudgementScript>();
        transformReference = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        // Change width of judgement meter depending on judgement percentage
        transformReference.sizeDelta = new Vector2(maxWidth * (judgementReference.GetMeterValue() / 100.0f), 38.75f);
	}
}
