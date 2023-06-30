using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [InlineEditor]
    [CreateAssetMenu(fileName ="KnifeColors_", menuName ="SO/Knife/Colors")]

    public class KnifeColors : ScriptableObject
    {
        [SerializeField]
        string colorsName;

        [Header("Tile")]
        [SerializeField]
        Color tileNotMyTurnColor = Color.gray;

        [SerializeField]
        Color tileInvalidMoveColor = Color.red;

        [SerializeField]
        Color tileValidMoveColor = Color.green;

        [SerializeField]
        Color tileActionColor = Color.magenta;

        [SerializeField]
        Color tileClickColor = Color.magenta;

        [SerializeField]
        Color tileOtherValidMoveColor = Color.green;

        public Color TileInvalidMoveColor { get => tileInvalidMoveColor; }
        public Color TileValidMoveColor { get => tileValidMoveColor; }
        public Color TileActionColor { get => tileActionColor; }
        public Color TileClickColor { get => tileClickColor; }
        public Color TileNotMyTurnColor { get => tileNotMyTurnColor; }
        public Color TileOtherValidMoveColor { get => tileOtherValidMoveColor; }
    }
}
