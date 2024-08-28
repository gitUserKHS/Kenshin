using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int exp;
    [SerializeField]
    protected int gold;

    public int Exp { 
        get 
        {
            return exp; 
        }
        set
        { 
            exp = value;
            // 레벨업 체크

            int level = Level;
            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;
                if (exp < stat.totalExp)
                    break;
                level++;
            }

            if(level != Level)
            {
                Debug.Log("Level Up!");
                Level = level;
                SetStat(Level);
            }
        }
    }
    public int Gold { get { return gold; } set { gold = value; } }

    private void Awake()
    {
        Init_Awake();
    }

    protected override void Init_Awake()
    {
        base.Init_Awake();

        level = 1;
        defense = 5;
        moveSpeed = 7.0f;
        exp = 0;
        gold = 0;

        SetStat(level);
    }

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];

        hp = stat.maxHp;
        maxHp = stat.maxHp;
        attack = stat.attack;
    }

    public override void OnAttacked(Stat attacker, int extraAtk = 0)
    {
        if (Hp <= 0)
            return;

        Managers.Sound.Play("hit");
        base.OnAttacked(attacker, extraAtk);
    }

    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
        Managers.Game.Despawn(gameObject);
    }
}
