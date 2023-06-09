using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [InlineEditor]

    public abstract class KnifeWallsPattern : ScriptableObject
    {
        [SerializeField]
        string patternName;

        public string PatternName { get => patternName; }
        public Color LeftWallColor { get => leftWallColor;  }
        public Color RightWallColor { get => rightWallColor;  }

        [SerializeField]
        Color leftWallColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        [SerializeField]
        Color rightWallColor = Color.white;

        public abstract GameObject GetLeftWall(int index, KnifeLevel levelProperties);
        public abstract GameObject GetRightWall(int index, KnifeLevel levelProperties);
    }
}
