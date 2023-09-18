using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Dialogue
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animator))]
    public class DialogueTriggerCanvas : MonoBehaviour
    {
        public enum TalkButState { Pressed, Idle, Hover }

        [SerializeField]
        DialogueManager dialogueManager;

        [SerializeField]
        Image talkBut;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom audioSource;

        [SerializeField]
        AudioSourceRandom buttonAudioSource;

        [SerializeField]
        string sfxShowName = "sfxShow";
        
        [SerializeField]
        string sfxHideName = "sfxHide";
        
        [SerializeField]
        string sfxHoverName = "sfxHover";

        [SerializeField]
        string sfxClickName = "sfxClick";

        int int_mode, boo_show;
        bool isInDialogue;

        CanvasGroup canvasGroup;
        Animator animator, talkButAnimator;
        DialogueData currentDialogueData;
        public event Action<DialogueData> OnStartDialogue;

        void Awake()
        {
            if (dialogueManager == null)
                dialogueManager = FindObjectOfType<DialogueManager>();
            dialogueManager.OnDialogueEnd += ResetIsInDialogue;
            OnStartDialogue = dialogueManager.StartDialogue;

            animator = GetComponent<Animator>();
            canvasGroup = GetComponent<CanvasGroup>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            boo_show = Animator.StringToHash(nameof(boo_show));

            talkButAnimator = talkBut.GetComponent<Animator>();
            talkBut.AddEventTriggers(
                onEnter: OnTalkButEnter,
                onExit: OnTalkButExit,
                onDown: OnTalkButDown,
                onClick: OnTalkButClick);

            void OnTalkButEnter()
            {
                if (isInDialogue) return;
                talkButAnimator.SetInteger(int_mode, (int)TalkButState.Hover);
                buttonAudioSource.PlayOneClipFromPack(sfxHoverName);
            }

            void OnTalkButExit()
            {
                if (isInDialogue) return;
                talkButAnimator.SetInteger(int_mode, (int)TalkButState.Idle);
                buttonAudioSource.PlayOneClipFromPack(sfxHoverName);
            }
                        
            
            void OnTalkButDown()
            {
                if (isInDialogue) return;
                talkButAnimator.SetInteger(int_mode, (int)TalkButState.Pressed);
                buttonAudioSource.PlayOneClipFromPack(sfxClickName);
            }

            void OnTalkButClick()
            {
                if (isInDialogue) return;
                StartDialogue();
            }

            var dialogueTriggers = new List<DialogueTriggerObject>(FindObjectsByType<DialogueTriggerObject>(FindObjectsSortMode.None));
            foreach (var dialogueTrigger in dialogueTriggers)
                dialogueTrigger.Init(Show, Hide, ref OnStartDialogue);
        }

        void OnDestroy()
        {
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnd -= ResetIsInDialogue;
            }
        }

        public void Show(DialogueData dialogueData)
        {
            if (isInDialogue) return;

            currentDialogueData = dialogueData;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            animator.SetBool(boo_show, true);
            audioSource.PlayOneClipFromPack(sfxShowName);

            StartCoroutine(Delay(0.5f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                talkButAnimator.SetInteger(int_mode, (int)TalkButState.Idle);
            }

        }

        public void Hide()
        {
            currentDialogueData = null;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            animator.SetBool(boo_show, false);
            talkButAnimator.SetInteger(int_mode, (int)TalkButState.Pressed);
            audioSource.PlayOneClipFromPack(sfxHideName);
        }

        void StartDialogue()
        {
            isInDialogue = true;
            OnStartDialogue?.Invoke(currentDialogueData);
            Hide();
        }

        public void ResetIsInDialogue()
        {
            isInDialogue = false;
        }
    }
}
