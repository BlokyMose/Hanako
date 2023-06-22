using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public class KnifePiecesPatternMapper : OdinEditorWindow
    {
        #region [Editor]

        [MenuItem("Tools/Knife Pieces Mapper")]
        public static void OpenWindow()
        {
            GetWindow<KnifePiecesPatternMapper>("Map").Show();
        }

        public static void OpenWindow(TextAsset textAsset)
        {
            var window = GetWindow<KnifePiecesPatternMapper>("Map");
            window.Show();
        }

        #endregion

        [System.Serializable]
        public class PieceColRow
        {
            public KnifePiece piece;
            public ColRow colRow;
            public TileCache tile;

            public PieceColRow(KnifePiece piece, TileCache tile, ColRow colRow)
            {
                this.piece = piece;
                this.tile = tile;
                this.colRow = colRow;
            }
        }

        public float accuraccy = 1f;
        public List<PieceColRow> pieceMap = new();
        KnifeLevelManager levelManager;
        const string PIECES = "__PIECES__";

        protected override void Initialize()
        {
            base.Initialize();
            levelManager = FindObjectOfType<KnifeLevelManager>();
        }

        [Button]
        public void Init()
        {
            if (levelManager == null) return;
            Debug.Log("Init");
            levelManager.GenerateLevelMap();
            levelManager.GenerateColRowTexts();
            GeneratePieces();


            void GeneratePieces()
            {
                GameObject piecesGO = null;
                var allGOs = new List<GameObject>(FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
                foreach (var go in allGOs)
                {
                    if (go.gameObject.name == PIECES)
                    {
                        piecesGO = go;
                        piecesGO.transform.DestroyImmediateChildren();
                        break;
                    }
                }
                if (piecesGO == null)
                    piecesGO = new GameObject(PIECES);

                var player = Instantiate(levelManager.PlayerPrefab);
                var playerTile = levelManager.GetTile(levelManager.LevelProperties.PiecesPattern.PlayerColRow);
                if (playerTile == null)
                {
                    playerTile = levelManager.Tiles[0];
                    Debug.LogWarning("Cannot generate piece on: " + levelManager.LevelProperties.PiecesPattern.PlayerColRow.row + ", Col: " + levelManager.LevelProperties.PiecesPattern.PlayerColRow.col);
                }
                player.transform.parent = piecesGO.transform;
                player.transform.localScale = playerTile.Tile.transform.localScale;
                player.transform.position = playerTile.Tile.transform.position;

                foreach (var piece in levelManager.LevelProperties.PiecesPattern.Pieces)
                {
                    if (levelManager.TryGetTile(piece.ColRow, out var pieceTile))
                    {
                        var pieceGO = Instantiate(piece.Prefab);
                        pieceGO.transform.parent = piecesGO.transform;
                        pieceGO.transform.localScale = pieceTile.Tile.transform.localScale;
                        pieceGO.transform.position = pieceTile.Tile.transform.position;
                    }
                }
            }
        }

        protected override void OnGUI()
        {
            if (levelManager == null) return;
            base.OnGUI();
            MapPieces();
        }

        [Button]
        public void MapPieces()
        {
            pieceMap = new();
            var allPieces = new List<KnifePiece>(FindObjectsOfType<KnifePiece>());
            var allTiles = levelManager.Tiles;
            foreach (var piece in allPieces)
            {
                foreach (var tile in allTiles)
                {
                    if (Vector2.Distance(piece.transform.position, tile.Tile.transform.position) < accuraccy)
                    {
                        pieceMap.Add(new(piece, tile, tile.ColRow));
                        break;
                    }
                }
            }
        }
    }
}
