using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpawnCoolDown : UI_Base
{
    float spawnCoolTime;
    float timePassed = 0;

    enum Images
    {
        SpawnCoolDown
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
    }

    private void Start()
    {
        transform.position = transform.parent.position + Vector3.up * 1.5f;
        spawnCoolTime = transform.parent.GetComponent<BaseSpawnPoint>().SpawnCoolTime;
    }

    void Update()
    {
        GetImage((int)Images.SpawnCoolDown).fillAmount = Mathf.Max(1 - timePassed / spawnCoolTime, 0);
        if(GetImage((int)Images.SpawnCoolDown).fillAmount == 0)
            Object.Destroy(gameObject);
        timePassed += Time.deltaTime;
    }
}
