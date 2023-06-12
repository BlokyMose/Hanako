using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Hanako.Knife
{
    [InlineEditor]

    [CreateAssetMenu(fileName ="KnifeLvl_", menuName ="SO/Knife/Knife Level")]

    public class KnifeLevel : ScriptableObject
    {
        [System.Serializable]
        public struct ColRow
        {
            public int col;
            public int row;

            public ColRow(int col, int row)
            {
                this.col = col;
                this.row = row;
            }

            public bool IsEqual(ColRow otherColRow)
            {
                return this.col == otherColRow.col && this.row == otherColRow.row;
            }

            public void Add(ColRow colRow)
            {
                col += colRow.col;
                row += colRow.row;
            }

            public static ColRow AddBetween(ColRow a, ColRow b)
            {
                return new ColRow(a.col + b.col, a.row + b.row);
            }

            public static ColRow SubstractBetween(ColRow a, ColRow b)
            {
                return new ColRow(a.col - b.col, a.row - b.row);
            }
        }


        [SerializeField]
        string levelName;

        [Header("Map")]
        [SerializeField]
        ColRow levelSize;

        [SerializeField]
        Vector2 tileSize = new(2.55f, 1.275f);

        [SerializeField]
        Vector2 originOffset = new(0, 0);

        [SerializeField]
        KnifeTilesPattern tilesPattern;

        [SerializeField]
        KnifeWallsPattern wallsPattern;

        [Header("Game")]
        [SerializeField]
        int roundCount;

        [SerializeField]
        KnifePiecesPattern piecesPattern;

        public ColRow LevelSize { get => levelSize;}
        public KnifeTilesPattern TilesPattern { get => tilesPattern; }
        public KnifeWallsPattern WallsPattern { get => wallsPattern; }
        public Vector2 TileSize { get => tileSize; }
        public Vector2 OriginOffset { get => originOffset; }
        public KnifePiecesPattern PiecesPattern { get => piecesPattern; }
        public int RoundCount { get => roundCount;  }
    }
}
