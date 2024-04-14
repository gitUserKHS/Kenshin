using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public WorldObject WorldObjectType { get; protected set; } = WorldObject.Unknown;

    void Awake()
    {
        Init_Awake();
    }

    protected virtual void Init_Awake()
    {

    }

    private void Start()
    {
        Init_Start();
    }

    protected virtual void Init_Start()
    {

    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {

    }
}
