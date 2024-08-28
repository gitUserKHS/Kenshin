using Rito.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameScene : BaseScene
{
    ItemDataManager ItemDataManager;
    public Dictionary<int, ItemData> ItemDataDict { get; private set; } = new Dictionary<int, ItemData>();

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;
        this.GetOrAddComponent<CursorController>();
        ItemDataManager = Managers.Resource.Instantiate("Item/ItemDataManager").GetComponent<ItemDataManager>();
        foreach(var itemData in ItemDataManager.ItemDataArray)
        {
            ItemDataDict.Add(itemData.ID, itemData);
        }

        Managers.Sound.Play("Bgm/game", Define.Sound.Bgm);
    }

    public override void Clear()
    {

    }
}
