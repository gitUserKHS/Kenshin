using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Vector3 camDir;

    [SerializeField]
    GameObject player;
    PlayerController playerController;

    Transform focus;
    float mouseDeltaX;
    float mouseDeltaY;

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
        camDir = new Vector3(0f, 1.5f, 3.5f);
        playerController = player.GetComponent<PlayerController>();
    }

    void Start()
    {
        focus = player.transform;
    }

    private void LateUpdate()
    {
        mouseDeltaX = Input.GetAxis("Mouse X");
        mouseDeltaY = Input.GetAxis("Mouse Y");

        mouseDeltaX = Mathf.Clamp(mouseDeltaX, -3, 3);
        Vector3 focusPos = focus.position + Vector3.up * 1f;

        float cameraUpperLimitY = focusPos.y + focusDist * Mathf.Sin(yAngleUpperLimit);
        float cameraLowerLimitY = focusPos.y + focusDist * Mathf.Sin(yAngleLowerLimit);

        Vector3 deltaCameraMove = transform.right * -mouseDeltaX;
        if(false == (transform.position.y > cameraUpperLimitY && mouseDeltaY < 0 || transform.position.y < cameraLowerLimitY && mouseDeltaY > 0))
            deltaCameraMove += transform.up * -mouseDeltaY;
        camDir = (camDir + deltaCameraMove * sensitivity * Time.deltaTime).normalized;

        transform.position = focusPos + camDir * focusDist;
        transform.LookAt(focusPos);
        
        Zoom();

        playerController.HandleCharacterInput();
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        //if (Mathf.Abs(scroll) < 0.01f)
        //    return;

        focusDist = Mathf.Clamp(focusDist - scroll, minFocusDist, maxFocusDist);
    }
}
