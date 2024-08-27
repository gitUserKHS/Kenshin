using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 날짜 : 2021-03-28 PM 11:07:23
// 작성자 : Rito

namespace Rito.InventorySystem
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            GameObject player = Managers.Game.GetPlayer();
            PlayerController pc = player.GetComponent<PlayerController>();
            PortionItemData data = Data as PortionItemData;

            if (pc.Stat.Hp >= pc.Stat.MaxHp)
                return false;

            pc.Stat.RestoreHP(data.Value);

            Amount--;
            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}