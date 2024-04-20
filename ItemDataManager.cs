using Rito.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    [SerializeField]
    ItemData[] itemDataArray;

    public ItemData[] ItemDataArray { get { return itemDataArray; } }
}
