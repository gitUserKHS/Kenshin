using System.Collections;
using System.Collections.Generic;
using System.Resources;
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
    float jumpAnimSpeed = 1.5f;

    bool playerSlerping = false;
    bool isMoving = false;

    CharacterController controller;

    [SerializeField]
    Transform weapon;

    Coroutine coSkill = null;
    Coroutine coJump = null;
    Coroutine coFall = null;
    Coroutine coLand = null;

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
                if (isMoving == false)
                    animator.CrossFade("JUMP_STOP", 0.1f);
                else
                    animator.CrossFade("JUMP_MOVE", 0.3f);
                break;
            case CreatureState.Falling:
                animator.CrossFade("FALL", 0.1f);
                break;
            case CreatureState.Landing:
                animator.CrossFade("LAND", 0.1f);
                break;
        }
    }

    protected override void Init_Awake()
    {
        base.Init_Awake();
        controller = GetComponent<CharacterController>();
        weapon.gameObject.SetActive(false);

        animator.SetFloat("AttackSpeed", attackSpeed);
        animator.SetFloat("JumpAnimSpeed", jumpAnimSpeed);
    }

    protected override void UpdateController()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        if (IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = -5f;
        }
        else if (State != CreatureState.Jumping && State != CreatureState.Falling)
        {
            coFall = StartCoroutine(CoFall());
        }
        controller.Move(playerVelocity * Time.deltaTime);

        ProcessMouseInput();

        base.UpdateController();

        switch (State)
        {
            case CreatureState.Jumping:
                UpdateJumping();
                break;
            case CreatureState.Falling:
                UpdateFalling();
                break;
            case CreatureState.Landing:
                UpdateMoving();
                break;
        }
    }

    protected override void UpdateMoving()
    {
        float moveSpeed = playerMoveSpeed;
        controller.Move(playerMoveDir * moveSpeed * Time.deltaTime);
    }

    protected void UpdateJumping()
    {
        controller.Move(playerMoveDir * playerMoveSpeed * Time.deltaTime);
    }

    protected void UpdateFalling()
    {
        if (IsGrounded())
        {
            coLand = StartCoroutine(CoLand());
        }
        controller.Move(playerMoveDir * playerMoveSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        ProcessMove();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void ProcessMouseInput()
    {
        if (coSkill == null && Input.GetMouseButton(0) && IsGrounded())
        {
            coSkill = StartCoroutine(CoStartSwordAttack());
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
            isMoving = true;
            if (State == CreatureState.Idle || State == CreatureState.Landing)
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
            isMoving = false;
            if(State == CreatureState.Moving)
                State = CreatureState.Idle;
        }
    }

    void Jump()
    {
        if (IsGrounded())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            coJump = StartCoroutine(CoJumpCooldown());
        }
    }

    protected override bool IsGrounded()
    {
        LayerMask layer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Wall");
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.3f, 0.5f, 0.3f), Quaternion.identity, layer);
        return colliders.Length != 0;
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

    IEnumerator CoJumpCooldown()
    {
        State = CreatureState.Jumping;
        yield return new WaitForSeconds(1 / jumpAnimSpeed);
        if (IsGrounded())
        {
            //isJumping = false;
            State = CreatureState.Idle;
        }
        else 
        {
            State = CreatureState.Falling;
        }
        coJump = null;
    }

    IEnumerator CoFall()
    {
        yield return new WaitForSeconds(0.2f);
        if(IsGrounded() == false)
            State = CreatureState.Falling;
        coFall = null;
    }

    IEnumerator CoLand()
    {
        State = CreatureState.Landing;
        yield return new WaitForSeconds(1f);
        if(State == CreatureState.Landing)
            State = CreatureState.Idle;
        coLand = null;
    }
}