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
        Transform levelInfoPreviewParent;

        [SerializeField]
        HubLevelInfoPreview levelInfoPreviewPrefab;

        Action<LevelInfo> OnShowLevelCanvas;
        Action OnHideLevelCanvas;
        HubLevelInfoPreview currentLevelInfoPreview;

        public LevelInfo LevelInfo { get => levelInfo; }

        void Awake()
        {
            levelInfoPreviewParent.DestroyChildren();

            detectAreaOfLevelCanvas.OnEnter += (enteredCol) => { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) ShowLevelCanvas(); };
            detectAreaOfLevelCanvas.OnExit += (exittedCol) => { if (exittedCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) HideLevelCanvas(); };

            detectAreaOfLevelPreview.OnEnter += (enteredCol) => { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) ShowLevelInfoPreview(); };
            detectAreaOfLevelPreview.OnExit += (exittedCol) => { if (exittedCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) HideLevelInfoPreview(); };
        }

        public void Init(Action<LevelInfo> onShowLevelCanvas, Action onHideLevelCanvas)
        {
            this.OnShowLevelCanvas = onShowLevelCanvas;
            this.OnHideLevelCanvas = onHideLevelCanvas;
        }

        public void ShowLevelCanvas()
        {
            OnShowLevelCanvas?.Invoke(levelInfo);
            HideLevelInfoPreview();
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
        }

        public void HideLevelInfoPreview()
        {
            currentLevelInfoPreview.Exit();
        }
    }
}
