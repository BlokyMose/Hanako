using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
        }

        [System.Serializable]
        public class CharacterProperties
        {
            public GameObject prefab;
            public ColRow position;
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
        public int roundCount;

        [SerializeField]
        public CharacterProperties player = new();

        [SerializeField]
        public List<CharacterProperties> enemies = new();

        [SerializeField]
        public List<CharacterProperties> objects = new();

        public ColRow LevelSize { get => levelSize;}
        public KnifeTilesPattern TilesPattern { get => tilesPattern; }
        public KnifeWallsPattern WallsPattern { get => wallsPattern; }
        public Vector2 TileSize { get => tileSize; }
        public Vector2 OriginOffset { get => originOffset; }
    }
}
