using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [InlineEditor]
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

            public PieceProperties(GameObject prefab, ColRow colRow)
            {
                this.prefab = prefab;
                this.colRow = colRow;
            }

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

        public void SetPiecesPattern(ColRow playerColRow, List<PieceProperties> pieces)
        {
            this.playerColRow = playerColRow;
            this.pieces = new();
            foreach (var piece in pieces)
                this.pieces.Add(piece);
        }
    }
}
