using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WarriorSpawnPoint : MonoBehaviour
{
    float spawnCoolTime = 10.0f;
    string prefabPath = "Prefabs/Creature/Monster/Warrior";
    GameObject warrior;

    public bool IsDead { get; set; } = false;

    Coroutine coSpawn = null;

    private void Awake()
    {
        SpawnWarrior();
    }

    private void SpawnWarrior()
    {
        warrior = Managers.Resource.Load<GameObject>(prefabPath);
        Object.Instantiate<GameObject>(warrior, transform.position, Quaternion.identity, transform);
    }

    void Update()
    {
        if(IsDead && coSpawn == null)
        {
            coSpawn = StartCoroutine(CoSpawnWithCoolDown());
        }
        Debug.Log(warrior);
    }

    IEnumerator CoSpawnWithCoolDown()
    {
        yield return new WaitForSeconds(spawnCoolTime);
        SpawnWarrior();
        IsDead = false;
        coSpawn = null;
    }
}
