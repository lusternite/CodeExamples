using UnityEngine;
using System.Collections;

public class PrizeCollection : MonoBehaviour
{
    bool[] PrizesObtained;
    public GameObject[] prizes;

    // Use this for initialization
    void Start ()
    {
        PrizesObtained = GameObject.Find("GameManager").GetComponent<GameManager>().PrizesObtained;
        for (int i = 0; i < prizes.Length; ++i)
        {
            if (PrizesObtained[i] == false)
                prizes[i].SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
