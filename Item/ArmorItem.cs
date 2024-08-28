using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static Define;

// 날짜 : 2021-03-28 PM 11:06:16
// 작성자 : Rito

namespace Rito.InventorySystem
{
    /// <summary> 장비 - 방어구 아이템 </summary>
    public class ArmorItem : EquipmentItem, IUsableItem
    {
        public ArmorItem(ArmorItemData data) : base(data) { }

        public bool Use()
        {
            ArmorItemData armorItemData = Data as ArmorItemData;
            GameObject player = Managers.Game.GetPlayer();
            PlayerController pc = player.GetComponent<PlayerController>();
 
            int idx = (int)armorItemData.Type;
            if (pc.PlayerArmors[idx] != null)
            {
                pc.InventoryManager.Add(pc.PlayerArmors[idx]);
                pc.OnChangeArmor(idx, false);
                pc.PlayerArmors[idx] = null;
            }
            //if (pc.PlayerArmors[idx] != null)
            //    return false;
            pc.PlayerArmors[idx] = armorItemData;
            pc.OnChangeArmor(idx, true);
            IsUsed = true;
            return true;
        }
    }
}