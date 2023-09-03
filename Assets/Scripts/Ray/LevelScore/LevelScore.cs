using Hanako.Hanako;
using Hanako.Knife;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="LvlScore_", menuName ="SO/Level Score")]

    public class LevelScore : ScriptableObject
    {
        public enum GameType { Hanako, Knife, TekeTeke }

        [SerializeField]
        GameInfo gameInfo;

        [SerializeField]
        GameType gameType;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(GameType) + "." + nameof(GameType.Hanako))]
        HanakoLevel hanakoLevel;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(GameType) + "." + nameof(GameType.Knife))]
        KnifeLevel knifeLevel;

        [Header("Runtime Data")]

        [SerializeField]
        int currentSoulCount;

        [SerializeField]
        int maxSoulCount = 3;

        [SerializeField]
        int score;

        [SerializeField]
        float playTime;

        public GameInfo GameInfo { get => gameInfo; }
        public string LevelName { get => gameType switch {
            GameType.Hanako => hanakoLevel.LevelName,
            GameType.Knife => knifeLevel.LevelName,
            _=> ""
        }; }
        public int Score { get => score; }
        public float PlayTime { get => playTime; }
        public int CurrentSoulCount { get => currentSoulCount; }
        public int MaxSoulCount { get => maxSoulCount; }
    }
}
