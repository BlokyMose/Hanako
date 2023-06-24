using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName ="KnifePieceInfo_", menuName ="SO/Knife/Piece Information")]

    public class KnifePieceInformation : ScriptableObject
    {
        public enum NamedColors { Yellow, Magenta, Orange, Green, Blue }

        [SerializeField]
        string pieceName;

        [SerializeField, Multiline]
        string desc;

        [SerializeField]
        NamedColors color = NamedColors.Magenta;

        public string PieceName { get => pieceName; }
        public string Desc { get => desc; }
        public Color Color { get
            {
                switch (color)
                {
                    case NamedColors.Yellow:
                        return Color.HSVToRGB(55 / 365f, 0.7f, 1f);
                    case NamedColors.Magenta:
                        return Color.HSVToRGB(325 / 365f, 0.7f, 1f);
                    case NamedColors.Orange:
                        return Color.HSVToRGB(24 / 365f, 0.7f, 1f);
                    case NamedColors.Green:
                        return Color.HSVToRGB(110 / 365f, 0.7f, 1f);
                    case NamedColors.Blue:
                        return Color.HSVToRGB(200 / 365f, 0.7f, 1f);
                    default:
                        return Color.HSVToRGB(55 / 365f, 0.7f, 1f);
                }
            } 
        }

    }
}
