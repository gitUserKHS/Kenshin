using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{

    protected CreatureState state = CreatureState.Idle;
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

    protected void UpdateAnimation()
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
                break;
        }
    }

    void Awake()
    {
        Init_Awake();
    }

    protected virtual void Init_Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {

    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
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
}
