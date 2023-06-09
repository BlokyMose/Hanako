using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityUtility;
using static Hanako.Knife.KnifeLevel;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Hanako.Knife
{
    public class KnifeLevelManager : MonoBehaviour
    {
        [System.Serializable]
        public class TileCache
        {
            ColRow position;
            GameObject go;
            SpriteRenderer sr;

            public TileCache(ColRow position, GameObject go, SpriteRenderer sr)
            {
                this.position = position;
                this.go = go;
                this.sr = sr;
            }

            public ColRow ColRow { get => position; }
            public GameObject GO { get => go; }
            public SpriteRenderer SR { get => sr;  }
        }

        [System.Serializable]
        public class WallCache
        {
            int index;
            GameObject go;
            SpriteRenderer sr;

            public WallCache(int index, GameObject go, SpriteRenderer sr)
            {
                this.index = index;
                this.go = go;
                this.sr = sr;
            }

            public int ColRow { get => index; }
            public GameObject GO { get => go; }
            public SpriteRenderer SR { get => sr; set => sr = value; }
        }

        [SerializeField]
        KnifeLevel levelProperties;

        [Header("Components")]
        [SerializeField]
        Transform levelPos;

        [Header("Debug")]
        [SerializeField]
        GameObject TileColRowCanvas;

        List<TileCache> tiles = new();
        List<WallCache> leftWalls = new();
        List<WallCache> rightWalls = new();

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            GenerateLevelMap();
        }

        [HorizontalGroup("Buts")]
        [Button("Make Map")]
        public void GenerateLevelMap()
        {
            levelPos.DestroyImmediateChildren();
            tiles = new();
            var tileSize = levelProperties.TileSize;
            var offset = levelProperties.OriginOffset;

            for (int row = 0; row < levelProperties.LevelSize.row; row++)
            {
                for (int col = 0; col < levelProperties.LevelSize.col; col++)
                {
                    var tileGO = Instantiate(levelProperties.TilesPattern.GetTile(new(col,row), levelProperties));
                    tileGO.name = $"{row}, c{col}";
                    tileGO.transform.parent = levelPos;
                    tileGO.transform.localPosition = new((col * tileSize.x / 2) - (row * tileSize.x / 2), (row * tileSize.y / 2) + (col * tileSize.y / 2));
                    tileGO.transform.localPosition += (Vector3) offset;
                    
                    var sr = tileGO.GetComponentInFamily<SpriteRenderer>();
                    sr.sortingOrder = -row - col;

                    var tileComponent = tileGO.GetComponent<KnifeTile>();
                    if (tileComponent == null)
                        tileComponent = tileGO.AddComponent<KnifeTile>();

                    tiles.Add(new TileCache(new(col, row), tileGO, sr));
                }
            }

            GenerateLevelWalls();
        }

        public void GenerateLevelWalls()
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning("Generate level map first before generate walls");
                return;
            }

            const string WALLS = "__WALLS__";
            float wallYOffset = levelProperties.TileSize.y * 1 / 4;
            var previousWallsParent = transform.Find(WALLS);
            if (previousWallsParent != null) DestroyImmediate(previousWallsParent.gameObject);   

            var mostTopTile = tiles.GetLast();
            var wallsParent = new GameObject("__WALLS__");
            var sortingGroup = wallsParent.AddComponent<SortingGroup>();
            sortingGroup.sortingOrder = mostTopTile.SR.sortingOrder - 1;
            wallsParent.transform.parent = levelPos;
            wallsParent.transform.position = mostTopTile.GO.transform.position;


            for (int col = 0; col < levelProperties.LevelSize.col; col++)
            {
                var wallGO = Instantiate(levelProperties.WallsPattern.GetLeftWall(col, levelProperties));
                wallGO.name = "Left_" + col;
                wallGO.transform.parent = wallsParent.transform;
                wallGO.transform.localPosition = new(-(levelProperties.TileSize.x/2*(col+1)), -levelProperties.TileSize.y/2*col - wallYOffset);
                var sr = wallGO.GetComponentInFamily<SpriteRenderer>();
                sr.color = levelProperties.WallsPattern.LeftWallColor;
                leftWalls.Add(new(col, wallGO, sr));
            }


            for (int row = 0; row < levelProperties.LevelSize.row; row++)
            {
                var wallGO = Instantiate(levelProperties.WallsPattern.GetRightWall(row, levelProperties));
                wallGO.transform.localScale = new(-wallGO.transform.localScale.x, wallGO.transform.localScale.y);
                wallGO.name = "Right_" + row;
                wallGO.transform.parent = wallsParent.transform;
                wallGO.transform.localPosition = new((levelProperties.TileSize.x /2* (row + 1)), -levelProperties.TileSize.y/2 * row - wallYOffset);
                var sr = wallGO.GetComponentInFamily<SpriteRenderer>();
                sr.color = levelProperties.WallsPattern.RightWallColor;
                rightWalls.Add(new(row, wallGO, sr));
            }
        }

        [HorizontalGroup("Buts")]
        [Button("Make ColRowTexts")]
        public void GenerateColRowTexts()
        {
            const string DEBUG_TEXT = "__DEBUG_TEXT__";
            if (tiles.Count == 0) GenerateLevelMap();

            foreach (var cell in tiles)
            {
                var previousText = cell.GO.transform.Find(DEBUG_TEXT);
                if (previousText != null)
                    DestroyImmediate(previousText.gameObject);

                var canvas = Instantiate(TileColRowCanvas, cell.GO.transform);
                canvas.name = DEBUG_TEXT;
                var textComponent = canvas.GetComponentInFamily<TextMeshProUGUI>();
                textComponent.text = $"{cell.ColRow.row}, c{cell.ColRow.col}";
                if (cell.ColRow.row % 2 == 0)
                    textComponent.color = Color.yellow;
            }
        }
    }
}
