using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : MonoBehaviour
{
    public GameObject Owner { get; set; }

    float speed = 30.0f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init()
    {
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Owner)
            return;

        LayerMask mask = 1 << (int)Layer.Monster;
        if(mask == 1 << other.gameObject.layer)
        {
            MonsterController mc = other.gameObject.GetComponent<MonsterController>();
            mc.OnDamaged(Owner, 5);
        }
        Managers.Resource.Destroy(gameObject);
    }
}
