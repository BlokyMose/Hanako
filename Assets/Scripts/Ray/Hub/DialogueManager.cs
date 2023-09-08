using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using static Hanako.Dialogue.DialogueData;

namespace Hanako.Dialogue
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueManager : MonoBehaviour
    {
        class Char
        {
            CharID charID;
            DialogueCharParent charParent;

            public Char(CharID charID, DialogueCharParent charParent)
            {
                this.charID = charID;
                this.charParent = charParent;
            }

            public void Reset(int sortingOrder)
            {
                charParent.ResetAll(sortingOrder);
            }

            public void End()
            {
                charParent.End();
            }

            public void EvaluateThenSay(TextLine textLine, DialogueSettings settings, int sortingOrder)
            {
                if (textLine.CharID != charID)
                    return;
                charParent.Say(textLine, settings, sortingOrder);
            }
        }

        [SerializeField]
        DialogueSettings defaultDialogueSettings;

        [Header("Components")]
        [SerializeField]
        Image clickToContinueImage;

        [SerializeField]
        Transform dialogueCharsParent;

        [Header("BG")]
        [SerializeField]
        CanvasGroup bgCanvas;

        [SerializeField]
        float bgAnimationDuration = 0.5f;

        [Header("Character")]
        [SerializeField]
        int charDialogueLayer = 6;

        [SerializeField]
        int charNonDialogueLayer = 4;

        [SerializeField]
        DialogueCharParent dialogueCharPrefab;

        [Header("Debug")]
        [SerializeField, InlineButton("@StartDialogue()", "Test")]
        DialogueData testDialogueData;

        List<Char> chars = new();
        Coroutine corBG;
        int currentTextLineIndex;
        DialogueData currentDialogueData;
        DialogueSettings currentDialogueSettings;   
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            clickToContinueImage.AddEventTrigger(NextTextLine);
            HideBG();
            DeactivateCanvasGroup();
            dialogueCharsParent.DestroyChildren();
        }

        public void StartDialogue()
        {
            StartDialogue(testDialogueData);
        }

        public void StartDialogue(DialogueData dialogueData)
        {
            currentDialogueData = dialogueData;
            currentDialogueSettings = dialogueData.Settings != null ? dialogueData.Settings : defaultDialogueSettings;
            currentTextLineIndex = 0;
            chars.Clear();
            dialogueCharsParent.DestroyChildren();
            for (int i = 0; i < dialogueData.Chars.Count; i++)
            {
                if (i >= dialogueData.CharPos.AllPos.Count) 
                    break;

                var charID = dialogueData.Chars[i].CharID;
                var charPos = dialogueData.CharPos.AllPos[i];
                var newChar = Instantiate(dialogueCharPrefab, dialogueCharsParent);
                newChar.transform.localPosition = charPos.Pos;
                newChar.Init(charID, currentDialogueSettings.CharScale, charPos.IsFacingRight);

                chars.Add(new Char(charID, newChar));
            }

            ShowBG();
            ActivateCanvasGroup();

            NextTextLine();
        }

        public void NextTextLine()
        {
            if (currentDialogueData == null) return;

            if (currentTextLineIndex < currentDialogueData.TextLines.Count)
            {
                foreach (var character in chars)
                {
                    character.Reset(charNonDialogueLayer);
                    character.EvaluateThenSay(
                        currentDialogueData.TextLines[currentTextLineIndex],
                        currentDialogueSettings,
                        charDialogueLayer);
                }
                currentTextLineIndex++;
            }
            else
            {
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            foreach (var character in chars)
                character.End();
            currentDialogueData = null;
            HideBG();
            DeactivateCanvasGroup();
        }

        void ActivateCanvasGroup()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;
        }

        void DeactivateCanvasGroup()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }


        void HideBG()
        {
            corBG = this.RestartCoroutine(ChangeBGAlpha(0f),corBG);
        }

        void ShowBG()
        {
            corBG = this.RestartCoroutine(ChangeBGAlpha(1f),corBG);
        }

        IEnumerator ChangeBGAlpha(float alphaTarget)
        {
            var curve = AnimationCurve.Linear(0, bgCanvas.alpha, bgAnimationDuration, alphaTarget);
            var time = 0f;
            while (time > bgAnimationDuration)
            {
                bgCanvas.alpha = curve.Evaluate(time);
                time += Time.deltaTime;
                yield return null;
            }
            bgCanvas.alpha = alphaTarget;
        }
    }
}
