using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hub
{
    public class HubLevelDoor : MonoBehaviour
    {
        [SerializeField]
        LevelInfo levelInfo;

        [Header("Components")]
        [SerializeField, LabelText("Detect: Canvas")]
        ColliderProxy detectAreaOfLevelCanvas;

        [SerializeField, LabelText("Detect: Preview")]
        ColliderProxy detectAreaOfLevelPreview;

        [SerializeField]
        Transform respawnPos;

        [Header("UI")]
        [SerializeField]
        Transform levelInfoPreviewParent;

        [SerializeField]
        HubLevelInfoPreview levelInfoPreviewPrefab;

        [SerializeField]
        HubMinimapIcon minimapIconPrefab;

        [SerializeField]
        HubColors colors;

        public event Action<LevelInfo> OnShowLevelCanvas;
        public event Action OnHideLevelCanvas;
        public event Action OnShowLevelInfoPreview;
        public event Action OnHideLevelInfoPreview;
        HubLevelInfoPreview currentLevelInfoPreview;

        public LevelInfo LevelInfo { get => levelInfo; }
        public Transform RespawnPos { get => respawnPos; }

        void Awake()
        {
            levelInfoPreviewParent.DestroyChildren();

            detectAreaOfLevelCanvas.OnEnter += (enteredCol) => { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) ShowLevelCanvas(); };
            detectAreaOfLevelCanvas.OnExit += (exittedCol) => { if (exittedCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) HideLevelCanvas(); };

            detectAreaOfLevelPreview.OnEnter += (enteredCol) => { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) ShowLevelInfoPreview(); };
            detectAreaOfLevelPreview.OnExit += (exittedCol) => { if (exittedCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) HideLevelInfoPreview(); };
        }

        void Start()
        {
            Color? glowColor = null;
            if (colors!=null)
                glowColor = levelInfo.CurrentSoulCount >= levelInfo.MaxSoulCount ? colors.CompletedLevel : colors.IncompleteLevel;
            var minimapIcon = Instantiate(minimapIconPrefab, transform);
            minimapIcon.Init(levelInfo.GameInfo.TitleIcon, glowColor);
        }

        public void Init(Action<LevelInfo> onShowLevelCanvas, Action onHideLevelCanvas, HubColors colors)
        {
            this.OnShowLevelCanvas = onShowLevelCanvas;
            this.OnHideLevelCanvas = onHideLevelCanvas;
            this.colors = colors;
        }

        public void ShowLevelCanvas()
        {
            HideLevelInfoPreview();
            OnShowLevelCanvas?.Invoke(levelInfo);
        }

        public void HideLevelCanvas()
        {
            OnHideLevelCanvas?.Invoke();
            ShowLevelInfoPreview();
        }

        public void ShowLevelInfoPreview()
        {
            currentLevelInfoPreview = Instantiate(levelInfoPreviewPrefab, levelInfoPreviewParent);
            currentLevelInfoPreview.Init(levelInfo);
            OnShowLevelInfoPreview?.Invoke();
        }

        public void HideLevelInfoPreview()
        {
            currentLevelInfoPreview.Exit();
            OnHideLevelInfoPreview?.Invoke();
        }

    }
}
