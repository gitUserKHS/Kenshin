using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    Vector3 camDir;

    [SerializeField]
    GameObject player;

    PlayerController playerController;

    public struct Directions
    {
        public Vector3 forward;
        public Vector3 right;
    }

    public Directions directions;
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

    private void Awake()
    {
        directions = new Directions() { forward = transform.forward, right = transform.right };
        camDir = new Vector3(0f, 1.5f, -3.5f);
    }

    void Start()
    {
        //playerController = player.GetComponent<PlayerController>();
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
        directions.forward = transform.forward - new Vector3(0, transform.forward.y, 0);
        directions.right = transform.right - new Vector3(0, transform.right.y, 0);
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
