using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Define;

public class PlayerController : CreatureController
{
    Vector3 playerMoveDir = Vector3.zero;
    Vector3 playerVelocity = Vector3.zero;

    float playerMoveSpeed = 7.0f;
    float dashSpeed = 0f;
    float gravity = -20f;
    float jumpHeight = 1f;
    float swordAttackRange = 1f;

    float attackSpeed = 1.5f;
    float jumpAnimSpeed = 1.5f;

    bool playerSlerping = false;
    bool isMoving = false;

    CharacterController controller;

    [SerializeField]
    Transform weapon;
    Transform targetToAttack;

    Coroutine coSkill = null;
    Coroutine coJump = null;
    Coroutine coFall = null;
    Coroutine coLand = null;
    Coroutine coDash = null;
    Coroutine coDashCooldown = null;

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

            if (state == CreatureState.Moving || state == CreatureState.Dashing)
                isMoving = true;
            else
                isMoving = false;

            state = value;
            UpdateAnimation();
        }
    }

    protected override void UpdateAnimation()
    {
        switch (State)
        {
            case CreatureState.Idle:
                animator.CrossFade("WAIT", 0.1f);
                break;
            case CreatureState.Moving:
                animator.CrossFade("RUN", 0.1f);
                break;
            case CreatureState.Skill:
                animator.CrossFade("SWORD_ATTACK", 0.1f);
                break;
            case CreatureState.Die:
                break;
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
            case CreatureState.Dashing:
                animator.CrossFade("DASH", 0.01f);
                break;
        }
    }

    protected override void Init_Awake()
    {
        base.Init_Awake();
        WorldObjectType = WorldObject.Player;
        controller = GetComponent<CharacterController>();
        weapon.gameObject.SetActive(false);

        animator.SetFloat("AttackSpeed", attackSpeed);
        animator.SetFloat("JumpAnimSpeed", jumpAnimSpeed);
    }

    protected override void UpdateController()
    {
        base.UpdateController();

        playerVelocity.y += gravity * Time.deltaTime;
        if (IsGrounded() && playerVelocity.y < 0)
        {
            if (coJump != null && coFall == null && coLand == null)
            {
                StopCoroutine(coJump);
                coJump = null;
                State = CreatureState.Idle;
            }
            playerVelocity.y = -5f;
        }
        else if (State != CreatureState.Jumping && State != CreatureState.Falling)
        {
            coFall = StartCoroutine(CoFall());
        }
        controller.Move(playerVelocity * Time.deltaTime);

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
            case CreatureState.Dashing:
                UpdateDashing();
                break;
        }
    }

    protected override void UpdateMoving()
    {
        float moveSpeed = playerMoveSpeed;
        controller.Move(playerMoveDir * moveSpeed * Time.deltaTime);
    }

    protected void UpdateDashing()
    {
        controller.Move(transform.forward * dashSpeed * Time.deltaTime);
        dashSpeed = Mathf.Max(dashSpeed - 0.02f * dashSpeed, playerMoveSpeed);
    }

    protected void UpdateJumping()
    {
        UpdateMoving();
    }

    protected void UpdateFalling()
    {
        if (IsGrounded())
        {
            coLand = StartCoroutine(CoLand());
        }
        UpdateMoving();
    }

    public void HandleCharacterInput()
    {
        ProcessMove();
        ProcessRotation();
        ProcessMouseInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void ProcessMouseInput()
    {
        if (coSkill == null && Input.GetMouseButton(0) && IsGrounded())
        {
            Vector3 dir = GetXZinputDir();
            if(dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
            coSkill = StartCoroutine(CoSwordAttack());
        }
       
        if(coDash == null && Input.GetMouseButtonDown(1) && IsGrounded())
        {
            coDash = StartCoroutine(CoDash());
            //coDashCoolDown = StartCoroutine(CoDashCooldown());
        }
    }

    void ProcessRotation()
    {
        if (State == CreatureState.Skill)
            return;

        Vector3 lookDir = playerMoveDir;
        if (lookDir == Vector3.zero)
            return;
       
        if (Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir))) > 20 || playerSlerping)
        {
            playerSlerping = true;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 30 * Time.deltaTime);
            if (Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir))) < 1f)
                playerSlerping = false;
        }
        else
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    Vector3 GetXZinputDir()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 dir = Camera.main.transform.rotation * new Vector3(moveX, 0, moveZ);
        dir.y = 0;
        return dir;
    }

    void ProcessMove()
    {
        if (coSkill != null)
            return;

        playerMoveDir = GetXZinputDir().normalized;
        if (playerMoveDir != Vector3.zero)
        {
            if (State == CreatureState.Idle || State == CreatureState.Landing)
                State = CreatureState.Moving;
        }
        else
        {
            if(State == CreatureState.Moving)
                State = CreatureState.Idle;
        }
    }

    void Jump()
    {
        if (State == CreatureState.Jumping || State == CreatureState.Falling)
            return;

        if (IsGrounded())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            if(coJump != null)
                StopCoroutine(coJump);
            coJump = StartCoroutine(CoJump());
        }
    }

    Transform FindSwordAttackTarget()
    {
        Collider[] monsters = Physics.OverlapSphere(transform.position + Vector3.up, swordAttackRange, LayerMask.GetMask("Monster"));
        LayerMask mask = 1 << (int)Layer.Block;
        
        foreach(Collider monster in monsters)
        {
            Vector3 delta = monster.transform.position - transform.position;

            if (delta.y > swordAttackRange * Mathf.Sin(Mathf.Deg2Rad * 45))
                continue;

            if(Physics.Raycast(transform.position + Vector3.up, delta, delta.magnitude, mask) == false)
            {
                return monster.transform;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position + Vector3.up, swordAttackRange);
    }

    protected override bool IsGrounded()
    {
        LayerMask mask = 1 << (int)Layer.Ground | 1 << (int)Layer.Block;
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.1f, 0.5f, 0.1f), Quaternion.identity, mask);
        return colliders.Length != 0;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (1 << hit.collider.gameObject.layer == 1 << (int)Layer.Ground)
            return;
    }

    void OnAttackEvent()
    {
        if(targetToAttack == null)
        {
            Debug.Log("cannot find targetToAttack");
            return;
        }
        LayerMask mask = 1 << (int)Layer.Block;
        Vector3 delta = targetToAttack.transform.position - transform.position;
        if (delta.magnitude > swordAttackRange + 0.5f)  // 길이를 0.5만큼 보정
        {
            Debug.Log("attackRange is too short");
            return;
        }
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, swordAttackRange, mask))
            return;

        CreatureController cc = targetToAttack.GetComponent<CreatureController>();
        cc.OnDamaged(gameObject, 0);
    }

    IEnumerator CoSwordAttack()
    {
        targetToAttack = FindSwordAttackTarget();
        if (targetToAttack != null)
        {
            Vector3 targetVec = targetToAttack.position - transform.position;
            targetVec.y = 0;
            transform.rotation = Quaternion.LookRotation(targetVec);
        }
        State = CreatureState.Skill;
        yield return new WaitForSeconds(1 / attackSpeed);
        State = CreatureState.Idle;
        coSkill = null;
    }

    IEnumerator CoJump()
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
        yield return new WaitForSeconds(0.3f);
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

    IEnumerator CoDash()
    {
        if (coDashCooldown != null)
            yield break;

        dashSpeed = 30f;
        State = CreatureState.Dashing;
        yield return new WaitForSeconds(0.5f);
        if(State == CreatureState.Dashing)
            State = CreatureState.Idle;
        coDashCooldown = StartCoroutine(CoDashCooldown());
        coDash = null;
    }

    IEnumerator CoDashCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        coDashCooldown = null;
    }
}