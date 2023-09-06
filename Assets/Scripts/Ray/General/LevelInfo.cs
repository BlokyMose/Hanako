using Hanako.Hanako;
using Hanako.Knife;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="LvlInfo_", menuName ="SO/Level Info")]
    [InlineEditor]
    public class LevelInfo : ScriptableObject
    {
        public enum GameType { Hub = -1, Hanako, Knife, TekeTeke }

        [SerializeField]
        GameInfo gameInfo;

        [SerializeField]
        GameType gameType;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(GameType) + "." + nameof(GameType.Hanako))]
        HanakoLevel hanakoLevel;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(GameType) + "." + nameof(GameType.Knife))]
        KnifeLevel knifeLevel;

        [SerializeField]
        int maxSoulCount = 3;

        [Header("Runtime Data")]

        [SerializeField]
        int currentSoulCount;

        [SerializeField]
        int score;

        [SerializeField]
        float playTime;

        public GameInfo GameInfo { get => gameInfo; }
        public string LevelName { get => gameType switch {
            GameType.Hub => "To Hub",
            GameType.Hanako => hanakoLevel.LevelName,
            GameType.Knife => knifeLevel.LevelName,
            _=> ""
        }; }
        public int Score { get => score; }
        public float PlayTime { get => playTime; }
        public int CurrentSoulCount { get => currentSoulCount; }
        public int MaxSoulCount { get => maxSoulCount; }

        public void ResetRuntimeData()
        {
            currentSoulCount = 0;
            score = 0;
            playTime = 0;
        }

        // TODO: make scoring rule, and evaluate it to set new currentSoulCount
    }
}
