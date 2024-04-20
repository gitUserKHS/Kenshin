using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameScene : BaseScene
{
    public ItemDataManager ItemDataManager { get; private set; }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;
        this.GetOrAddComponent<CursorController>();
        ItemDataManager = Managers.Resource.Instantiate("Item/ItemDataManager").GetComponent<ItemDataManager>();
        
    }

    public override void Clear()
    {

    }
}
