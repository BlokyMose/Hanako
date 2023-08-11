using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [InlineEditor]
    [CreateAssetMenu(fileName ="HanakoColors", menuName ="SO/Hanako/Colors")]

    public class HanakoColors : ScriptableObject
    {
        [SerializeField]
        string colorsName;

        [SerializeField]
        Color playerColor;

        [SerializeField]
        Color occupiedColor;

        [SerializeField]
        Color hoverColor;

        [SerializeField]
        Color detectingColor;

        [SerializeField]
        Color attackableColor;

        [SerializeField]
        Color loadingBarColor;

        public Color PlayerColor { get => playerColor; }
        public Color OccupiedColor { get => occupiedColor; }
        public Color HoverColor { get => hoverColor; }
        public Color DetectingColor { get => detectingColor; }
        public Color AttackableColor { get => attackableColor; }
        public Color LoadingBarColor { get => loadingBarColor; }
    }
}
