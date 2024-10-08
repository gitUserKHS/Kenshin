using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int level;
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected int maxHp;

    [SerializeField]
    protected int attack;
    [SerializeField]
    protected int defense;

    [SerializeField]
    protected float moveSpeed;

    public int Level { get { return level; } set { level = value; } }
    public int Hp { get { return hp; } set { hp = value; } }
    public int MaxHp { get { return maxHp; } set { maxHp = value; } }
    public int Attack { get { return attack; } set { attack = value; } }
    public int Defense { get { return defense; } set { defense = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    private void Awake()
    {
        Init_Awake();
    }

    protected virtual void Init_Awake()
    {
        level = 1;
        hp = 100;
        maxHp = 100;
        attack = 10;
        defense = 5;
        moveSpeed = 5.0f;
    }

    public void RestoreHP(int hp)
    {
        if (Hp + hp > MaxHp)
            Hp = MaxHp;
        else
            Hp += hp;
    }

    public virtual void OnAttacked(Stat attacker, int extraAtk = 0)
    {
        if (Hp <= 0)
            return;

        int damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage;

        if(Hp <= 0)
        {
            Hp = 0;
            OnDead(attacker);
        }
    }

    protected virtual void OnDead(Stat attacker)
    {
        Define.WorldObject type = Managers.Game.GetWorldObjectType(attacker.gameObject);
        if(type == Define.WorldObject.Player)
        {
            PlayerStat playerStat = attacker as PlayerStat;
            if (playerStat != null)
            {
                playerStat.Exp += 5;
            }
        }

        Managers.Game.Despawn(gameObject);
    }
}
