using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Dialogue.DialogueEnums;

namespace Hanako.Dialogue
{
    [CreateAssetMenu(fileName ="Dialogue_", menuName ="SO/Dialogue/Dialogue")]

    public class DialogueData : ScriptableObject
    {

        [System.Serializable]
        public class CharProperties
        {
            [SerializeField, HideLabel]
            CharID charID;

            public CharID CharID { get => charID; }
        }

        [System.Serializable]
        public class TextLine
        {
            [SerializeField]
            CharID charID;

            [SerializeField, Multiline]
            string text;

            [SerializeField]
            FaceAnimation faceAnimation;

            [SerializeField]
            BodyAnimation bodyAnimation;

            [SerializeField]
            BubbleShape bubbleShape = BubbleShape.Round;

            [SerializeField]
            BubbleAnimation bubbleAnimation = BubbleAnimation.Still;

            [SerializeField]
            Color textColor = Color.black;

            public CharID CharID { get => charID; }
            public string Text { get => text; }
            public FaceAnimation FaceAnimation { get => faceAnimation;  }
            public BubbleShape BubbleShape { get => bubbleShape; }
            public BubbleAnimation BubbleAnimation { get => bubbleAnimation; }
            public Color TextColor { get => textColor; }
            public BodyAnimation BodyAnimation { get => bodyAnimation;  }
        }

        [SerializeField]
        DialogueSettings settings;

        [SerializeField]
        List<CharProperties> charProperties = new();

        [SerializeField, InlineEditor]
        DialogueCharPos charPos;

        [SerializeField]
        List<TextLine> textLines = new();

        public DialogueSettings Settings { get => settings; }
        public DialogueCharPos CharPos { get => charPos; }
        public List<CharProperties> Chars { get => charProperties; }
        public List<TextLine> TextLines { get => textLines; }
    }
}
