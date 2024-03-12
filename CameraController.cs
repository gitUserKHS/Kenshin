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

    float mouseDeltaX;
    float mouseDeltaY;

    [SerializeField]
    float sensitivity;
    [SerializeField]
    float scrollSpeed;

    float maxPlayerDist = 5f;
    float minPlayerDist = 2f;
    float playerDist = 3f;
    float yAngleLimit = 60 * Mathf.PI / 180;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        camDir = new Vector3(0f, 1.5f, -3.5f);
    }

    //void Update()
    //{

    //}

    private void LateUpdate()
    {
        if (player == null)
            return;

        mouseDeltaX = Input.GetAxis("Mouse X");
        mouseDeltaY = Input.GetAxis("Mouse Y");
        
        float cameraUpperLimitY = player.transform.position.y + playerDist * Mathf.Sin(yAngleLimit);
        float cameraLowerLimitY = player.transform.position.y - playerDist * Mathf.Sin(yAngleLimit);

        Vector3 deltaCameraMove = transform.right * -mouseDeltaX;
        if(false == (transform.position.y > cameraUpperLimitY && mouseDeltaY < 0 || transform.position.y < cameraLowerLimitY && mouseDeltaY > 0))
            deltaCameraMove += transform.up * -mouseDeltaY;
        camDir = (camDir + deltaCameraMove * sensitivity * Time.deltaTime).normalized;

        transform.position = player.transform.position + camDir * playerDist;

        transform.LookAt(player.transform.position);
        var f = transform.forward - new Vector3(0, transform.forward.y, 0);
        var r = transform.right - new Vector3(0, transform.right.y, 0);
        playerController.dirQueue.Enqueue(new PlayerController.Directions() { forward = f, right = r });
        Zoom();
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        playerDist = Mathf.Clamp(playerDist - scroll, minPlayerDist, maxPlayerDist);
    }
}
