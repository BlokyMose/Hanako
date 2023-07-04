using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;
using Unity.EditorCoroutines.Editor;
using Mono.Cecil.Cil;
using static Hanako.Knife.KnifePiecesPattern;

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
            int id;
            public KnifePiece piece;
            public TileCache tile;
            public ColRow colRow;

            public int Id { get => id; }

            public PieceColRow(int id, KnifePiece piece, TileCache tile, ColRow colRow)
            {
                this.id = id;
                this.piece = piece;
                this.tile = tile;
                this.colRow = colRow;
            }
        }

        #region [Vars: Level]

        [OnValueChanged(nameof(OnLevelValueChanged))]
        public KnifeLevel level;

        [OnValueChanged(nameof(OnPiecesPatternValueChanged)), LabelText("Pattern")]
        public KnifePiecesPattern piecesPattern;

        #endregion

        #region [Vars: Generation]

        [FoldoutGroup("Generation")]
        [LabelText("Tiles"), ToggleLeft, GUIColor("@" + nameof(isGenerateTiles) + "?Color.green:UnityUtility.ColorUtility.lightSalmon")]
        public bool isGenerateTiles = true;
        
        [FoldoutGroup("Generation")]
        [LabelText("Pieces"), ToggleLeft, GUIColor("@" + nameof(isGeneratePieces) + "?Color.green:UnityUtility.ColorUtility.lightSalmon")]
        public bool isGeneratePieces = true;

        [FoldoutGroup("Generation")]
        [LabelText("ColRow Texts"), ToggleLeft, GUIColor("@" + nameof(isGenerateColRow) + "?Color.green:UnityUtility.ColorUtility.lightSalmon")]
        public bool isGenerateColRow = true;

        #endregion

        #region [Vars: Editing]

        [FoldoutGroup("Editing")]
        public float accuracy = 1f;

        [FoldoutGroup("Editing"), GUIColor("@"+nameof(liveUpdate)+ "?Color.green:UnityUtility.ColorUtility.lightSalmon")]
        public bool liveUpdate = true;

        public List<PieceColRow> pieceMap = new();

        #endregion

        #region [Vars: Data Handlers]

        List<PieceColRow> previousPieceMap = new();
        KnifeLevelManager levelManager;
        const string PIECES = "__PIECES__";
        EditorCoroutine corMappingPieces, corCheckLevelManager;
        bool isPiecesDestroyedWhenPlay = false;

        #endregion

        protected override void Initialize()
        {
            base.Initialize();
            level = null;
            piecesPattern = null;
            pieceMap.Clear();
            levelManager = FindObjectOfType<KnifeLevelManager>();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (levelManager == null)
                levelManager = FindObjectOfType<KnifeLevelManager>();


            if (Application.isPlaying && !isPiecesDestroyedWhenPlay)
            {
                DestroyPieces();
                isPiecesDestroyedWhenPlay = true;
            }
            else if (Application.isEditor)
            {
                isPiecesDestroyedWhenPlay = false;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DestroyPieces();
        }

        #region [Methods]

        [HorizontalGroup("But", order: -1, width: 0.5f)]
        [Button("Generate")]
        public void GenerateMap()
        {
            if (levelManager == null) return;
            DestroyPieces();

            level = levelManager.LevelProperties;
            piecesPattern = levelManager.LevelProperties.PiecesPattern;

            if (isGenerateTiles)
            {
                levelManager.GenerateLevelMap();
                foreach (var tile in levelManager.Tiles)
                    tile.Tile.CallAwake();
            }

            if (isGeneratePieces)
            {
                GeneratePieces();
            }

            if (isGenerateColRow)
            {
                levelManager.GenerateColRowTexts();
            }


            void GeneratePieces()
            {
                var piecesGO = new GameObject(PIECES);

                var player = (GameObject)PrefabUtility.InstantiatePrefab(levelManager.PlayerPrefab);
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
                        var pieceGO = (GameObject)PrefabUtility.InstantiatePrefab(piece.Prefab);
                        pieceGO.transform.parent = piecesGO.transform;
                        pieceGO.transform.localScale = pieceTile.Tile.transform.localScale;
                        pieceGO.transform.position = pieceTile.Tile.transform.position;
                    }
                }

                if (corMappingPieces != null) EditorCoroutineUtility.StopCoroutine(corMappingPieces);
                corMappingPieces = EditorCoroutineUtility.StartCoroutineOwnerless(MappingPieces());
            }

        }

        [HorizontalGroup("But"), Button]
        public void Save()
        {
            GeneratePiecePattern(out var playerColRow, out var pieces);
            levelManager.LevelProperties.PiecesPattern.SetPiecesPattern(playerColRow, pieces);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GenerateMap();
        }

        [HorizontalGroup("But"), Button]
        public void SaveAs()
        {
            GeneratePiecePattern(out var playerColRow, out var pieces);
            var path = EditorUtility.SaveFilePanelInProject(title: "Save As", defaultName: "KnifePiece_", extension: "asset", message: "");
            if (!string.IsNullOrEmpty(path))
            {
                var existingFile = AssetDatabase.LoadMainAssetAtPath(path);
                if (existingFile != null && existingFile is KnifePiecesPattern)
                {
                    var existingPattern = (KnifePiecesPattern)existingFile;
                    existingPattern.SetPiecesPattern(playerColRow, pieces);
                    Debug.Log("Overwrite: " + path);
                    piecesPattern = existingPattern;
                }
                else
                {
                    var newPiecesPattern = CreateInstance<KnifePiecesPattern>();
                    newPiecesPattern.SetPiecesPattern(playerColRow, pieces);
                    AssetDatabase.CreateAsset(newPiecesPattern, path);
                    Debug.Log("Saved as: " + path);
                    piecesPattern = newPiecesPattern;
                }

                OnPiecesPatternValueChanged();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }

        void GeneratePiecePattern(out ColRow playerColRow, out List<PieceProperties> pieces)
        {
            playerColRow = new ColRow();
            pieces = new List<PieceProperties>();
            foreach (var piece in pieceMap)
            {
                if (piece.piece is KnifePiece_Player)
                {
                    playerColRow = piece.colRow;
                }
                else
                {
                    if (TryFindPrefab(piece.piece.gameObject, out var prefab))
                    {
                        pieces.Add(new PieceProperties(prefab, piece.colRow));
                    }
                }
            }

            static bool TryFindPrefab(GameObject targetGO, out GameObject prefab)
            {
                // Get the root object if the given object is a child
                if (PrefabUtility.IsPartOfPrefabInstance(targetGO))
                {
                    GameObject rootObject = PrefabUtility.GetOutermostPrefabInstanceRoot(targetGO);
                    targetGO = PrefabUtility.GetCorrespondingObjectFromSource(rootObject);
                }

                // Check if the scene object is a prefab
                if (PrefabUtility.IsPrefabAssetMissing(targetGO))
                {
                    Debug.Log(targetGO.name + "is missing or not a prefab");
                    prefab = null;
                }

                // Find the prefab in the project
                var prefabPath = AssetDatabase.GetAssetPath(targetGO);
                prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                if (prefab == null)
                {
                    Debug.Log(targetGO.name + "cannot be loaded from the project");
                }

                return prefab != null;
            }

        }

        IEnumerator MappingPieces()
        {
            while (true)
            {
                yield return new EditorWaitForSeconds(0.1f);
                if (liveUpdate)
                {
                    MapPieces();
                }
            }
        }

        void MapPieces()
        {
            pieceMap = new();
            var allPieces = new List<KnifePiece>(FindObjectsOfType<KnifePiece>());
            var allTiles = levelManager.Tiles;
            foreach (var piece in allPieces)
            {
                foreach (var tile in allTiles)
                {
                    if (Vector2.Distance(piece.transform.position, tile.Tile.transform.position) < accuracy)
                    {
                        pieceMap.Add(new(piece.gameObject.GetInstanceID(), piece, tile, tile.ColRow));
                        break;
                    }
                }
            }
            HighlightDifferentMapTiles();
            UpdatePreviousPieceMap();

            void HighlightDifferentMapTiles()
            {
                var toHighlightPieces = new List<PieceColRow>();
                foreach (var piece in pieceMap)
                    toHighlightPieces.Add(piece);

                foreach (var piece in pieceMap)
                {
                    foreach (var previousPiece in previousPieceMap)
                    {
                        if (piece.Id == previousPiece.Id &&
                            piece.colRow.IsEqual(previousPiece.colRow))
                        {
                            toHighlightPieces.Remove(piece);
                            break;
                        }

                    }
                }

                EditorCoroutineUtility.StartCoroutineOwnerless(Delay(0.75f));
                IEnumerator Delay(float delay)
                {
                    foreach (var toHighlight in toHighlightPieces)
                        toHighlight.tile.Tile.Hovered(levelManager.Colors.TileValidMoveColor);

                    yield return new EditorWaitForSeconds(delay);

                    foreach (var toHighlight in toHighlightPieces)
                        toHighlight.tile.Tile.Unhovered();
                }
            }

            void UpdatePreviousPieceMap()
            {
                previousPieceMap = new();
                foreach (var piece in pieceMap)
                    previousPieceMap.Add(piece);
            }

        }

        void DestroyPieces()
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
            if (corMappingPieces!=null) EditorCoroutineUtility.StopCoroutine(corMappingPieces);
            DestroyImmediate(piecesGO);
        }

        #endregion

        #region [Methods: Inspector]

        void OnLevelValueChanged()
        {
            if (levelManager != null && level != null)
            {
                levelManager.SetLevelProperties(level);
                piecesPattern = level.PiecesPattern;
                GenerateMap();
            }
        }

        void OnPiecesPatternValueChanged()
        {
            if (levelManager != null && level != null && piecesPattern != null)
            {
                levelManager.LevelProperties.SetPiecesPattern(piecesPattern);
                GenerateMap();
            }
        }

        

        #endregion

    }
}
