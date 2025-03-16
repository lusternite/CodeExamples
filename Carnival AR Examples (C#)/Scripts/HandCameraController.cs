using UnityEngine;
using System.Collections;

public class HandCameraController : MonoBehaviour {

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public Transform HandTransform;
    public Transform TargetTransform;
    public Transform m_CameraTransform;
    public bool isActive;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    public Vector3 m_PreviousPosition;

    public void Init()
    {
        m_CameraTransform = Camera.main.transform;
        m_CharacterTargetRot = TargetTransform.localRotation;
        m_CameraTargetRot = m_CameraTransform.localRotation;
        m_PreviousPosition = HandTransform.position;
    }

    public void SetEnabled()
    {
        isActive = true;
        m_PreviousPosition = HandTransform.localPosition;
    }

    public void SetDisabled()
    {
        isActive = false;
    }

    public void LookRotation(Transform character, Transform camera)
    {
        float yRot = (m_PreviousPosition.y - HandTransform.localPosition.y) * XSensitivity * 5.0f;
        float xRot = (m_PreviousPosition.x - HandTransform.localPosition.x) * YSensitivity;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }
        Debug.Log("Character target rot = " + m_CharacterTargetRot);
        Debug.Log("Camera target rot = " + m_CameraTargetRot);
    }

    // Use this for initialization
    void Start () {
        Init();
        isActive = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            LookRotation(TargetTransform, m_CameraTransform);
        }
        m_PreviousPosition = HandTransform.localPosition;
	}

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }


}
