using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthMeterScript : MonoBehaviour {

    public UIHealthScript healthReference;
    private float maxHealthValue;

    public float maxWidth = 465;
    public float maxHeight = 17.0f;

    public bool getWidthAndHeightOnSpawn = true;

    private RectTransform transformReference;

    // Use this for initialization
    void Start()
    {
        //revengeReference = GetComponentInChildren<UIRevengeScript>();
        transformReference = GetComponent<RectTransform>();
        maxHealthValue = healthReference.GetCurrentHealth();

        if (getWidthAndHeightOnSpawn)
        {
            maxWidth = transformReference.rect.width;
            maxHeight = transformReference.rect.height;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Change width of judgement meter depending on judgement percentage
        transformReference.sizeDelta = new Vector2(maxWidth * (healthReference.GetCurrentHealth() / maxHealthValue), maxHeight);
    }

    public void ResetMaxHealthValue()
    {
        maxHealthValue = healthReference.GetCurrentHealth();
    }
}
