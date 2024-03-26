using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class PlayerController : CreatureController
{
    Vector3 playerMoveDir = Vector3.zero;
    Vector3 playerVelocity = Vector3.zero;   

    float playerMoveSpeed = 7.0f;
    float gravity = -9.8f;
    float jumpHeight = 1.5f;

    bool playerSlerping = false;
    bool isAttacking = false;

    CharacterController controller;

    protected override void Init_Awake()
    {
        base.Init_Awake();
        controller = GetComponent<CharacterController>();
    }

    protected override void UpdateController()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        if (IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);

        if(Input.GetMouseButtonDown(0) && IsGrounded())
        {
            isAttacking = true;
            endAttacking = false;
            State = CreatureState.Skill;
        }

        //if(Input.GetMouseButtonUp(0))
        //{
        //    isAttacking = false;
        //}

        base.UpdateController();
    }

    protected override void UpdateMoving()
    {
        float moveSpeed = playerMoveSpeed;
        if (IsGrounded() == false)
            moveSpeed = 0.01f;
        controller.Move(playerMoveDir * moveSpeed * Time.deltaTime);
    }

    bool endAttacking = false;
    protected override void UpdateSkill()
    {
        if (Input.GetMouseButtonUp(0))
            endAttacking = true;

        if (endAttacking && !isAttacking)
            State = CreatureState.Idle;
    }

    private void LateUpdate()
    {
        if(isAttacking == false)
            ProcessMove();

        //if (Input.GetKey(KeyCode.Space))
        //{
        //    Jump();
        //}
    }

    void ProcessMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        playerMoveDir = Camera.main.transform.rotation * new Vector3(moveX, 0, moveZ);
        playerMoveDir.y = 0;

        playerMoveDir = playerMoveDir.normalized;
        if (playerMoveDir != Vector3.zero)
        {
            State = CreatureState.Moving;
            if (Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(playerMoveDir))) > 20 || playerSlerping)
            {
                playerSlerping = true;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerMoveDir), 30 * Time.deltaTime);
                if (Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(playerMoveDir))) < 1f)
                    playerSlerping = false;
            }
            else
                transform.rotation = Quaternion.LookRotation(playerMoveDir);
        }
        else
        {
            if(State == CreatureState.Skill)
                return;
            State = CreatureState.Idle;
        }
    }

    void Jump()
    {
        if(IsGrounded())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    protected override bool IsGrounded()
    {
        return Physics.BoxCast(transform.position + Vector3.up, new Vector3(1, 0.1f, 1), -transform.up, transform.rotation, 1.01f);
    }

    protected void SetAttackEnd()
    {
        isAttacking = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (1 << hit.collider.gameObject.layer == LayerMask.GetMask("Ground"))
            return;


    }
}