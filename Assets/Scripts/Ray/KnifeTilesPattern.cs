using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [InlineEditor]
    public abstract class KnifeTilesPattern : ScriptableObject
    {
        [SerializeField]
        string patternName;

        public string PatternName { get => patternName; }

        public abstract GameObject GetTile(ColRow colRow, KnifeLevel levelProperties);

    }
}