using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-28 PM 11:02:03
// 작성자 : Rito

namespace Rito.InventorySystem
{
    /// <summary> 장비 - 무기 아이템 </summary>
    public class WeaponItem : EquipmentItem, IUsableItem
    {
        public WeaponItem(WeaponItemData data) : base(data) { }

        public bool Use()
        {
            WeaponItemData weaponItemData = Data as WeaponItemData;
            GameObject player = Managers.Game.GetPlayer();
            PlayerController pc = player.GetComponent<PlayerController>();

            int idx = (int)weaponItemData.Type;
            if (pc.PlayerWeapons[idx] != null)
                return false;
            pc.PlayerWeapons[idx] = weaponItemData;
            pc.PlayerWeaponType = weaponItemData.Type;
            IsUsed = true;
            return true;
        }
    }
}