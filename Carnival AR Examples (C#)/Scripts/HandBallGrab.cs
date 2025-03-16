using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandBallGrab : MonoBehaviour {

    public GameObject SelectedBall;
    public GameObject GrabbedBall;
    public Transform PalmTransform;
    public bool IsActive;

    public Vector3 GrabbedBallPreviousPosition;
    public Vector3 GrabbedBallThrowStrength;
    public Vector3 PreviousPosition;
    public Vector3 PreviousPreviousPosition;
    public List<Vector3> PreviousPositions;

    // Use this for initialization
    void Start () {
        SelectedBall = null;
        IsActive = false;

    }
	
	// Update is called once per frame
	void Update () {
	    if (GrabbedBall != null && IsActive)
        {
            //Make ball stay in hand
            GrabbedBall.transform.position = PalmTransform.position + PalmTransform.up * -0.05f + PalmTransform.right * 0.05f;

            //Update throw strength
            UpdateThrowStrength();
        }
    }

    void FixedUpdate()
    {
        //PreviousPreviousPosition = PreviousPosition;
        //PreviousPosition = transform.position;
        if (PreviousPositions.Count < 10)
        {
            PreviousPositions.Add(transform.position);
        }
        else
        {
            PreviousPositions.RemoveAt(0);
            PreviousPositions.Add(transform.position);
        }
    }

    void OnTriggerEnter(Collider Col)
    {
        if (Col.tag == "Ball" && SelectedBall == null)
        {
            SelectedBall = Col.gameObject;
        }
    }

    void OnTriggerExit(Collider Col)
    {
        if (Col.tag == "Ball" && Col.gameObject == SelectedBall.gameObject)
        {
            SelectedBall = null;
        }
    }

    public void GrabBall()
    {
        if (GrabbedBall == null)
        {
            IsActive = true;
            GrabbedBall = SelectedBall;
            if (GrabbedBall != null)
            {
                GrabbedBallPreviousPosition = GrabbedBall.transform.position;
                GrabbedBall.GetComponent<SphereCollider>().enabled = false;
            }
        }
    }

    public void ThrowBall()
    {
        IsActive = false;
        if (GrabbedBall != null)
        {
            Vector3 AverageMovement = new Vector3(0.0f, 0.0f, 0.0f);
            for (int i = 0; i < PreviousPositions.Count; i++)
            {
                AverageMovement += PreviousPositions[i];
            }
            AverageMovement /= PreviousPositions.Count;
            Debug.Log(AverageMovement);
            GrabbedBall.GetComponent<Rigidbody>().velocity = Vector3.Normalize(transform.position - AverageMovement) * 10.0f;
            GrabbedBall.transform.position = PalmTransform.position;// + PalmTransform.up * -0.18f + PalmTransform.right * 0.06f;
            //GrabbedBall.GetComponent<Rigidbody>().AddForce(PalmTransform.up * 100.0f);
            GrabbedBall.GetComponent<SphereCollider>().enabled = true;
        }
        GrabbedBall = null;
    }

    public void TestFunction()
    {
        Debug.Log("Test Function happened.");
    }

    void UpdateThrowStrength()
    {
        //X
        if (Mathf.Abs(GrabbedBallThrowStrength.x + (GrabbedBall.transform.position.x - GrabbedBallPreviousPosition.x)) > GrabbedBallThrowStrength.x)
        {
            GrabbedBallThrowStrength.x += (GrabbedBall.transform.position.x - GrabbedBallPreviousPosition.x) * 10.0f;
        }
        else
        {
            GrabbedBallThrowStrength.x = (GrabbedBall.transform.position.x - GrabbedBallPreviousPosition.x) * -10.0f;
        }

        //Y
        if (Mathf.Abs(GrabbedBallThrowStrength.y + (GrabbedBall.transform.position.y - GrabbedBallPreviousPosition.y)) > GrabbedBallThrowStrength.y)
        {
            GrabbedBallThrowStrength.y += (GrabbedBall.transform.position.y - GrabbedBallPreviousPosition.y) * 10.0f;
        }
        else
        {
            GrabbedBallThrowStrength.y = (GrabbedBall.transform.position.y - GrabbedBallPreviousPosition.y) * -10.0f;
        }

        //Z
        if (Mathf.Abs(GrabbedBallThrowStrength.z + (GrabbedBall.transform.position.z - GrabbedBallPreviousPosition.z)) > GrabbedBallThrowStrength.z)
        {
            GrabbedBallThrowStrength.z += (GrabbedBall.transform.position.z - GrabbedBallPreviousPosition.z) * 10.0f;
        }
        else
        {
            GrabbedBallThrowStrength.z = (GrabbedBall.transform.position.z - GrabbedBallPreviousPosition.z) * -10.0f;
        }
    }
}
