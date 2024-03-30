using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class PlayerController : CreatureController
{
    Vector3 playerMoveDir = Vector3.zero;
    Vector3 playerVelocity = Vector3.zero;

    [SerializeField]
    float playerMoveSpeed = 7.0f;
    float gravity = -20f;
    float jumpHeight = 1f;
    float attackSpeed = 1f;

    bool playerSlerping = false;
    bool isJumping = false;

    CharacterController controller;

    [SerializeField]
    Transform weapon;

    Coroutine coSkill = null;

    public override CreatureState State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;

            if (value == CreatureState.Skill)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            state = value;
            UpdateAnimation();
        }
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();

        switch (State)
        {
            case CreatureState.Jumping:
                animator.CrossFade("JUMP", 0.1f);
                break;
        }
    }

    protected override void Init_Awake()
    {
        base.Init_Awake();
        controller = GetComponent<CharacterController>();
        weapon.gameObject.SetActive(false);

        animator.SetFloat("AttackSpeed", attackSpeed);
    }

    protected override void UpdateController()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        if (IsGrounded() && playerVelocity.y < 0)
        {
            if (State == CreatureState.Jumping)
            {
                isJumping = false;
                State = CreatureState.Idle;
            }
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);

        if (coSkill == null && Input.GetMouseButton(0) && IsGrounded())
        {
            coSkill = StartCoroutine(CoStartSwordAttack());
        }

        base.UpdateController();

        switch (State)
        {
            case CreatureState.Jumping:
                UpdateMoving();
                break;
        }
    }

    protected override void UpdateMoving()
    {
        float moveSpeed = playerMoveSpeed;
        controller.Move(playerMoveDir * moveSpeed * Time.deltaTime);
    }


    private void LateUpdate()
    {
        ProcessMove();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void ProcessMove()
    {
        if (coSkill != null)
            return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        playerMoveDir = Camera.main.transform.rotation * new Vector3(moveX, 0, moveZ);
        playerMoveDir.y = 0;

        playerMoveDir = playerMoveDir.normalized;
        if (playerMoveDir != Vector3.zero)
        {
            if (isJumping)
                State = CreatureState.Jumping;
            else
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
            switch (State)
            {
                case CreatureState.Skill:
                case CreatureState.Jumping:
                    return;
            }
            State = CreatureState.Idle;
        }
    }

    void Jump()
    {
        if (IsGrounded())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            State = CreatureState.Jumping;
            isJumping = true;
        }
    }

    protected override bool IsGrounded()
    {
        LayerMask layer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Wall");
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, layer);
        Debug.Log(colliders.Length);
        return colliders.Length != 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (1 << hit.collider.gameObject.layer == LayerMask.GetMask("Ground"))
            return;
    }

    IEnumerator CoStartSwordAttack()
    {
        State = CreatureState.Skill;
        yield return new WaitForSeconds(attackSpeed);
        State = CreatureState.Idle;
        coSkill = null;
    }
}