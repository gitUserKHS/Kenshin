using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Define;

public class WarriorController : CreatureController
{
    WarriorSpawnPoint warriorSpawnPoint;
    GameObject lockTarget;

    Rigidbody rb;

    float scanRange;
    float attackSpeed = 1.5f;
    float dieAnimSpeed = 1f;

    [SerializeField]
    float attackRange = 2f;

    Coroutine coDead = null;
    Coroutine coReturn = null;
    Coroutine coAttack = null;

    protected override void Init_Awake()
    {
        base.Init_Awake();
        WorldObjectType = WorldObject.Monster;
        warriorSpawnPoint = transform.parent.GetComponent<WarriorSpawnPoint>();
        rb = GetComponent<Rigidbody>();
        animator.SetFloat("AttackSpeed", attackSpeed);
        animator.SetFloat("DieAnimSpeed", dieAnimSpeed);

        scanRange = warriorSpawnPoint.Radius;

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform.Find("UI_Holder"));
    }

    protected override void Init_Start()
    {
        base.Init_Start();
        Stat = gameObject.GetComponent<Stat>();
    }

    protected override void UpdateIdle()
    {
        GameObject player = Managers.Game.GetPlayer();
        if (player == null || player.activeSelf == false || player.GetComponent<PlayerController>().State == CreatureState.Die)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= scanRange)
        {
            lockTarget = player;
            State = CreatureState.Moving;
            return;
        }

    }

    protected override void UpdateMoving()
    {
        if (lockTarget == null || coReturn != null)
            return;

        // 플레이어가 내 사정거리보다 가가우면 공격
        Vector3 destPos = lockTarget.transform.position;
        Vector3 dir = destPos - transform.position;
        if (lockTarget != null)
        {
            destPos = lockTarget.transform.position;
            float distance = (destPos - transform.position).magnitude;
            if (distance <= attackRange)
            {
                State = CreatureState.Skill;
                return;
            }
        }

        // 이동
        if (dir.magnitude < 0.1f)   // 변위가 매우 작은 값이면 이동하지 않음 (float 연산 오류 가능성 예방)
        {
            State = CreatureState.Idle;
        }
        else if ((transform.position - warriorSpawnPoint.transform.position).magnitude > warriorSpawnPoint.Radius)
        {
            lockTarget = null;
            coReturn = StartCoroutine(CoReturn());
        }
        else
        {
            dir = new Vector3(dir.x, 0, dir.z);
            rb.MovePosition(transform.position + dir.normalized * Time.deltaTime * Stat.MoveSpeed);
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    protected override void UpdateSkill()
    {
        CreatureState targetState = lockTarget.GetComponent<CreatureController>().State;

        if (targetState == CreatureState.Die)
        {
            State = CreatureState.Idle;
            return;
        }

        if(lockTarget != null && (lockTarget.transform.position - transform.position).magnitude > attackRange)
            State = CreatureState.Idle;
        else if(coAttack == null)
            coAttack = StartCoroutine(CoAttack());
    }

    void OnAttackEvent()
    {
        if (lockTarget == null)
            return;

        LayerMask mask = 1 << (int)Layer.Block;
        Vector3 delta = lockTarget.transform.position - transform.position;
        if (delta.magnitude > attackRange)
        {
            Debug.Log("attackRange is too short");
            return;
        }

        // 앞에 장애물이 가로막고 있는지 확인
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, attackRange, mask))
            return;

        Stat creatureStat = lockTarget.GetComponent<CreatureController>().Stat;
        creatureStat.OnAttacked(Stat, 0);
    }

    protected override void UpdateDie()
    {
        if (coDead == null)
        {
            StopAllCoroutines();
            coDead = StartCoroutine(CoDead());
        }
    }

    IEnumerator CoReturn()
    {
        while (Mathf.Abs((transform.position - warriorSpawnPoint.transform.position).magnitude) >= 0.5f)
        {
            Vector3 dir= warriorSpawnPoint.transform.position - transform.position;

            rb.MovePosition(transform.position + dir.normalized * Time.deltaTime * Stat.MoveSpeed);
            transform.rotation = Quaternion.LookRotation(dir);

            yield return null;
        }

        State = CreatureState.Idle;
        Stat.RestoreHP(Stat.MaxHp - Stat.Hp);
        coReturn = null;
    }

    IEnumerator CoAttack()
    {
        transform.rotation = Quaternion.LookRotation(lockTarget.transform.position - transform.position);
        animator.Play("ATTACK");
        yield return new WaitForSeconds(1 / attackSpeed);
        coAttack = null;
    }

    IEnumerator CoDead()
    {
        yield return new WaitForSeconds(1.333f / dieAnimSpeed);
        Managers.Resource.Destroy(gameObject);
        warriorSpawnPoint.IsDead = true;
    }
}
