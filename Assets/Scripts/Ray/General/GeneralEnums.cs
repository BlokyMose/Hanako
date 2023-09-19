using System.Collections;
using System.Collections.Generic;

namespace Hanako
{
    public enum CharacterMotion { 
        Die = -1, 
        Idle, 
        Run, 
        Attack, 
        Pushed, 
        Scared = 11, 
        Stiffed = 12, 
        PointingScared = 13 
    }

    public enum SoulIconState { Dead, Alive }
    public enum SolidButtonState { Pressed, Idle, Hover }
    
}
