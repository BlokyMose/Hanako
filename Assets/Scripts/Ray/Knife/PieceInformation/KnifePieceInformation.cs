using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName ="KnifePieceInfo_", menuName ="SO/Knife/Piece Information")]

    public class KnifePieceInformation : ScriptableObject
    {
        [SerializeField]
        string pieceName;

        [SerializeField, Multiline]
        string desc;

        [SerializeField, PreviewField]
        Sprite logo;

        public string PieceName { get => pieceName; }
        public string Desc { get => desc; }
        public Sprite Logo { get => logo; }
    }
}
