using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Die_UI : UI_Popup
{
    public void RespawnPlayer()
    {
        Managers.UI.CloseAllPopupUI();

        PlayerController pc = Managers.Game.GetPlayer().GetComponent<PlayerController>();

        pc.Stat.RestoreHP(pc.Stat.MaxHp);
        pc.State = CreatureState.Idle;
    }
}
