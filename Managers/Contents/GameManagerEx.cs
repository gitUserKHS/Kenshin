using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    GameObject player;

    public GameObject GetPlayer() { return player; }

    public void SetPlayer(GameObject p) { player = p; }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Monster:
                {
                    MonsterController mc = go.GetComponent<MonsterController>();
                    mc.State = Define.CreatureState.Die;
                }
                break;
            case Define.WorldObject.Player:
                {
                    PlayerController pc = go.GetComponent<PlayerController>();
                    pc.State = Define.CreatureState.Die;
                    player.SetActive(false);
                }
                break;
        }
    }
}
