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
        #region [Class]
        
        public class TileCache
        {
            ColRow colRow;
            GameObject go;
            SpriteRenderer sr;
            KnifeTile tile;

            public TileCache(ColRow colRow, GameObject go, SpriteRenderer sr, KnifeTile tile)
            {
                this.colRow = colRow;
                this.go = go;
                this.sr = sr;
                this.tile = tile;
            }

            public ColRow ColRow { get => colRow; }
            public GameObject GO { get => go; }
            public SpriteRenderer SR { get => sr;  }
            public KnifeTile Tile { get => tile; }
        }

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

        public class PieceCache
        {
            protected int controllerID;
            protected GameObject go;
            protected ColRow colRow;
            protected KnifePiece piece;

            public PieceCache(int controllerID, GameObject go, ColRow colRow, KnifePiece piece)
            {
                this.controllerID = controllerID;
                this.go = go;
                this.colRow = colRow;
                this.piece = piece;
            }

            public GameObject GO { get => go; }
            public ColRow ColRow { get => colRow; }
            public KnifePiece Piece { get => piece; }
            public int ControllerID { get => controllerID; }

            public void SetColRow(ColRow colRow) => this.colRow = colRow;
        }

        public class LivingPieceCache : PieceCache
        {
            public LivingPieceCache(int controllerID, GameObject go, ColRow colRow, KnifePiece piece) : base(controllerID, go, colRow, piece)
            {
            }

            public KnifePiece_Living LivingPiece => piece as KnifePiece_Living;
        }

        #endregion

        [SerializeField]
        KnifeLevel levelProperties;

        [SerializeField]
        KnifeColors colors;

        [Header("Components")]
        [SerializeField]
        Transform levelPos;

        [SerializeField]
        KnifeCursor playerCursor;

        [SerializeField]
        GameObject playerPrefab;

        [Header("Debug")]
        [SerializeField]
        GameObject TileColRowCanvas;

        List<WallCache> leftWalls = new();
        List<WallCache> rightWalls = new();
        List<TileCache> tiles = new();
        List<PieceCache> pieces = new();
        List<LivingPieceCache> livingPieces = new();
        GameObject player;
        int currentMovingPieceIndex;
        int playerControllerID = 0;

        KnifePiece_Living currentMovingPiece => livingPieces[currentMovingPieceIndex].Piece as KnifePiece_Living;
        LivingPieceCache currentMovingPieceCache => livingPieces[currentMovingPieceIndex];

        public KnifeLevel LevelProperties { get => levelProperties; }
        public KnifeColors Colors { get => colors;  }
        public List<TileCache> Tiles { get => tiles; }
        public List<PieceCache> Pieces { get => pieces; }
        public List<LivingPieceCache> LivingPieces { get => livingPieces; }

        private void Awake()
        {
            playerCursor.Init(this, playerControllerID);
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            GenerateLevelMap();
            GeneratePieces();
            StartGame();
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
                    
                    var tileComponent = tileGO.GetComponent<KnifeTile>();
                    if (tileComponent == null) Debug.LogWarning("Tile prefab has no KnifeTile component");
                    tileComponent.SortingGroup.sortingOrder = -row - col;

                    tiles.Add(new TileCache(new(col, row), tileGO, tileComponent.SR, tileComponent));
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
            sortingGroup.sortingOrder = mostTopTile.Tile.SortingGroup.sortingOrder - 1;
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

        public void GeneratePieces()
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning("Please generate map before generate pieces");
                return;
            }

            player = Instantiate(playerPrefab);
            var playerTile = GetTile(levelProperties.PiecesPattern.PlayerColRow);
            if (playerTile == null)
            {
                playerTile = tiles[0];
                Debug.LogWarning("Cannot generate piece on: "+ levelProperties.PiecesPattern.PlayerColRow.row+", Col: "+ levelProperties.PiecesPattern.PlayerColRow.col);
            }
            playerTile.Tile.SetAsParentOf(player);
            player.transform.localScale = Vector2.one;
            player.transform.localPosition = new(0, 0);
            var playerPieceComponent = player.GetComponent<KnifePiece>();
            pieces.Add(new(playerControllerID, player, playerTile.ColRow, playerPieceComponent));
            playerPieceComponent.Init(this);

            var playerPieceLivingComponent = player.GetComponent<KnifePiece_Living>();
            livingPieces.Add(new(playerControllerID, player, playerTile.ColRow, playerPieceLivingComponent));

            int pieceControllerID = 1;
            foreach (var piece in levelProperties.PiecesPattern.Pieces)
            {
                if(TryGetTile(piece.ColRow, out var pieceTile))
                {
                    var pieceGO = Instantiate(piece.Prefab);
                    pieceTile.Tile.SetAsParentOf(pieceGO);
                    pieceGO.transform.localScale = Vector2.one;
                    pieceGO.transform.localPosition = new(0, 0);
                    var pieceComponent = pieceGO.GetComponent<KnifePiece>();
                    pieces.Add(new(pieceControllerID, pieceGO, piece.ColRow, pieceComponent));
                    pieceComponent.Init(this);

                    if(pieceGO.TryGetComponent<KnifePiece_Living>(out var pieceLivingComponent))
                        livingPieces.Add(new(pieceControllerID, pieceGO, piece.ColRow, pieceLivingComponent));

                    pieceControllerID++;
                }
                else
                {
                    Debug.LogWarning("Cannot generate piece on: " + piece.ColRow.row + ", Col: " + piece.ColRow.col);
                }
            }
        }

        public void StartGame()
        {
            int round = -1;
            currentMovingPieceIndex = -1;
            GoToNextMovingPiece();

            void GoToNextMovingPiece()
            {
                UpdateCache();
                IncrementMovingPieceIndex();
                round++;
                if (round < levelProperties.RoundCount - 1)
                {
                    if (currentMovingPieceCache.ControllerID == 0)
                    {
                        playerCursor.PleaseClick(OnClickDone);
                        void OnClickDone(KnifeTile tile)
                        {
                            currentMovingPiece.MoveToTile(tile);
                        }
                    }
                    currentMovingPiece.PleaseMove(GoToNextMovingPiece);
                }
                else
                {

                }

                
            }
        }

        void UpdateCache()
        {
            foreach (var piece in pieces)
            {
                foreach (var tile in tiles)
                {
                    if (tile.Tile.PieceParent.TryGetComponentInFamily<KnifePiece>(out var foundPiece) &&
                        foundPiece == piece.Piece)
                    {
                        piece.SetColRow(tile.ColRow);
                        var livingPiece = GetLivingPiece(piece.ControllerID);
                        if (livingPiece != null)
                            livingPiece.SetColRow(tile.ColRow);
                        break;
                    }
                }
            }
        }

        void IncrementMovingPieceIndex()
        {
            currentMovingPieceIndex = (currentMovingPieceIndex + 1) % livingPieces.Count;
        }

        bool TryGetTile(ColRow colRow, out TileCache foundTile)
        {
            foundTile = GetTile(colRow);
            return foundTile != null;
        }

        TileCache GetTile(ColRow colRow)
        {
            foreach (var tile in tiles)
            {
                if (tile.ColRow.IsEqual(colRow))
                {
                    return tile;
                }
            }
            return null;
        }

        public PieceCache GetPiece(int controllerID)
        {
            foreach (var piece in pieces)
                if (piece.ControllerID == controllerID) return piece;
            return null;
        }

        public PieceCache GetPiece(KnifePiece knifePiece)
        {
            foreach (var piece in pieces)
                if (piece.Piece == knifePiece) return piece;
            return null;
        }

        public LivingPieceCache GetLivingPiece(int controllerID)
        {
            foreach (var piece in livingPieces)
                if (piece.ControllerID == controllerID) return piece;
            return null;
        }
    }
}
