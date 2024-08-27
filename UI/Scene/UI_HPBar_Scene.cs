using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar_Scene : UI_Scene
{
    enum GameObjects
    {
        HPBar
    }

    PlayerStat stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Start()
    {
        stat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();
    }

    private void Update()
    {
        float ratio = (float)stat.Hp / stat.MaxHp;
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }
}
