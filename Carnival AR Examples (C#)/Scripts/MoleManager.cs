using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoleManager : MonoBehaviour {

    public GameObject[] Moles;

    public bool _canTheGameStart = true;
    public bool _isGameActive = false;
    bool _iWaveDefeated = false;
    bool _GameOver = false;

    public int _iWaveNumber = 0;
    public int _iMolesMin = 2;
    public int _iMolesMax = 4;

    public float _fTimeToHit = 3.5f;
    public int[] _iMolesToActivate;

    public bool _isWaveRunning = false;
    public float _fTimeWaveStarted = 0.0f;

    public float _fTime = 0;
    public float _fMaxTime = 30.0f;

    public float _fEndTimer = 0.0f;
    public float _fEndTimeMax = 5.0f;

    float _fWinTimer = 0.0f;

    public int _CurrentScore = 0;
    int _ScoreAfterWave = 0;

    public AudioClip Victory;

    public Text TimeText;

    // Use this for initialization
    void Start () {
        //GameObject.Find("GameManager").GetComponent<GameManager>().ActivateStartPopup();
        _fTimeToHit = 3.5f;
        _isWaveRunning = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (_canTheGameStart == true)
        {
            //if (GameStart() == true) //Whatever detects when the game wants to be started
            {
                _canTheGameStart = false;
                _isGameActive = true;
            }
        }

        if (_isGameActive == true) //If game is active
        {
            if (_isWaveRunning == false) //If there is no wave of moles spawned
            {
                //----Spawn a new wave
                int _iMolesToSpawn = Random.Range(2, 5); //Decide how many moles to spawn in the wave
                //Debug.Log(_iMolesToSpawn);
                _ScoreAfterWave = _CurrentScore + _iMolesToSpawn; //Set a future score param to check against

                ActivateMoles(_iMolesToSpawn);

                _fTimeWaveStarted = Time.time;
                _isWaveRunning = true;
                _iWaveDefeated = false;
                //----
            }
            else
            {
                if ((Time.time - _fTimeWaveStarted > (_fTimeToHit + 0.5f)) && _iWaveDefeated == false) //Is the wave over
                {
                    _isWaveRunning = false;
                }
                if (_CurrentScore == _ScoreAfterWave && _iWaveDefeated == false) //Did the player Level up? (Checks a predicted score to current score)
                {
                    AudioSource.PlayClipAtPoint(Victory, transform.position, 3.0f);
                    _iWaveDefeated = true;
                    IncreaseDifficulty();
                }
                if (_iWaveDefeated == true)
                {
                    _fWinTimer += Time.deltaTime;
                    if (_fWinTimer >= 0.2f)
                    {
                        _fWinTimer = 0.0f;
                        _isWaveRunning = false;
                    }
                }
                //
            }
          
            
            _fTime += Time.deltaTime;
            if (_fTime >= _fMaxTime) //is game over?
            {
                _isGameActive = false;
                _GameOver = true;
                _fEndTimer = 0.0f;
                TimeText.text = 0.ToString() ;
            }
            else
            {
                TimeText.text = (Mathf.Ceil(_fMaxTime - _fTime)).ToString();
            }
        }
        else
        {
            _fEndTimer += Time.deltaTime;
        }

        if (_GameOver == true && _fEndTimer > _fEndTimeMax)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    public void ActivateMoles(int _iAmount)
    {
        int[] _iAlreadyUsed = new int[_iAmount];
        for (int i = 0; i < _iAmount; ++i) //Activate the amount of moles passed in
        {
            bool _isUsable;
            int _iMole;

            do{ //Find a usable mole to activate 
                _isUsable = true;
                _iMole = Random.Range(1, 8);
                for (int j = 0; j < _iAlreadyUsed.Length; ++j) //Check to see if the random mole has already been used
                {
                    if (_iMole == _iAlreadyUsed[j])
                    {
                        _isUsable = false;
                    }
                }
            } while (_isUsable == false) ;
            //Debug.Log(_iAlreadyUsed.Length);


           _iAlreadyUsed[i] = _iMole; //Add new moles to already used
            Moles[_iMole - 1].GetComponent<MoleScript>().Activate();
        }
    }

    public void IncreaseDifficulty()
    {
        _iWaveNumber += 1;
        if (_iWaveNumber > 5)
        {
            _fTimeToHit = 1.0f - (0.2f * (_iWaveNumber - 5));
        }
        else
        {
            _fTimeToHit = 3.5f - (0.5f * _iWaveNumber);
        }
        if (_fTimeToHit < 0.2f)
        {
            _fTimeToHit = 0.2f;
        }
    }
}
