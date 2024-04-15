using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class CameraController : MonoBehaviour
{ 
    Vector3 camDir;
    Vector3 camDir_FirstPerson;

    [SerializeField]
    GameObject player;
    PlayerController playerController;

    CameraMode camMode = CameraMode.RoundView;
    public CameraMode CamMode { 
        get { return camMode; } 
        set
        {
            if(camMode == value) 
                return;

            if (value == CameraMode.FirstPersonView)
            {
                transform.parent = player.transform;
                transform.position = player.transform.TransformPoint(camDir_FirstPerson);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if(value == CameraMode.RoundView)
            {
                camDir = player.transform.forward * -3.5f + player.transform.up * 1.5f;
                transform.position = focusPos + camDir;
                transform.parent = Managers.Scene.CurrentScene.transform;
                transform.LookAt(focusPos);
            }

            camMode = value;
        }
    }

    Vector3 focusPos;
    float mouseDeltaX;
    float mouseDeltaY;
    float currentRotationX;

    [SerializeField]
    float sensitivity;
    [SerializeField]
    float scrollSpeed;

    float maxFocusDist = 5f;
    float minFocusDist = 2f;
    float focusDist = 3f;
    float yAngleUpperLimit = 60 * Mathf.PI / 180;
    float yAngleLowerLimit = 5 * Mathf.PI / 180;

    private void Awake()
    {
        camDir = player.transform.forward * -3.5f + player.transform.up * 1.5f;
        camDir_FirstPerson = player.transform.up * 1.391f + player.transform.forward * 0.5f;
        playerController = player.GetComponent<PlayerController>();
        focusPos = player.transform.position + Vector3.up * 1f;
    }

    void Start()
    {
        
    }

    private void LateUpdate()
    {
        focusPos = player.transform.position + Vector3.up * 1f;

        switch (CamMode)
        {
            case CameraMode.RoundView:
                HandleRoundView();
                playerController.HandleCharacterInput();
                playerController.ProcessRotation();
                break;
            case CameraMode.FirstPersonView:
                HandleFirstPersonView();
                playerController.HandleCharacterInput();
                break;
        }
    }

    private void HandleRoundView()
    {
        mouseDeltaX = Input.GetAxis("Mouse X");
        mouseDeltaY = Input.GetAxis("Mouse Y");
        mouseDeltaX = Mathf.Clamp(mouseDeltaX, -3, 3);

        float cameraUpperLimitY = focusPos.y + focusDist * Mathf.Sin(yAngleUpperLimit);
        float cameraLowerLimitY = focusPos.y + focusDist * Mathf.Sin(yAngleLowerLimit);

        Vector3 deltaCameraMove = transform.right * -mouseDeltaX;
        if (false == (transform.position.y > cameraUpperLimitY && mouseDeltaY < 0 || transform.position.y < cameraLowerLimitY && mouseDeltaY > 0))
            deltaCameraMove += transform.up * -mouseDeltaY;
        camDir = (camDir + deltaCameraMove * sensitivity * Time.deltaTime).normalized;

        transform.position = focusPos + camDir * focusDist;
        transform.LookAt(focusPos);

        Zoom();
    }

    private void HandleFirstPersonView()
    {
        mouseDeltaX = Input.GetAxis("Mouse X");
        mouseDeltaY = Input.GetAxis("Mouse Y");

        float xRotation = mouseDeltaY * sensitivity;
        currentRotationX -= xRotation;
        currentRotationX = Mathf.Clamp(currentRotationX, -yAngleUpperLimit * Mathf.Rad2Deg, yAngleUpperLimit * Mathf.Rad2Deg);
        player.transform.rotation *= Quaternion.Euler(0, mouseDeltaX * sensitivity, 0);
        transform.localRotation = Quaternion.Euler(currentRotationX, 0, 0);
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        focusDist = Mathf.Clamp(focusDist - scroll, minFocusDist, maxFocusDist);
    }
}
