using Hanako.Hanako;
using Hanako.Knife;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="LvlInfo_", menuName ="SO/Level Info")]
    [InlineEditor]
    public class LevelInfo : ScriptableObject
    {
        [SerializeField]
        GameInfo gameInfo;

        [SerializeField]
        GameType gameType;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(Hanako) + "." + nameof(GameType) + "." + nameof(GameType.Hanako))]
        HanakoLevel hanakoLevel;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(Hanako) + "." + nameof(GameType) + "." + nameof(GameType.Knife))]
        KnifeLevel knifeLevel;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(Hanako) + "." + nameof(GameType) + "." + nameof(GameType.TekeTeke))]
        TekeLevel tekeLevel;

        [SerializeField, ShowIf("@" + nameof(gameType) + "==" + nameof(Hanako) + "." + nameof(GameType) + "." + nameof(GameType.Kokkurisan))]
        KokkuriLevel kokkuriLevel;

        [SerializeField]
        ScoreRules scoreRules;

        [SerializeField, ListDrawerSettings(Expanded = true)]
        List<int> scoreThresholds = new() { 100, 200, 300 };

        [SerializeField]
        TutorialPreview tutorialPreview = new();

        [SerializeField]
        LevelRuntimeData runtimeData;

        public GameInfo GameInfo { get => gameInfo; }
        public string LevelName { get => gameType switch {
            GameType.Hub => "To Hub",
            GameType.Hanako => HanakoLevel.LevelName,
            GameType.Knife => KnifeLevel.LevelName,
            GameType.TekeTeke => TekeLevel.LevelName,
            GameType.Kokkurisan => KokkuriLevel.LevelName,
            _=> ""
        }; }
        public int CurrentScore { get => runtimeData.CurrentScore; }
        public float PlayTime { get => runtimeData.PlayTime; }
        public int CurrentSoulCount { get => runtimeData.CurrentSoulCount; }
        public int MaxSoulCount { get => scoreThresholds.Count; }
        public ScoreRules ScoreRules { get => scoreRules; }
        public HanakoLevel HanakoLevel { get => hanakoLevel; }
        public KnifeLevel KnifeLevel { get => knifeLevel; }
        public TekeLevel TekeLevel { get => tekeLevel; }
        public KokkuriLevel KokkuriLevel { get => kokkuriLevel; }
        public List<int> ScoreThresholds { get => scoreThresholds;  }
        public GameType GameType { get => gameType; }
        public TutorialPreview TutorialPreview { get => tutorialPreview;  }
        public bool HasShownTutorial { get => runtimeData.HasShownTutorial; }

        public void ResetRuntimeData()
        {
            runtimeData = new();
        }

        public void SetRuntimeData(LevelRuntimeData newData)
        {
            runtimeData = newData;
        }

        // TODO: set currentSouldCount when evaluating score for each game
    }
}
