using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIGenericButtonSoundsScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On hover and click events
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonHover");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<AudioManagerScript>().PlayUISFX("BasicButtonClick");
    }
}
