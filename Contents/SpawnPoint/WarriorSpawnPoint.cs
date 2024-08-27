using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WarriorSpawnPoint : BaseSpawnPoint
{
    string prefabPath = "Creature/Monster/Warrior";
    GameObject warrior;

    public bool IsDead { get; set; } = false;
    public float Radius { get; private set; } = 10.0f;

    Coroutine coSpawn = null;

    private void Awake()
    {
        SpawnWarrior();
    }

    private void SpawnWarrior()
    {
        warrior = Managers.Resource.Instantiate(prefabPath, transform.position, Quaternion.identity, transform);
    }

    void Update()
    {
        if(IsDead && coSpawn == null)
        {
            coSpawn = StartCoroutine(CoSpawnWithCoolDown());
        }
    }

    IEnumerator CoSpawnWithCoolDown()
    {
        //Managers.UI.MakeWorldSpaceUI<UI_SpawnCoolDown>(transform);
        yield return new WaitForSeconds(SpawnCoolTime);
        SpawnWarrior();
        IsDead = false;
        coSpawn = null;
    }
}
