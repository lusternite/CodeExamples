using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BearSelection : MonoBehaviour {

    public int TicketsRequired;
    public float PointTimer;
    public bool PointedAt;
    public GameObject BearType;
    public GameObject TutorialImage;
    public Sprite GiftGiveSprite;

    public int PrizeIndex;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	    if (PointedAt)
        {
            PointTimer -= Time.deltaTime;
            if (PointTimer <= 0.0f)
            {
                //Obtain prize
                //BearType.SetActive(true);
                gameObject.SetActive(false);
                //TutorialImage.GetComponent<Image>().sprite = GiftGiveSprite;
                //GameObject.Find("FPSController").GetComponent<Animator>().Play("CameraPanToDate");
                GameObject.Find("GameManager").GetComponent<GameManager>().PrizesObtained[PrizeIndex] = true;
            }
        }
	}

    public void PointAtBear()
    {
        if (FindObjectOfType<CarnivalManager>().ticketScore >= TicketsRequired)
        {
            PointedAt = true;
            Debug.Log("Pointed at " + BearType);
        }
    }

    public void PointOffBear()
    {
        PointedAt = false;
    }

}
