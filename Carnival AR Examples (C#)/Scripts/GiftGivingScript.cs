using UnityEngine;
using System.Collections;

public class GiftGivingScript : MonoBehaviour
{
    public GameObject[] Prizes;
    public GameObject[] PrizeTransforms;
    bool[] PrizesObtained;

    // Use this for initialization
    void Start ()
    {
        PrizesObtained = GameObject.Find("GameManager").GetComponent<GameManager>().PrizesObtained;
        for (int i = 0; i < Prizes.Length; ++i)
        {
            if (PrizesObtained[i] == false)
            {
                Prizes[i].SetActive(true);
                PrizeTransforms[i].SetActive(true);
            }
            else
            {
                Prizes[i].SetActive(false);
                PrizeTransforms[i].SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
