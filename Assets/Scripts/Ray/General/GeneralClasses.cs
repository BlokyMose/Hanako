using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [Serializable]
    public class ScoreDetail
    {
        string paramName;
        int value;

        public ScoreDetail(string paramName, int value)
        {
            this.paramName = paramName;
            this.value = value;
        }

        public string ParamName { get => paramName; }
        public int Value { get => value; }
    }

    [Serializable]
    public class TutorialPreview
    {
        [SerializeField]
        int startIndex = 0;

        [SerializeField]
        int endIndex = 3;

        [SerializeField]
        bool isAutoShow = false;

        public int InitialShowIndex { get => startIndex; }
        public int EndIndex { get => endIndex; }
        public bool IsAutoShow { get => isAutoShow; }
    }

    [Serializable]
    public class LeaderboardItem
    {
        [SerializeField]
        int score;
        [SerializeField]
        string playerID;

        public LeaderboardItem(int score, string playerID)
        {
            this.score = score;
            this.playerID = playerID;
        }

        public int Score { get => score; }
        public string PlayerID { get => playerID; }
    }

    [Serializable]
    public class PlayerID
    {
        [SerializeField]
        string displayName;

        [SerializeField]
        string id;

        public PlayerID(string displayName, string id)
        {
            this.displayName = displayName;
            this.id = id;
        }

        public string DisplayName { get => displayName; }
        public string ID { get => id; }
    }

    [Serializable]
    public class LevelRuntimeData
    {
        [SerializeField]
        int currentScore;

        [SerializeField]
        int currentSoulCount;

        [SerializeField]
        float playTime;

        [SerializeField]
        bool hasShownTutorial = false;

        [SerializeField]
        List<LeaderboardItem> leaderboard = new();

        public LevelRuntimeData(int currentScore = 0, int currentSoulCount = 0,  float playTime = 0, bool hasShownTutorial = false, List<LeaderboardItem> leaderboard = null)
        {
            this.currentScore = currentScore;
            this.currentSoulCount = currentSoulCount;
            this.playTime = playTime;
            this.hasShownTutorial = hasShownTutorial;
            this.leaderboard = leaderboard != null ? leaderboard : new();
        }

        public int CurrentScore { get => currentScore; }
        public int CurrentSoulCount { get => currentSoulCount; }
        public float PlayTime { get => playTime; }
        public bool HasShownTutorial { get => hasShownTutorial; }
        public List<LeaderboardItem> Leaderboard { get => leaderboard; }

        public void AddLeaderboardItem(LeaderboardItem newItem)
        {
            var previousRecordItem = Leaderboard.Find(x => x.PlayerID == newItem.PlayerID);
            if (previousRecordItem != null)
                if (previousRecordItem.Score < newItem.Score)
                    Leaderboard.Remove(previousRecordItem);
                else 
                    return;
            
            for (int i = 0; i < Leaderboard.Count; i++)
                if (newItem.Score > Leaderboard[i].Score)
                {
                    Leaderboard.Insert(i, newItem);
                    return;
                }

            Leaderboard.Add(newItem);
        }
    }

    [Serializable]
    public class ActionIconPack
    {
        [SerializeField, PreviewField]
        Sprite icon;
        public Sprite Icon => icon;

        [SerializeField]
        bool isOverrideColor = false;
        public bool IsOverrideColor => isOverrideColor;

        [SerializeField, ShowIf(nameof(isOverrideColor))]
        Color color = Color.white;
        public Color Color => color;

        [SerializeField]
        ActionIconMode animation;
        public ActionIconMode Animation => animation;

        public ActionIconPack(Sprite icon, bool isOverrideColor, Color color, ActionIconMode animation)
        {
            this.icon = icon;
            this.isOverrideColor = isOverrideColor;
            this.color = color;
            this.animation = animation;
        }
    }
}
