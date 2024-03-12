using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    enum Buttons
    {
        PointButton
    }

    enum Texts
    {
        PointText,
        ScoreText
    }

    enum GameObjects
    {
        TestObject
    }

    enum Images
    {
        ItemIcon
    }

    public override void Init()
    {
        base.Init();

        Bind<UnityEngine.UI.Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        GetText((int)Texts.ScoreText).text = "Bind Test";

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);

        GameObject go1 = GetButton((int)Buttons.PointButton).gameObject;
        BindEvent(go1, OnButtonClicked, Define.UIEvent.Click);
    }

    int score = 0;

    public void OnButtonClicked(PointerEventData data)
    {
        score++;
        GetText((int)Texts.ScoreText).text = $"score: {score}";
    }

}
