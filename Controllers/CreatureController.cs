using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    protected CreatureState state = CreatureState.Idle;
    public Stat Stat { get; set; }

    public virtual CreatureState State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;
            state = value;
            UpdateAnimation();
        }
    }

    protected Animator animator;

    protected virtual void UpdateAnimation()
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
                animator.CrossFade("ATTACK", 0.1f);
                break;
            case CreatureState.Die:
                animator.CrossFade("DIE", 0.1f);
                break;
        }
    }


    protected override void Init_Awake()
    {
        base.Init_Awake();
        animator = GetComponent<Animator>();
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Die:
                UpdateDie();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateDie() { }

    protected virtual bool IsGrounded()
    {
        return true;
    }

    public virtual void OnDamaged(GameObject attacker, int damage)
    {

    }

    public virtual void OnDead()
    {

    }
}
