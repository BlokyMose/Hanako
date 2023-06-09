using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName ="KnifePieces_", menuName ="SO/Knife/Pieces Pattern")]

    public class KnifePiecesPattern : ScriptableObject
    {
        [System.Serializable]
        public class PieceProperties
        {
            [SerializeField]
            GameObject prefab;

            [SerializeField]
            ColRow colRow;

            public GameObject Prefab { get => prefab; }
            public ColRow ColRow { get => colRow; }
        }

        [SerializeField]
        string patternName;

        [SerializeField]
        ColRow playerColRow = new(0,0);

        [SerializeField]
        List<PieceProperties> pieces = new();

        public List<PieceProperties> Pieces { get => pieces; }

        public ColRow PlayerColRow { get => playerColRow; }
    }
}
