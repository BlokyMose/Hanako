using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Dialogue
{
    [CreateAssetMenu(fileName ="DPos_", menuName ="SO/Dialogue/Dialogue Char Pos")]

    public class DialogueCharPos : ScriptableObject
    {
        [System.Serializable]
        public class CharPos
        {
            [SerializeField]
            Vector2 pos;

            [SerializeField]
            bool isFacingRight = true;

            public Vector2 Pos { get => pos; }
            public bool IsFacingRight { get => isFacingRight; }
        }

        [SerializeField]
        List<CharPos> allPos = new();

        public List<CharPos> AllPos { get => allPos; }
    }
}
