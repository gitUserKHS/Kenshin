using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    enum GameObjects
    {
        HPBar
    }

    Stat stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        stat = transform.parent.parent.GetComponent<Stat>();
    }

    private void Start()
    {
        Transform obj = transform.parent;
        transform.position = obj.position;
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;

        float ratio = (float)stat.Hp / stat.MaxHp;
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }
}
