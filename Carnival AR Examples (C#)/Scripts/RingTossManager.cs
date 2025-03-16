using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RingTossManager : MonoBehaviour {
    public int _iRings = 20;
    bool GameOver = false;
    float _fTimer;

    public Text RingText;

    // Use this for initialization
    void Start () {
        RingText.text = _iRings.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (_iRings <= 0 && GameOver == false)
        {
            GameOver = true;
            _fTimer = Time.time;
        }
        if (GameOver == true && Time.time - _fTimer > 5.0f)
        {
            //NEXT LEVEL
        }
    }

    public bool ShootRing()
    {
        if (_iRings >= 1)
        {
            _iRings--;
            RingText.text = _iRings.ToString();
            return true;
        }
        else
        {
            return false;
        }
        return false;
    }


}
