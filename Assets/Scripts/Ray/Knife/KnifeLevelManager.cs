using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditorInternal;
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
            protected KnifeLevelManager levelManager;

            public PieceCache(int controllerID, GameObject go, ColRow colRow, KnifePiece piece, KnifeLevelManager levelManager)
            {
                this.controllerID = controllerID;
                this.go = go;
                this.colRow = colRow;
                this.piece = piece;
                this.levelManager = levelManager;
            }

            public GameObject GO { get => go; }
            public ColRow ColRow { get => colRow; }
            public KnifePiece Piece { get => piece; }
            public int ControllerID { get => controllerID; }

            public void SetColRow(ColRow colRow) => this.colRow = colRow;

        }

        public class LivingPieceCache : PieceCache
        {
            public class TileCheckResult
            {
                bool isValid;
                bool isInteractable;

                public TileCheckResult(bool isValid, bool isInteractable)
                {
                    this.isValid = isValid;
                    this.isInteractable = isInteractable;
                }

                public bool IsValid { get => isValid; }
                public bool IsInteractable { get => isInteractable;  }
            }

            private List<TileCache> validTilesByMoveRule = new();
            public KnifePiece_Living LivingPiece => piece as KnifePiece_Living;

            public List<TileCache> ValidTilesByMoveRule { get => validTilesByMoveRule; }

            public LivingPieceCache(int controllerID, GameObject go, ColRow colRow, KnifePiece piece, KnifeLevelManager levelManager) : base(controllerID, go, colRow, piece, levelManager)
            {
            }


            public void UpdateCache(ColRow colRow)
            {
                this.colRow = colRow;
                validTilesByMoveRule = LivingPiece.MoveRule.GetValidTiles(this, levelManager.Pieces, levelManager.LevelProperties, levelManager.Tiles);
            }

            public TileCheckResult CheckTile(KnifeTile tile)
            {
                foreach (var validTile in validTilesByMoveRule)
                {
                    if (validTile.Tile == tile)
                    {
                        if (tile.TryGetPiece(out var tilePiece))
                        {
                            var isValid = tilePiece.CheckValidityAgainst(levelManager.GetPiece(tilePiece), levelManager.GetTile(tile), this, levelManager.GetTile(colRow));
                            var isInteratable = tilePiece.CheckInteractabilityAgainst(levelManager.GetPiece(tilePiece), levelManager.GetTile(tile), this, levelManager.GetTile(colRow));
                            return new TileCheckResult(isValid, isInteratable);
                        }
                        else
                        {
                            return new TileCheckResult(true, false);
                        }
                    }
                }
                return new TileCheckResult(false, false);
            }
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

        [SerializeField]
        LayerMask tileLayer;

        [Header("Game")]
        [SerializeField]
        float moveDuration = 1f;

        [SerializeField]
        AnimationCurve moveAnimationCurve;

        [Header("Debug")]
        [SerializeField]
        GameObject TileColRowCanvas;

        List<WallCache> leftWalls = new();
        List<WallCache> rightWalls = new();
        List<TileCache> tiles = new();
        List<PieceCache> pieces = new();
        List<LivingPieceCache> livingPieces = new();
        List<LivingPieceCache> diedPieces = new();
        List<LivingPieceCache> escapedPieces = new();
        GameObject player;
        int currentMovingPieceIndex;
        int currentRound;
        int playerControllerID = 0;

        KnifePiece_Living currentMovingPiece => livingPieces[currentMovingPieceIndex].Piece as KnifePiece_Living;
        LivingPieceCache currentMovingPieceCache => livingPieces[currentMovingPieceIndex];

        public KnifeLevel LevelProperties { get => levelProperties; }
        public KnifeColors Colors { get => colors;  }
        public List<TileCache> Tiles { get => tiles; }
        public List<PieceCache> Pieces { get => pieces; }
        public List<LivingPieceCache> LivingPieces { get => livingPieces; }
        public float MoveDuration { get => moveDuration; }

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
                    tileGO.name = $"{col}, r{row}";
                    var layerNumber = tileLayer.GetMemberLayerNumbers()[0];
                    tileGO.layer = layerNumber;
                    for (int i = 0; i < tileGO.transform.childCount; i++)
                        tileGO.transform.GetChild(i).gameObject.layer = layerNumber;
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
                textComponent.text = $"{cell.ColRow.col}, r{cell.ColRow.row}";
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
            pieces.Add(new(playerControllerID, player, playerTile.ColRow, playerPieceComponent, this));
            playerPieceComponent.Init(this);

            var playerPieceLivingComponent = player.GetComponent<KnifePiece_Living>();
            playerPieceLivingComponent.Init(moveDuration, moveAnimationCurve);
            livingPieces.Add(new(playerControllerID, player, playerTile.ColRow, playerPieceLivingComponent, this));

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
                    pieces.Add(new(pieceControllerID, pieceGO, piece.ColRow, pieceComponent, this));
                    pieceComponent.Init(this);

                    if(pieceGO.TryGetComponent<KnifePiece_Living>(out var pieceLivingComponent))
                    {
                        pieceLivingComponent.Init(moveDuration, moveAnimationCurve);
                        livingPieces.Add(new(pieceControllerID, pieceGO, piece.ColRow, pieceLivingComponent, this));
                    }

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
            currentRound = -1;
            GoToNextRound();

            void GoToNextRound()
            {
                currentRound++;
                if (currentRound < levelProperties.RoundCount)
                {
                    currentMovingPieceIndex = -1;
                    GoToNextMovingPiece();
                }
                else
                {
                    Lost();
                }
            }

            void GoToNextMovingPiece()
            {

                currentMovingPieceIndex++;
                if (currentMovingPieceIndex < livingPieces.Count)
                {
                    UpdateCache();
                    // Player's turn
                    if (currentMovingPieceCache.ControllerID == 0)
                    {
                        playerCursor.PleaseClick(OnClickDone);
                        void OnClickDone(KnifeTile tile)
                        {
                            TryMovePieceToTile(currentMovingPiece, tile);
                        }
                    }
                    currentMovingPiece.PlaseAct(GoToNextMovingPiece);
                }
                else
                {
                    GoToNextRound();
                }
            }
        }

        public bool TryMovePieceToTile(KnifePiece_Living piece, KnifeTile tile)
        {
            var foundPieceCache = GetLivingPiece(piece);
            if (foundPieceCache != null)
            {
                var tileCheckResult = foundPieceCache.CheckTile(tile);
                if (tileCheckResult.IsValid)
                {
                    if (tileCheckResult.IsInteractable)
                    {
                        if (tile.TryGetPiece(out var tilePiece) && TryGetPiece(tilePiece, out var occupantPiece))
                        {
                            occupantPiece.Piece.Interacted(occupantPiece, GetTile(occupantPiece.ColRow), foundPieceCache, GetTile(foundPieceCache.ColRow));
                            return true;
                        }
                    }
                    else
                    {
                        foundPieceCache.LivingPiece.MoveToTile(tile);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Lost()
        {
            Debug.Log("Game lost");
        }

        public void Won()
        {
            Debug.Log("Game won");
        }

        void UpdateCache()
        {
            if (livingPieces.Count == 1)
            {
                if (diedPieces.Count > 0)
                {
                    Won();
                }
                else
                {
                    Lost();
                }
            }

            foreach (var piece in pieces)
            {
                foreach (var tile in tiles)
                {
                    if (tile.Tile.TryGetPiece(out var foundPiece) &&
                        foundPiece == piece.Piece)
                    {
                        piece.SetColRow(tile.ColRow);
                        var livingPiece = GetLivingPiece(piece.ControllerID);
                        if (livingPiece != null)
                        {
                            livingPiece.UpdateCache(tile.ColRow);
                        }
                        break;
                    }
                }
            }

            playerCursor.Refresh();
        }

        public bool TryGetTile(ColRow colRow, out TileCache foundTile)
        {
            foundTile = GetTile(colRow);
            return foundTile != null;
        }

        public TileCache GetTile(ColRow colRow)
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
        
        public TileCache GetTile(KnifeTile knifeTile)
        {
            foreach (var tile in tiles)
            {
                if (tile.Tile == knifeTile)
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

        public bool TryGetPiece(KnifePiece knifePiece, out PieceCache foundPiece)
        {
            foundPiece = GetPiece(knifePiece);
            return foundPiece != null;
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

        public LivingPieceCache GetLivingPiece(KnifePiece_Living targetPiece)
        {
            foreach (var piece in livingPieces)
                if (piece.LivingPiece == targetPiece) return piece;
            return null;
        }

        public LivingPieceCache GetLivingPiece(KnifePiece targetPiece)
        {
            foreach (var piece in livingPieces)
                if (piece.LivingPiece == targetPiece) return piece;
            return null;
        }

        /// <summary>
        /// Remove the piece from both pieces and livingPieces list
        /// </summary>
        public void RemovePiece(KnifePiece targetPiece)
        {
            PieceCache foundPiece = null;
            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                var pieceCache = pieces[i];
                if (pieceCache.Piece == targetPiece)
                {
                    foundPiece = pieceCache;
                    pieces.Remove(pieceCache);
                    for (int livingPieceIndex = livingPieces.Count - 1; livingPieceIndex >= 0; livingPieceIndex--)
                    {
                        if (livingPieces[livingPieceIndex] == pieceCache)
                        {
                            livingPieces.RemoveAt(livingPieceIndex);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove living piece from the livingPieces list, and add it to diedPieces list
        /// </summary>
        public void RemoveLivingPiece(KnifePiece_Living targetPiece)
        {
            if (TryRemovePieceFromLivingAndNonLivingList(targetPiece, out var foundPiece))
            {
                diedPieces.Add(foundPiece);
                targetPiece.transform.parent = null;
            }
        }

        public void ResurrectLivingPiece(KnifePiece_Living targetPiece)
        {
            LivingPieceCache foundPiece = null;
            for (int i = diedPieces.Count - 1; i >= 0; i--)
            {
                var piece = diedPieces[i];
                if (piece.LivingPiece == targetPiece)
                {
                    foundPiece = piece;
                    diedPieces.Remove(piece);
                    break;
                }
            }

            if (foundPiece == null) return;
            diedPieces.Remove(foundPiece);
            livingPieces.Add(foundPiece);
            pieces.Add(foundPiece);
        }

        public void MoveLivingPieceToEscapeList(KnifePiece_Living targetPiece)
        {
            if(TryRemovePieceFromLivingAndNonLivingList(targetPiece, out var foundPiece))
            {
                escapedPieces.Add(foundPiece);
                targetPiece.transform.parent = null;
            }
        }

        bool TryRemovePieceFromLivingAndNonLivingList(KnifePiece_Living targetPiece, out LivingPieceCache foundPiece)
        {
            foundPiece = null;
            for (int i = livingPieces.Count - 1; i >= 0; i--)
            {
                if (livingPieces[i].LivingPiece == targetPiece)
                {
                    foundPiece = livingPieces[i];
                    livingPieces.RemoveAt(i);
                    pieces.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
    }
}
