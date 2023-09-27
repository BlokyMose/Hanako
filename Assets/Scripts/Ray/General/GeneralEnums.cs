using System.Collections;
using System.Collections.Generic;

namespace Hanako
{
    public enum GameType { Hub = -1, Hanako, Knife, TekeTeke }

    public enum CharacterMotion { 
        Die = -1, 
        Idle = 0, 
        Run = 1, 
        Attack = 2, 
        Pushed = 3, 
        Scared = 11, 
        Stiffed = 12, 
        PointingScared = 13,
        WaveMuch = 14,
        WashHands = 15,
        Think = 16,
        
    }

    public enum SoulIconState { Dead, Alive }
    public enum SolidButtonState { Pressed, Idle, Hover }
    public enum LevelInfoInitMode { SceneLoadingData, LevelInfo, LevelProperties }
    
}
