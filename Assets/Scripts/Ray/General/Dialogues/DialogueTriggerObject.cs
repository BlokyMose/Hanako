using Hanako.Hub;
using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Dialogue
{
    public class DialogueTriggerObject : MonoBehaviour
    {
        public enum CharacterAnimation { Idle = 0, WaveMuch = 14 }

        [SerializeField]
        DialogueData dialogueData;

        [SerializeField]
        CharID charID;

        [Header("Components")]
        [SerializeField]
        ColliderProxy detectArea;

        [SerializeField]
        ColliderProxy detectAreaOfPreview;

        [SerializeField]
        DialoguePreviewCanvas dialoguePreviewCanvasPrefab;

        [SerializeField]
        Transform dialoguePreviewParent;

        [SerializeField]
        Animator characterAnimator;

        [Header("Minimap")]
        [SerializeField]
        HubMinimapIcon minimapIconPrefab;

        [SerializeField]
        Transform minimapIconParent;

        DialoguePreviewCanvas currentDialoguePreviewCanvas;
        Action<DialogueData> OnShowDialogueTriggerCanvas;
        Action OnHideDialogueTriggerCanvas;
        Coroutine corDeletingDialoguePreview;
        int int_motion;

        void Awake()
        {
            int_motion = Animator.StringToHash(nameof(int_motion));

            detectArea.OnEnter += (enteredCol) => 
            { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) ShowDialogueTriggerCanvas(); };
            detectArea.OnExit += (enteredCol) => 
            { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) HideDialogueTriggerCanvas(); };

            detectAreaOfPreview.OnEnter += (enteredCol) =>
            { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) ShowDialoguePreviewCanvas(); };
            detectAreaOfPreview.OnExit += (enteredCol) =>
            { if (enteredCol.TryGetComponent<HubCharacterBrain_Player>(out var player)) HideDialoguePreviewCanvas(); };

            if (minimapIconPrefab != null)
            {
                var minimapIcon = Instantiate(minimapIconPrefab, minimapIconParent);
                minimapIcon.Init(charID.Icon);
            }

        }

        public void Init(Action<DialogueData> onShowDialogueTriggerCanvas, Action onHideDialogueTriggerCanvas, ref Action<DialogueData> onDialogueStart)
        {
            this.OnShowDialogueTriggerCanvas = onShowDialogueTriggerCanvas;
            this.OnHideDialogueTriggerCanvas = onHideDialogueTriggerCanvas;
            onDialogueStart += (dialogueData) => { HideDialoguePreviewCanvas(); };
        }

        void ShowDialogueTriggerCanvas()
        {
            OnShowDialogueTriggerCanvas?.Invoke(dialogueData);
        }

        void HideDialogueTriggerCanvas()
        {
            OnHideDialogueTriggerCanvas?.Invoke();
        }

        void ShowDialoguePreviewCanvas()
        {
            if (corDeletingDialoguePreview != null)
                StopCoroutine(corDeletingDialoguePreview);
            else
                currentDialoguePreviewCanvas = Instantiate(dialoguePreviewCanvasPrefab, dialoguePreviewParent);
            currentDialoguePreviewCanvas.Show(charID);
            characterAnimator.SetInteger(int_motion, (int)CharacterMotion.WaveMuch);
        }

        void HideDialoguePreviewCanvas()
        {
            currentDialoguePreviewCanvas.Hide();
            corDeletingDialoguePreview = this.RestartCoroutine(Delay(1f), corDeletingDialoguePreview);
            characterAnimator.SetInteger(int_motion, (int)CharacterMotion.Idle);

            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                Destroy(currentDialoguePreviewCanvas);
                corDeletingDialoguePreview = null;
            }
        }
    }
}
