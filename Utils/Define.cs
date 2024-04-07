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
        Monster = 6,
        Ground = 7,
        Block = 8
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
        QuarterView
    }
}
