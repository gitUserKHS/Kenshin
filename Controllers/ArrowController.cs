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
            Stat opponentStat = other.gameObject.GetComponent<Stat>();
            Stat ownerStat = Owner.GetComponent<CreatureController>().Stat;

            WorldObject objType = Managers.Game.GetWorldObjectType(Owner);

            switch (objType)
            {
                case WorldObject.Player:
                    opponentStat.OnAttacked(ownerStat, Owner.GetComponent<PlayerController>().AdditionalDmg);
                    break;

                default:
                    opponentStat.OnAttacked(ownerStat, 0);
                    break;
            }
                
        }
        Managers.Resource.Destroy(gameObject);
    }
}
