using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoleScript : MonoBehaviour {

    float _fMaxHeight; //Min height
    float _fMinHeight; //Min height

    public float _fHeightoffset = 0.2f;

    public bool _isGameOn = false; //Is MoleManager on
    public bool _isActive = false; //If the mole is going to go up then down
    public bool _isGoingUp = false; //Flag for direction up
    public bool _isGoingDown = false; //Flag for direction up
    public bool _PointsAvalible = false; //If mole is active then have the points been collected

    public float _fDownVelocity = -0.02f; //y velocity
    public float _fUpVelocity = 0.02f;

    public float _fTimeAtMax; //How long it should stay at the top
    public double _dTimePassed;


    public float _fCheckRate = 1.0f;

    public AudioClip HitSound;
    public AudioClip MoleOuch;

	// Use this for initialization
	void Start () {
        _fMinHeight = transform.position.y;
        _fMaxHeight = transform.position.y + _fHeightoffset;
    }
	
	// Update is called once per frame
	void Update () {
        _isGameOn = GameObject.Find("Mole Manager").GetComponent<MoleManager>()._isGameActive;

        if (_isGameOn == true)
        {
            OperateMole();
        }
        else 
        {
            if (transform.position.y > _fMinHeight) //If game is off but moles are still awake, bring them down at once
            {
                OperateMole();
            }            
        }
	}  

    public void Activate()
    {
        _isActive = true;
        _isGoingUp = true;
        _PointsAvalible = true;
        _fTimeAtMax = GameObject.Find("Mole Manager").GetComponent<MoleManager>()._fTimeToHit;
    }

    public void OperateMole()
    {
        if (_isGoingUp == true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + _fUpVelocity, transform.position.z);
            if (transform.position.y >= _fMaxHeight)
            {
                transform.position = new Vector3(transform.position.x, _fMaxHeight, transform.position.z);
                _isGoingUp = false;
                _dTimePassed = Time.time;
            }
            _fDownVelocity = -0.02f;
        }
        else if (_isGoingDown == true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + _fDownVelocity, transform.position.z);
            if (transform.position.y <= _fMinHeight)
            {
                transform.position = new Vector3(transform.position.x, _fMinHeight, transform.position.z);
                _isGoingDown = false;
                _isActive = false;         
                _PointsAvalible = false;
                _dTimePassed = Time.time + 1.0f; //Gives the mole a cooldown
            }
        }
        else
        {
            if (Time.time - _dTimePassed > _fTimeAtMax)
            {
                _isGoingDown = true;
            }
        }
    }

    GameManager getManager()
    {
        GameObject obj = GameObject.Find("GameManager");
        return obj.GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hammer")
        {
            if (_PointsAvalible == true && GameObject.Find("R_Palm").GetComponent<HammerHand>()._Velocity.y < -0.01f)
            {
                GetComponent<ParticleSystem>().Play();
                _isGoingDown = true;
                _isGoingUp = false;
                _PointsAvalible = false;
                GameObject.Find("Mole Manager").GetComponent<MoleManager>()._CurrentScore += 1;
                getManager().SetScore(5);
                _fDownVelocity = -0.06f;
                AudioSource.PlayClipAtPoint(HitSound, transform.position, 0.3f);
                AudioSource.PlayClipAtPoint(MoleOuch, transform.position, 0.3f);
            }
        }       
    }
}
