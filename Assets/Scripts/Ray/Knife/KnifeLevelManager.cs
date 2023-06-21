using Encore.Utility;
using Sirenix.OdinInspector;
using System;
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
            
            public KnifePiece_Living LivingPiece => piece as KnifePiece_Living;

            List<TileCache> validTilesByMoveRule = new();

            public List<TileCache> ValidTilesByMoveRule { get => validTilesByMoveRule; }

            KnifeTurnOrderText orderText;
            public KnifeTurnOrderText TurnOrderText => orderText;

            public LivingPieceCache(int controllerID, GameObject go, ColRow colRow, KnifePiece piece, KnifeLevelManager levelManager, KnifeTurnOrderText orderText) : base(controllerID, go, colRow, piece, levelManager)
            {
                this.orderText = orderText;
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

        public class TurnManager
        {
            public class Round
            {
                public List<LivingPieceCache> turns = new();

                public Round(List<LivingPieceCache> turns)
                {
                    this.turns = turns;
                }
            }

            List<Round> rounds = new();

            int currentRoundIndex = 0;
            public Round currentRound => rounds[currentRoundIndex];
            int currentTurnIndex = 0;
            public LivingPieceCache currentPiece => rounds[currentRoundIndex].turns[currentTurnIndex];

            public int CurrentRoundIndex { get => currentRoundIndex; }

            public Func<bool> OnCheckWinningCondition;
            public Action OnUpdateCache;
            public Action OnPlayerTurn;
            public Action OnNextRound;
            public Action OnNextTurn;

            public TurnManager(
                List<LivingPieceCache> pieces, 
                int roundCount,
                Action onPlayerTurn,
                Action onUpdateCache, 
                Func<bool> onCheckWinningCondition,
                Action onNextRound,
                Action onNextTurn
                )
            {
                OnCheckWinningCondition = onCheckWinningCondition;
                OnUpdateCache = onUpdateCache;
                OnPlayerTurn = onPlayerTurn;
                OnNextRound = onNextRound;
                OnNextTurn = onNextTurn;

                rounds = new();
                for (int i = 0; i < roundCount; i++)
                {
                    rounds.Add(new Round(new List<LivingPieceCache>()));
                    foreach (var piece in pieces)
                        rounds.GetLast().turns.Add(piece);
                }

                currentRoundIndex = -1;
            }

            public void GoToNextRound()
            {
                currentRoundIndex++;
                OnNextRound?.Invoke();
                if (currentRoundIndex < rounds.Count)
                {
                    currentTurnIndex = -1;
                    GoToNextMovingPiece();
                }
                else
                {
                    OnCheckWinningCondition?.Invoke();
                }
            }

            public void GoToNextMovingPiece()
            {
                currentTurnIndex++;
                if (currentTurnIndex < currentRound.turns.Count)
                {
                    OnNextTurn?.Invoke();
                    OnUpdateCache?.Invoke();

                    // Player's turn
                    if (currentPiece.ControllerID == 0)
                    {
                        OnPlayerTurn?.Invoke();
                    }
                    currentPiece.LivingPiece.PlaseAct(GoToNextMovingPiece);

                }
                else
                {
                    GoToNextRound();
                }
            }

            public bool IsPieceCountOne()
            {
                return rounds[0].turns.Count == 0;
            }

            public void RemovePiece(LivingPieceCache piece)
            {
                foreach (var round in rounds)
                {
                    for (int i = round.turns.Count - 1; i >= 0; i--)
                        if (round.turns[i] == piece)
                            round.turns.RemoveAt(i);
                }
            }

            public void RemoveTurnOf(LivingPieceCache targetPiece, int count = 1)
            {
                if (count <= 0) return;

                int toRemoveCount = count;
                for (int roundIndex = currentRoundIndex; roundIndex < rounds.Count; roundIndex++)
                {
                    if (toRemoveCount <= 0) break;
                    var round = rounds[roundIndex];
                    RemoveTurn();

                    void RemoveTurn()
                    {
                        bool isFound = false;
                        for (int turnIndex = 0; turnIndex < round.turns.Count; turnIndex++)
                        {
                            var turn = round.turns[turnIndex];
                            if (!(roundIndex == currentRoundIndex && turnIndex == currentTurnIndex) &&
                                turn == targetPiece)
                            {
                                round.turns.Remove(targetPiece);
                                toRemoveCount--;
                                isFound = true;
                                break;
                            }
                        }

                        if (toRemoveCount > 0 && isFound)
                        {
                            RemoveTurn();
                        }
                    }
                }

                foreach (var round in rounds)
                {

                }
            }
        }

        #endregion

        #region [Vars: Serialiazables]

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

        [SerializeField]
        KnifeTurnOrderText turnOrderTextPrefab;

        [Header("Game")]
        [SerializeField]
        float moveDuration = 1f;

        [SerializeField]
        AnimationCurve moveAnimationCurve;

        [SerializeField]
        TextMeshProUGUI roundCountText;

        [SerializeField]
        string roundCountTemplate = "Round: {} left";

        [SerializeField]
        TextMeshProUGUI soulCountText;

        [SerializeField]
        string soulCountTemplate = "x {}";

        [Header("Debug")]
        [SerializeField]
        GameObject TileColRowCanvas;



        #endregion

        #region [Vars: Data Handlers]

        List<WallCache> leftWalls = new();
        List<WallCache> rightWalls = new();
        List<TileCache> tiles = new();
        List<PieceCache> pieces = new();
        List<LivingPieceCache> livingPieces = new();
        List<LivingPieceCache> diedPieces = new();
        List<LivingPieceCache> escapedPieces = new();
        TurnManager turnManager = null;
        GameObject player;
        int playerControllerID = 0;
        KnifePiece_Player playerPiece;
        int soulCount = 0;

        public KnifeLevel LevelProperties { get => levelProperties; }
        public KnifeColors Colors { get => colors;  }
        public List<TileCache> Tiles { get => tiles; }
        public List<PieceCache> Pieces { get => pieces; }
        public List<LivingPieceCache> LivingPieces { get => livingPieces; }
        public float MoveDuration { get => moveDuration; }

        #endregion

        #region [Methods: Game]

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

        public void StartGame()
        {
            turnManager = new(
                livingPieces, 
                levelProperties.RoundCount,
                OnPlayerTurn,
                UpdateCache,
                CheckWinningCondition,
                OnNextRound,
                OnNextTurn);

            turnManager.GoToNextRound();
            UpdateSoulCountText();
            
            void OnPlayerTurn()
            {
                playerCursor.PleaseClick(OnClickDone);
                void OnClickDone(KnifeTile tile)
                {
                    TryMovePieceToTile(playerPiece, tile);
                }
            }

            void OnNextRound()
            {
                UpdateRoundCountText();
                UpdateTurnOrderTexts();
            }

            void OnNextTurn()
            {

            }
        }

        public bool CheckWinningCondition()
        {
            if (turnManager.CurrentRoundIndex >= levelProperties.RoundCount)
            {
                Lost();
                return true;
            }

            if (turnManager.IsPieceCountOne())
            {
                if (diedPieces.Count > 0)
                {
                    Won();
                }
                else
                {
                    Lost();
                }
                return true;
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

        public void AddSoul()
        {
            soulCount++;
            UpdateSoulCountText();
        }

        public void UpdateCache()
        {
            if (CheckWinningCondition()) return;

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


        void UpdateSoulCountText()
        {
            soulCountText.text = soulCountTemplate.Replace("{}", soulCount.ToString());
        }

        void UpdateRoundCountText()
        {
            roundCountText.text = roundCountTemplate.Replace("{}", (LevelProperties.RoundCount - turnManager.CurrentRoundIndex).ToString());
        }

        public void RemoveTurnOf(LivingPieceCache targetPiece, int count = 1)
        {
            turnManager.RemoveTurnOf(targetPiece, count);
        }

        void UpdateTurnOrderTexts()
        {
            foreach (var piece in livingPieces)
            {
                piece.TurnOrderText.ClearText();
            }

            var turnOrder = 1;
            foreach (var turn in turnManager.currentRound.turns)
            {
                if (turn.TurnOrderText.GetText() == "")
                    turn.TurnOrderText.SetText(turnOrder.ToString());
                else
                    turn.TurnOrderText.SetText(turn.TurnOrderText.GetText()+","+turnOrder.ToString());

                turnOrder++;
            }
        }

        public void ShowTurnOrderTexts()
        {
            foreach (var piece in livingPieces)
                piece.TurnOrderText.Show();
        }

        public void HideTurnOrderTexts()
        {
            foreach (var piece in livingPieces)
                piece.TurnOrderText.Hide();
        }

        #endregion

        #region [Methods: Generators]

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
            this.playerPiece = player.GetComponent<KnifePiece_Player>();
            pieces.Add(new(playerControllerID, player, playerTile.ColRow, playerPiece, this));
            playerPiece.Init(this);

            var playerPieceLivingComponent = player.GetComponent<KnifePiece_Living>();
            playerPieceLivingComponent.Init(moveDuration, moveAnimationCurve);
            var playerTurnOrderText = Instantiate(turnOrderTextPrefab, player.transform);
            playerTurnOrderText.Init(playerPieceLivingComponent.TurnOrderTextOffset);
            livingPieces.Add(new(playerControllerID, player, playerTile.ColRow, playerPieceLivingComponent, this, playerTurnOrderText));

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
                        var turnOrderText = Instantiate(turnOrderTextPrefab, pieceGO.transform);
                        turnOrderText.Init(pieceLivingComponent.TurnOrderTextOffset);
                        livingPieces.Add(new(pieceControllerID, pieceGO, piece.ColRow, pieceLivingComponent, this, turnOrderText));
                    }

                    pieceControllerID++;
                }
                else
                {
                    Debug.LogWarning("Cannot generate piece on: " + piece.ColRow.row + ", Col: " + piece.ColRow.col);
                }
            }
        }


        #endregion

        #region [Methods: Getters]

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


        #endregion

        #region [Methods: Managing Lists]

        /// <summary>
        /// Remove the piece from both pieces and livingPieces list
        /// </summary>
        public void RemovePiece(KnifePiece targetPiece)
        {
            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                var pieceCache = pieces[i];
                if (pieceCache.Piece == targetPiece)
                {
                    pieces.Remove(pieceCache);
                    if (targetPiece is KnifePiece_Living)
                        RemoveLivingPiece(targetPiece as KnifePiece_Living, out var foundLivingCache);
                    break;
                }
            }
        }


        bool RemoveLivingPiece(KnifePiece_Living targetPiece, out LivingPieceCache foundCache)
        {
            for (int livingPieceIndex = livingPieces.Count - 1; livingPieceIndex >= 0; livingPieceIndex--)
            {
                if (livingPieces[livingPieceIndex].Piece == targetPiece)
                {
                    foundCache = livingPieces[livingPieceIndex];
                    turnManager.RemovePiece(foundCache);
                    livingPieces.RemoveAt(livingPieceIndex);
                    return true;
                }
            }
            foundCache = null;
            return false;
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

        public void MoveLivingPieceToDeadList(KnifePiece_Living targetPiece)
        {
            var foundLivingCache = GetLivingPiece(targetPiece);
            RemovePiece(targetPiece);
            if (foundLivingCache != null)
            {
                diedPieces.Add(foundLivingCache);
                targetPiece.transform.parent = null;
                AddSoul();
            }
        }

        public void MoveLivingPieceToEscapeList(KnifePiece_Living targetPiece)
        {
            var foundLivingCache = GetLivingPiece(targetPiece);
            RemovePiece(targetPiece);
            if (foundLivingCache != null)
            {
                escapedPieces.Add(foundLivingCache);
                targetPiece.transform.parent = null;
            }
        }



        #endregion


    }
}
