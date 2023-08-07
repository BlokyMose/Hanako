using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
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

        public Color PlayerColor { get => playerColor; }
        public Color OccupiedColor { get => occupiedColor; }
        public Color HoverColor { get => hoverColor; }
    }
}
