using Rito.InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
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
    float bowAttackRange = 5f;

    float attackSpeed = 1.5f;
    float jumpAnimSpeed = 1.5f;

    string playerSwordPath = "Weapon/PlayerSword";
    string playerCrossBowPath = "Weapon/PlayerCrossBow";
    string arrowPath = "Weapon/Arrow";

    bool playerSlerping = false;
    bool isMoving = false;

    CharacterController controller;
    CameraController cameraController;
    Dictionary<int, ItemData> itemDataDict;

    WeaponType weaponType = WeaponType.None;
    public WeaponType PlayerWeaponType { 
        get { return weaponType; }
        set 
        {
            if (value == weaponType)
                return;

            StartCoroutine(CoTryChangeWeapon(value));
            weaponType = value;
        } 
    }

    public ArmorItemData[] PlayerArmors { get; set; } = new ArmorItemData[(int)ArmorType.Count];
    public WeaponItemData[] PlayerWeapons { get; set; } = new WeaponItemData[(int)WeaponType.Count];

    [SerializeField]
    Transform playerSpine;
    public Transform PlayerSpine { get {return playerSpine; }}

    Transform statUiHolder;
    Transform inventoryHolder;
    public Inventory InventoryManager { get; set; }

    [SerializeField]
    Transform rightHandWeaponParent;
    Transform targetToAttack;
    GameObject playerWeapon;

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
                if (PlayerWeaponType == WeaponType.None)
                    return;
                rightHandWeaponParent.gameObject.SetActive(true);
            }
            else
            {
                rightHandWeaponParent.gameObject.SetActive(false);
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
                if (PlayerWeaponType == WeaponType.Sword)
                    animator.CrossFade("SWORD_ATTACK", 0.1f);
                else if (PlayerWeaponType == WeaponType.CrossBow)
                    animator.CrossFade("BOW_ATTACK", 0.05f);
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
        cameraController = Camera.main.GetComponent<CameraController>();
        rightHandWeaponParent.gameObject.SetActive(false);

        statUiHolder = Managers.Resource.Instantiate("UI/StatUI/StatUiHolder").transform;
        inventoryHolder = Managers.Resource.Instantiate("UI/Inventory/InvenHolder").transform;
        InventoryManager = inventoryHolder.GetComponentInChildren<Inventory>();
        statUiHolder.gameObject.SetActive(false);
        inventoryHolder.gameObject.SetActive(false);

        animator.SetFloat("AttackSpeed", attackSpeed);
        animator.SetFloat("JumpAnimSpeed", jumpAnimSpeed);
    }

    protected override void Init_Start()
    {
        Managers.Game.SetPlayer(gameObject);

        if(Managers.Scene.CurrentScene is GameScene)
        {
            itemDataDict = Managers.Scene.CurrentScene.GetComponent<GameScene>().ItemDataDict;
            AddInvenItems();
        }
        
        // ID = 1 -> 장검
        if (itemDataDict.TryGetValue(1, out ItemData itemData) == false)
            Debug.LogError("id에 해당하는 아이템이 없습니다");
        var data = itemData as WeaponItemData;
        PlayerWeapons[(int)data.Type] = data;
        PlayerWeaponType = data.Type;
    }

    void AddInvenItems()
    {
        foreach (var itemData in itemDataDict.Values)
        {
            if (itemData is CountableItemData)
                InventoryManager.Add(itemData, 20);
            else
                InventoryManager.Add(itemData, 1);
        }
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
        ProcessMouseInput();
        ProcessKeyInput();
    }

    void ProcessMouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (coSkill == null && Input.GetMouseButton(0) && IsGrounded() && PlayerWeaponType == WeaponType.Sword)
        {
            Vector3 dir = GetXZinputDir();
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            coSkill = StartCoroutine(CoSwordAttack());
        }

        if(coSkill == null && Input.GetMouseButtonDown(0) && IsGrounded())
        {
            if(PlayerWeaponType == WeaponType.CrossBow)
            {
                coSkill = StartCoroutine(CoBowAttack());
            }
        }
       
        if(coDash == null && Input.GetMouseButtonDown(1) && IsGrounded())
        {
            coDash = StartCoroutine(CoDash());
            //coDashCoolDown = StartCoroutine(CoDashCooldown());
        }
    }

    public void ProcessRotation()
    {
        if (State == CreatureState.Skill)
            return;

        Vector3 lookDir = playerMoveDir;
        if (lookDir == Vector3.zero)
            return;
       
        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir)) > 20 || playerSlerping)
        {
            playerSlerping = true;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 30 * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir)) < 1f)
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

    void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // 카메라 시점 전환
            if (cameraController.CamMode == CameraMode.RoundView)
                cameraController.CamMode = CameraMode.FirstPersonView;
            else if(cameraController.CamMode == CameraMode.FirstPersonView)
                cameraController.CamMode= CameraMode.RoundView;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if(inventoryHolder.gameObject.activeSelf)
                inventoryHolder.gameObject.SetActive(false);
            else
                inventoryHolder.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if(statUiHolder.gameObject.activeSelf)
                statUiHolder.gameObject.SetActive(false);
            else
                statUiHolder.gameObject.SetActive(true);
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ChangeWeapon(WeaponType.Sword);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ChangeWeapon(WeaponType.CrossBow);
        //}
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

    void ChangeWeapon(WeaponType targetWeapon)
    {
        if (PlayerWeaponType == targetWeapon)
            return;

        Managers.Resource.Destroy(playerWeapon);
        switch (targetWeapon)
        {
            case WeaponType.None:
                break;
            case WeaponType.Sword:
                playerWeapon = Managers.Resource.Instantiate(playerSwordPath, rightHandWeaponParent);
                break;
            case WeaponType.CrossBow:
                playerWeapon = Managers.Resource.Instantiate(playerCrossBowPath, rightHandWeaponParent);
                break;
        }
    }

    Transform FindTargetToAttack()
    {
        float attackRange = 0f;
        switch (PlayerWeaponType)
        {
            case WeaponType.Sword:
                attackRange = swordAttackRange; 
                break;
            case WeaponType.CrossBow:
                attackRange = bowAttackRange;
                break;
        }

        Collider[] monsters = Physics.OverlapSphere(transform.position + Vector3.up, attackRange, LayerMask.GetMask("Monster"));
        LayerMask mask = 1 << (int)Layer.Block;
        
        foreach(Collider monster in monsters)
        {
            Vector3 delta = monster.transform.position - transform.position;

            if (delta.y > swordAttackRange * Mathf.Sin(Mathf.Deg2Rad * 45) && PlayerWeaponType == WeaponType.Sword)
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
        //Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(0.1f, 0.1f, 0.1f));
    }

    protected override bool IsGrounded()
    {
        LayerMask mask = 1 << (int)Layer.Ground | 1 << (int)Layer.Block;
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.15f, 0.5f, 0.15f), Quaternion.identity, mask);
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
            return;

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

    IEnumerator CoTryChangeWeapon(WeaponType type)
    {
        while(coSkill != null)
        {
            yield return null;
        }

        ChangeWeapon(type);
    }

    IEnumerator CoSwordAttack()
    { 
        targetToAttack = FindTargetToAttack();
        if (targetToAttack != null && cameraController.CamMode == CameraMode.RoundView)
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

    IEnumerator CoBowAttack()
    { 
        targetToAttack = FindTargetToAttack();
        if (targetToAttack != null && cameraController.CamMode == CameraMode.RoundView)
        {
            Vector3 targetVec = targetToAttack.position - transform.position;
            targetVec.y = 0;
            transform.rotation = Quaternion.LookRotation(targetVec);
        }

        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.1f);

        // spawn arrow
        Quaternion arrowRot = transform.rotation * Quaternion.Euler(new Vector3(-5, 0, 0));
        if (cameraController.CamMode == CameraMode.FirstPersonView)
            arrowRot = Camera.main.transform.rotation * Quaternion.Euler(new Vector3(-5, 0, 0));
        GameObject arrow = Managers.Resource.Instantiate(arrowPath, count: 10);
        arrow.transform.position = rightHandWeaponParent.position;
        arrow.transform.rotation = arrowRot;
        ArrowController ac = arrow.GetOrAddComponent<ArrowController>();
        ac.Owner = gameObject;
        ac.Init();

        yield return new WaitForSeconds(0.2f);
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