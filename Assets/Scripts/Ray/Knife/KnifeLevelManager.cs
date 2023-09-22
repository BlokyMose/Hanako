using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityUtility;
using static Cinemachine.DocumentationSortingAttribute;
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

            public bool HasValidTile() => validTilesByMoveRule.Count > 0;
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

            public Func<bool> IsWinningConditionFulfilled;
            public Action OnUpdateCache;
            public Action OnPlayerTurn;
            public Action OnNextRound;
            public Action OnNextTurn;

            public TurnManager(
                List<LivingPieceCache> pieces, 
                int roundCount,
                Action onPlayerTurn,
                Action onUpdateCache, 
                Func<bool> isWinningConditionFulfilled,
                Action onNextRound,
                Action onNextTurn
                )
            {
                IsWinningConditionFulfilled = isWinningConditionFulfilled;
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
                OnUpdateCache?.Invoke();

                if (!IsWinningConditionFulfilled())
                {
                    OnNextRound?.Invoke();
                    currentTurnIndex = -1;
                    GoToNextMovingPiece();
                }
                else
                {

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
                return currentRound.turns.Count == 1;
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
        LevelInfoInitMode levelInfoInit = LevelInfoInitMode.SceneLoadingData;

        [SerializeField, ShowIf("@"+nameof(levelInfoInit)+"=="+nameof(LevelInfoInitMode)+"."+nameof(LevelInfoInitMode.LevelInfo))]
        LevelInfo levelInfo;

        [SerializeField, ShowIf("@"+nameof(levelInfoInit)+"=="+nameof(LevelInfoInitMode)+"."+nameof(LevelInfoInitMode.LevelProperties))]
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

        [SerializeField]
        GameObject tileUnhoverColPrefab;

        [Header("Game")]
        [SerializeField]
        float moveDuration = 1f;

        [SerializeField]
        AnimationCurve moveAnimationCurve;

        [SerializeField]
        GameInfoCanvas gameInfoCanvas;

        [SerializeField]
        Vector2 panAreaMax = new(4, 2);

        [SerializeField]
        Vector2 panAreaMin = new(-4, -2);

        [SerializeField]
        string killParamName = "kill";

        [SerializeField]
        string turnParamName = "turn";

        [SerializeField]
        string playTimeParamName = "playTime";

        [Header("UI")]
        [SerializeField]
        ScoreCanvas scoreCanvas;

        [SerializeField]
        LostCanvas lostCanvas;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom sfxAudioSource;

        [SerializeField]
        string sfxRoundTransition = "sfxRoundTransition";

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
        int killCount = 0;
        float playTime;
        Coroutine corPlayTime;

        public KnifeLevel LevelProperties { get => levelProperties; }
        public void SetLevelProperties(KnifeLevel newLevel) => levelProperties = newLevel;
        public KnifeColors Colors { get => colors;  }
        public List<TileCache> Tiles { get => tiles; }
        public List<PieceCache> Pieces { get => pieces; }
        public List<LivingPieceCache> LivingPieces { get => livingPieces; }
        public float MoveDuration { get => moveDuration; }
        public Transform LevelPos { get => levelPos;  }
        public GameObject PlayerPrefab { get => playerPrefab;  }
        public Vector2 PanAreaMax { get => panAreaMax;  }
        public Vector2 PanAreaMin { get => panAreaMin;  }
        public int KillCount { get => killCount;  }
        public int RoundCount { get => turnManager != null ? turnManager.CurrentRoundIndex : 0; }
        public float PlayTime { get => playTime;  }
        public KnifePiece_Player PlayerPiece { get => playerPiece; }

        #endregion

        public event Action<float> OnPlayTime;
        public event Action OnStartGame;
        public event Action<KnifePiece_Living> OnLivingPieceDied;
        public event Action<int> OnNextRound;
        public event Action<bool> OnGameOver;

        #region [Methods: Game]

        private void Awake()
        {
            scoreCanvas.gameObject.SetActive(false);
            lostCanvas.gameObject.SetActive(false);
        }

        private void Start()
        {
            AdjustLevelInfo();
            Init();
        }

        void AdjustLevelInfo()
        {
            if (levelInfoInit == LevelInfoInitMode.SceneLoadingData)
            {
                var sceneLoading = FindObjectOfType<SceneLoadingManager>();
                if (sceneLoading != null)
                {
                    if (sceneLoading.SceneLoadingData.LevelInfoToLoad.GameType == GameType.Knife)
                    {
                        levelInfo = sceneLoading.SceneLoadingData.LevelInfoToLoad;
                        levelProperties = levelInfo.KnifeLevel;
                    }
                    else Debug.LogWarning("SceneLoadingData.LevelInfoToLoad doesn't match this gameType");
                }
                else Debug.LogWarning("Cannot find SceneLoadingManager");
            }

            if (levelInfo != null)
            {
                levelProperties = levelInfo.KnifeLevel;
                var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
                if (allGamesInfoManager != null)
                {
                    allGamesInfoManager.AllGamesInfo.SetCurrentLevel(levelInfo);
                }
            }
            else Debug.LogWarning("LevelInfo is not set; Error might occur");

            if (levelProperties == null) Debug.LogWarning("LevelProperties is not set; Error might occur");
        }

        public void Init()
        {
            GenerateLevelMap();
            GeneratePieces();
            playerCursor.Init(this, playerControllerID, GenerateTileUnhoverCols());

            StartGame();
        }

        public void StartGame()
        {
            turnManager = new(
                livingPieces, 
                levelProperties.RoundCount,
                OnPlayerTurn,
                UpdateCache,
                IsWinningConditionFulfilled,
                OnNextRound,
                OnNextTurn
                );
            OnStartGame?.Invoke();
            turnManager.GoToNextRound();
            StartPlayTimer();
            
            void OnPlayerTurn()
            {
                sfxAudioSource.PlayOneClipFromPack(sfxRoundTransition);
                playerCursor.PleaseClick(OnClickDone);
                void OnClickDone(KnifeTile tile)
                {
                    TryMovePieceToTile(playerPiece, tile);
                }
            }

            void OnNextRound()
            {
                UpdateTurnOrderTexts();
                this.OnNextRound?.Invoke(turnManager.CurrentRoundIndex);
            }

            void OnNextTurn()
            {

            }
        }

        public bool IsWinningConditionFulfilled()
        {
            if (turnManager.CurrentRoundIndex >= levelProperties.RoundCount)
            {
                WonGame();
                return true;
            }

            if (turnManager.IsPieceCountOne())
            {
                WonGame();
                return true;
            }

            for (int i = livingPieces.Count - 1; i >= 0; i--)
            {
                var piece = livingPieces[i];
                if (!piece.HasValidTile())
                {
                    if (piece.LivingPiece == playerPiece)
                    {
                        LostGame();
                        return true; // Lost
                    }
                    else
                    {
                        piece.LivingPiece.Die(piece);
                    }
                }
            }

            return false;
        }

        public void WonGame()
        {
            StopPlayTimer();
            gameInfoCanvas.CheckAllRound();
            OnGameOver?.Invoke(true);
            scoreCanvas.gameObject.SetActive(true);
            scoreCanvas.Init(levelInfo, new()
            {
                new(killParamName, KillCount),
                new(turnParamName, RoundCount),
                new(playTimeParamName, (int)playTime)
            });
        }

        public void LostGame()
        {
            StopPlayTimer();
            gameInfoCanvas.CheckAllRound();
            OnGameOver?.Invoke(false);
        }

        public void AddKillCount()
        {
            killCount++;
        }

        public void UpdateCache()
        {
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

        public void StartPlayTimer()
        {
            corPlayTime = this.RestartCoroutine(Update(), corPlayTime);
            IEnumerator Update()
            {
                playTime = 0f;
                while (true)
                {
                    playTime += Time.deltaTime;
                    OnPlayTime?.Invoke(playTime);
                    yield return null;
                }
            }
        }

        public void StopPlayTimer()
        {
            if (corPlayTime!=null)
                StopCoroutine(corPlayTime);
        }

        #endregion

        #region [Methods: Generators]

        public void GenerateLevelMap()
        {
            levelPos.DestroyImmediateChildren();
            tiles = new();

            for (int row = 0; row < levelProperties.LevelSize.row; row++)
            {
                for (int col = 0; col < levelProperties.LevelSize.col; col++)
                {
                    var tileGO = InstantiateTile(
                        levelProperties.TilesPattern.GetTile(new(col, row), levelProperties),
                        levelPos,
                        levelProperties.TileSize,
                        levelProperties.OriginOffset, 
                        row, 
                        col);

                    tileGO.name = $"{col}, r{row}";
                    tileGO.transform.parent = levelPos;

                    var layerNumber = tileLayer.GetMemberLayerNumbers()[0];
                    tileGO.layer = layerNumber;
                    for (int i = 0; i < tileGO.transform.childCount; i++)
                        tileGO.transform.GetChild(i).gameObject.layer = layerNumber;

                    var tileComponent = tileGO.GetComponent<KnifeTile>();
                    tileComponent.SortingGroup.sortingOrder = -row - col;
                    tiles.Add(new TileCache(new(col, row), tileGO, tileComponent.SR, tileComponent));
                }
            }

            GenerateLevelWalls();
            GenerateLevelBottomWalls();
        }

        /// <summary>
        /// Make tiles that surround the level map, so the cursor can be prompted to unhover when exiting the map
        /// </summary>
        public List<Collider2D> GenerateTileUnhoverCols()
        {
            var parent = new GameObject("__UNHOVER__");
            parent.transform.parent = levelPos;
            parent.transform.localPosition = Vector3.zero;

            var tileUnhoverCols = new List<Collider2D>();
            for (int row = -1; row < levelProperties.LevelSize.row+1; row++)
            {
                for (int col = -1; col < levelProperties.LevelSize.col+1; col++)
                {
                    if (!(col == -1 || col == levelProperties.LevelSize.col ||
                          row == -1 || row == levelProperties.LevelSize.row ))
                        continue;

                    var tileGO = InstantiateTile(
                        tileUnhoverColPrefab,
                        parent.transform,
                        levelProperties.TileSize,
                        levelProperties.OriginOffset,
                        row,
                        col);

                    tileGO.name = $"UN: {col}, r{row}";
                    tileUnhoverCols.Add(tileGO.GetComponent<Collider2D>());
                }
            }
            return tileUnhoverCols;
        }

        private GameObject InstantiateTile(GameObject tilePrefab, Transform parent, Vector2 tileSize, Vector2 offset, int row, int col)
        {
            var tileGO = Instantiate(tilePrefab, parent);
            tileGO.transform.localPosition = new((col * tileSize.x / 2) - (row * tileSize.x / 2), (row * tileSize.y / 2) + (col * tileSize.y / 2));
            tileGO.transform.localPosition += (Vector3)offset;
            return tileGO;
        }

        public void GenerateLevelWalls()
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning("Generate level map first before generate walls");
                return;
            }

            float wallYOffset =  levelProperties.TileHeightHalf;
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
        
        public void GenerateLevelBottomWalls()
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning("Generate level map first before generate walls");
                return;
            }

            for (int i = 0; i < levelProperties.BottomWallStoriesCount; i++)
            {
                var wallYOffset = levelProperties.WallSize.y + levelProperties.TileSize.y + (levelProperties.WallSize.y * i);
                var mostBottomTile = tiles[0];
                var mostTopTile = tiles.GetLast();
                var wallsParent = new GameObject("__BOTTOM_WALLS__"+i);
                var sortingGroup = wallsParent.AddComponent<SortingGroup>();
                sortingGroup.sortingOrder = mostTopTile.Tile.SortingGroup.sortingOrder - (2+i); // BottomWalls should be behind Walls
                wallsParent.transform.parent = levelPos;
                wallsParent.transform.position = mostBottomTile.GO.transform.position;
                GenerateWalls(wallYOffset, wallsParent.transform);

                wallYOffset = levelProperties.WallSize.y + levelProperties.TileSize.y - levelProperties.TileHeightHalf + (levelProperties.WallSize.y * (i-2));
                GenerateRightEdgeWall(levelProperties.LevelSize.row - 1, wallYOffset, wallsParent.transform);
                GenerateLeftEdgeWall(levelProperties.LevelSize.col - 1, wallYOffset, wallsParent.transform);
            }

            void GenerateWalls(float wallYOffset, Transform wallsParent)
            {
                for (int col = 0; col < levelProperties.LevelSize.col; col++)
                {
                    var wallGO = Instantiate(levelProperties.BottomWallsPattern.GetLeftWall(col, levelProperties));
                    wallGO.transform.localScale = new(-wallGO.transform.localScale.x, wallGO.transform.localScale.y);
                    wallGO.name = "Left_" + col;
                    wallGO.transform.parent = wallsParent.transform;
                    wallGO.transform.localPosition = new(-(levelProperties.TileSize.x / 2 * col), levelProperties.TileSize.y / 2 * col - wallYOffset);
                    var sr = wallGO.GetComponentInFamily<SpriteRenderer>();
                    sr.color = levelProperties.BottomWallsPattern.LeftWallColor;
                    sr.sortingOrder = -col;
                    leftWalls.Add(new(col, wallGO, sr));
                }

                for (int row = 0; row < levelProperties.LevelSize.row; row++)
                {
                    var wallGO = Instantiate(levelProperties.BottomWallsPattern.GetRightWall(row, levelProperties));
                    wallGO.name = "Right_" + row;
                    wallGO.transform.parent = wallsParent.transform;
                    wallGO.transform.localPosition = new((levelProperties.TileSize.x / 2 * row), levelProperties.TileSize.y / 2 * row - wallYOffset);
                    var sr = wallGO.GetComponentInFamily<SpriteRenderer>();
                    sr.color = levelProperties.BottomWallsPattern.RightWallColor;
                    sr.sortingOrder = -row;
                    rightWalls.Add(new(row, wallGO, sr));
                }


            }

            void GenerateRightEdgeWall(int row, float wallYOffset, Transform wallsParent)
            {
                var wallGO = Instantiate(levelProperties.WallsPattern.GetRightWall(row, levelProperties));
                wallGO.transform.localScale = new(-wallGO.transform.localScale.x, wallGO.transform.localScale.y);
                wallGO.name = "Right_" + row;
                wallGO.transform.parent = wallsParent.transform;
                wallGO.transform.localPosition = new((levelProperties.TileSize.x / 2 * (row + 1)), -levelProperties.TileSize.y / 2 * row - wallYOffset);
                var sr = wallGO.GetComponentInFamily<SpriteRenderer>();
                sr.color = levelProperties.WallsPattern.RightWallColor;
                sr.sortingOrder = -(row+1);
                rightWalls.Add(new(row, wallGO, sr));
            }

            void GenerateLeftEdgeWall(int col, float wallYOffset, Transform wallsParent)
            {
                var wallGO = Instantiate(levelProperties.WallsPattern.GetLeftWall(col, levelProperties));
                wallGO.name = "Left_" + col;
                wallGO.transform.parent = wallsParent.transform;
                wallGO.transform.localPosition = new(-(levelProperties.TileSize.x / 2 * (col + 1)), -levelProperties.TileSize.y / 2 * col - wallYOffset);
                var sr = wallGO.GetComponentInFamily<SpriteRenderer>();
                sr.color = levelProperties.WallsPattern.LeftWallColor;
                sr.sortingOrder = -(col+1);
                leftWalls.Add(new(col, wallGO, sr));
            }
        }

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
                foundLivingCache.TurnOrderText.Hide();
                diedPieces.Add(foundLivingCache);
                targetPiece.transform.parent = null;
                AddKillCount();
                OnLivingPieceDied?.Invoke(targetPiece);
            }
        }

        public void MoveLivingPieceToEscapeList(KnifePiece_Living targetPiece)
        {
            var foundLivingCache = GetLivingPiece(targetPiece);
            RemovePiece(targetPiece);
            if (foundLivingCache != null)
            {
                foundLivingCache.TurnOrderText.Hide();
                escapedPieces.Add(foundLivingCache);
                targetPiece.transform.parent = null;
            }
        }



        #endregion

    }
}
