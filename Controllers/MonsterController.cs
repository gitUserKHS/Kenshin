using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Define;

public class MonsterController : CreatureController
{
    WarriorSpawnPoint warriorSpawnPoint;
    GameObject lockTarget;

    [SerializeField]
    float scanRange = 10;

    [SerializeField]
    float attackRange = 2;

    protected override void Init_Awake()
    {
        base.Init_Awake();
        WorldObjectType = WorldObject.Monster;
        warriorSpawnPoint = transform.parent.GetComponent<WarriorSpawnPoint>();
      
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
        if (player == null || player.activeSelf == false)
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
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            dir = new Vector3(dir.x, 0, dir.z);
            rb.MovePosition(transform.position + dir.normalized * Time.deltaTime * Stat.MoveSpeed);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill()
    {
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z);
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateDie()
    {
        StartCoroutine(CoDead());
    }

    IEnumerator CoDead()
    {
        yield return new WaitForSeconds(3);
        Managers.Resource.Destroy(gameObject);
        warriorSpawnPoint.IsDead = true;
    }
}
