using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster
    }

    public enum ArmorType
    {
        Helmet,
        Chestplate,
        Leggings,
        Boots,
        Count
    }

    public enum WeaponType
    {
        None,
        Sword,
        CrossBow,
        Count
    }

    public enum CreatureState
    {
        Die,
        Moving,
        Idle,
        Skill,
        Jumping,
        Falling,
        Landing,
        Dashing
    }

    public enum Layer
    {
        Block = 3,
        Ground = 6,
        Monster = 7
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game
    }
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }
    public enum UIEvent 
    { 
        Click,
        Drag
    }
    public enum MouseEvent
    {
        Press,
        Click,
        PointerDown,
        PointerUp
    }

    public enum CameraMode
    {
        RoundView,
        FirstPersonView
    }
}
