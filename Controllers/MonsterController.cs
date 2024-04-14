using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    WarriorSpawnPoint warriorSpawnPoint;

    protected override void Init_Awake()
    {
        base.Init_Awake();
        warriorSpawnPoint = transform.parent.GetComponent<WarriorSpawnPoint>();
    }

    public override void OnDamaged(GameObject attacker, int damage)
    {
        OnDead();
    }

    public override void OnDead()
    {
        Managers.Resource.Destroy(gameObject);
        warriorSpawnPoint.IsDead = true;
        State = CreatureState.Die;
    }
}
