using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Hanako.Dialogue.DialogueData;
using static Hanako.Dialogue.DialogueEnums;

namespace Hanako.Dialogue
{
    [RequireComponent(typeof(Animator))]
    public class DialogueBubble : MonoBehaviour
    {
        [SerializeField]
        string textLinePrefix = "<rotate=90>";

        [Header("Components")]
        [SerializeField]
        TextMeshProUGUI text;

        [SerializeField]
        Image bubbleImage;

        [SerializeField]
        List<LayoutGroup> layoutGroups = new();

        [SerializeField]
        List<ContentSizeFitter> contentSizeFitters = new();

        Animator animator;
        int int_mode;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
        }

        public void SetText(TextLine textLine, DialogueSettings settings)
        {
            this.text.text = textLinePrefix + textLine.Text;
            this.text.color = textLine.TextColor;
            Canvas.ForceUpdateCanvases();

            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = false;
            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = true;

            foreach (var cont in contentSizeFitters)
                cont.enabled = false;
            foreach (var cont in contentSizeFitters)
                cont.enabled = true;

            bubbleImage.sprite = settings.GetBubbleShape(textLine.BubbleShape);

            animator.SetInteger(int_mode, (int)textLine.BubbleAnimation);
        }

        public void Hide()
        {
            animator.SetInteger(int_mode, (int)BubbleAnimation.Hidden);
        }
    }
}
