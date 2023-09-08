using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Dialogue.DialogueEnums;

namespace Hanako.Dialogue
{
    [CreateAssetMenu(fileName ="DialogueSettings", menuName ="SO/Dialogue/Settings")]

    public class DialogueSettings : ScriptableObject
    {
        [SerializeField]
        float charScale = 188f;

        [Header("Sprites")]

        [SerializeField, PreviewField]
        Sprite bubbleShape_Round;

        [SerializeField, PreviewField]
        Sprite bubbleShape_Rectangle;

        [SerializeField, PreviewField]
        Sprite bubbleShape_Spiky;

        [SerializeField, PreviewField]
        Sprite bubbleShape_Wiggly;

        [SerializeField, PreviewField]
        Sprite bubbleShape_Grumpy;

        [SerializeField, PreviewField]
        Sprite bubbleShape_RectangleBlack;

        public Sprite BubbleShape_Round { get => bubbleShape_Round; }
        public Sprite BubbleShape_Rectangle { get => bubbleShape_Rectangle; }
        public Sprite BubbleShape_Spiky { get => bubbleShape_Spiky; }
        public float CharScale { get => charScale; }

        public Sprite GetBubbleShape(BubbleShape bubbleShape)
        {
            return bubbleShape switch
            {
                BubbleShape.Round => bubbleShape_Round,
                BubbleShape.Rectangle => bubbleShape_Rectangle,
                BubbleShape.Spiky => bubbleShape_Spiky,
                BubbleShape.Wiggly => bubbleShape_Wiggly,
                BubbleShape.Grumpy => bubbleShape_Grumpy,
                BubbleShape.RectangleBlack => bubbleShape_RectangleBlack,
                _ => bubbleShape_Round
            };

        }
    }
}
