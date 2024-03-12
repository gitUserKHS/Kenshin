using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10.0f;
    Vector3 moveDir = Vector3.zero;
    Vector3 lookDir = Vector3.zero;

    [SerializeField]
    Camera camera;

    CameraController cameraController;

    private void Awake()
    {

    }

    void Start()
    {
        cameraController = camera.GetComponent<CameraController>();
    }

    void Update()
    {
        ProcessMove();
    }
    
    void ProcessMove()
    { 
        moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += cameraController.directions.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += -cameraController.directions.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += -cameraController.directions.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += cameraController.directions.right;
        }
        moveDir = moveDir.normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 40 * Time.deltaTime);
            //transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}


