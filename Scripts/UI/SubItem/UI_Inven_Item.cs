using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText
    }

    string name;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        var tmpGo = this.gameObject;
        BindEvent(tmpGo, (PointerEventData data) => { Debug.Log($"item clicked : {name}"); }, Define.UIEvent.Click);
    }

    public void SetInfo(string name)
    {
        this.name = name;
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<TMP_Text>().text = name;
    }
}
