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
    public class LevelRuntimeData
    {
        [SerializeField]
        int currentSoulCount;

        [SerializeField]
        int score;

        [SerializeField]
        float playTime;

        [SerializeField]
        bool hasShownTutorial = false;

        public LevelRuntimeData(int currentSoulCount = 0, int score = 0, float playTime = 0, bool hasShownTutorial = false)
        {
            this.currentSoulCount = currentSoulCount;
            this.score = score;
            this.playTime = playTime;
            this.hasShownTutorial = hasShownTutorial;
        }

        public int CurrentSoulCount { get => currentSoulCount; }
        public int Score { get => score; }
        public float PlayTime { get => playTime; }
        public bool HasShownTutorial { get => hasShownTutorial; }
    }
}
