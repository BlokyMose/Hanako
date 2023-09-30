using Hanako.Hub;
using Hanako.Knife;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Dialogue.DialogueEnums;

namespace Hanako.Dialogue
{
    public class DialogueTriggerObject : MonoBehaviour
    {
        public enum CharacterAnimation { Idle = 0, WaveMuch = 14 }

        [SerializeField]
        DialogueData dialogueData;

        [SerializeField, InlineButton(nameof(InstantiateCharacter), "Show")]
        CharID charID;

        [SerializeField]
        SwapCharMode swapMode;

        [SerializeField, ShowIf("@"+nameof(swapMode)+".HasFlag("+nameof(DialogueEnums)+"."+nameof(DialogueEnums.SwapCharMode)+"."+nameof(DialogueEnums.SwapCharMode.Index)+")")]
        int swapWithIndex = 1;

        [SerializeField, ShowIf("@"+nameof(swapMode)+".HasFlag("+nameof(DialogueEnums)+"."+nameof(DialogueEnums.SwapCharMode)+"."+nameof(DialogueEnums.SwapCharMode.CharID)+")")]
        CharID swapWithCharID;

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
        Transform characterPos;

        [Header("Minimap")]
        [SerializeField]
        HubMinimapIcon minimapIconPrefab;

        [SerializeField]
        Transform minimapIconParent;

        Animator characterAnimator;
        DialoguePreviewCanvas currentDialoguePreviewCanvas;
        Action<DialogueRuntimeData> OnShowDialogueTriggerCanvas;
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

            dialoguePreviewParent.DestroyChildren();
            InstantiateCharacter();
        }

        public void Init(Action<DialogueRuntimeData> onShowDialogueTriggerCanvas, Action onHideDialogueTriggerCanvas, ref Action<DialogueRuntimeData> onDialogueStart)
        {
            this.OnShowDialogueTriggerCanvas = onShowDialogueTriggerCanvas;
            this.OnHideDialogueTriggerCanvas = onHideDialogueTriggerCanvas;
            onDialogueStart += (dialogueData) => { if (dialogueData.DialogueData == this.dialogueData) HideDialoguePreviewCanvas(); };
        }

        public void InstantiateCharacter()
        {
            if (charID == null) return;
            characterPos.DestroyChildren();
            var character = Instantiate(charID.Prefab, characterPos);
            character.transform.localPosition = Vector3.zero;
            characterAnimator = character.GetComponent<Animator>();
        }

        void ShowDialogueTriggerCanvas()
        {
            OnShowDialogueTriggerCanvas?.Invoke(new DialogueRuntimeData(
                dialogueData: dialogueData, 
                characters: new() {new(charID,swapMode, swapWithIndex, swapWithCharID)}));
        }

        void HideDialogueTriggerCanvas()
        {
            OnHideDialogueTriggerCanvas?.Invoke();
        }

        void ShowDialoguePreviewCanvas()
        {
            if (corDeletingDialoguePreview != null)
                StopCoroutine(corDeletingDialoguePreview);
            if (currentDialoguePreviewCanvas == null)
                currentDialoguePreviewCanvas = Instantiate(dialoguePreviewCanvasPrefab, dialoguePreviewParent);
            currentDialoguePreviewCanvas.Show(charID);
            characterAnimator.SetInteger(int_motion, (int)CharacterMotion.WaveMuch);
        }

        void HideDialoguePreviewCanvas()
        {
            if (currentDialoguePreviewCanvas == null) return;
            currentDialoguePreviewCanvas.Hide();
            corDeletingDialoguePreview = this.RestartCoroutine(Delay(1f), corDeletingDialoguePreview);
            characterAnimator.SetInteger(int_motion, (int)CharacterMotion.Idle);

            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                Destroy(currentDialoguePreviewCanvas.gameObject);
                currentDialoguePreviewCanvas = null;
                corDeletingDialoguePreview = null;
            }
        }
    }
}
