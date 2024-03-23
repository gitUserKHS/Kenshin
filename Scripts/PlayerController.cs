using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float moveSpeed = 10.0f;
    Vector3 moveDir = Vector3.zero;
    Vector3 lookDir = Vector3.zero;
    public Vector3 forward;
    public Vector3 right;
    
    public struct Directions
    {
        public Vector3 forward;
        public Vector3 right;
    }

    public Queue<Directions> dirQueue;

    private void Awake()
    {
        dirQueue = new Queue<Directions>();  
        forward = transform.forward;
        right = transform.right;
    }

    void Start()
    {
      
    }

    void Update()
    {
        if (dirQueue.Count > 0)
        {
            ProcessMove();
        }
    }
    
    void ProcessMove()
    {
        while (dirQueue.Count > 0)
        {
            Directions dir = dirQueue.Dequeue();
            forward = dir.forward;
            right = dir.right;

            moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                moveDir += forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir += -forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDir += -right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDir += right;
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
}


