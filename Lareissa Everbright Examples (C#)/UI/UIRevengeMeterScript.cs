using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRevengeMeterScript : MonoBehaviour {

    public UIRevengeScript revengeReference;

    public float maxWidth = 465;

    private RectTransform transformReference;

    // Use this for initialization
    void Start()
    {
        //revengeReference = GetComponentInChildren<UIRevengeScript>();
        transformReference = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change width of judgement meter depending on judgement percentage
        transformReference.sizeDelta = new Vector2(maxWidth * (revengeReference.GetMeterValue() / 100.0f), 35.35f);
    }
}
