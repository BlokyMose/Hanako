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
        int currentScore;

        [SerializeField]
        int currentSoulCount;

        [SerializeField]
        float playTime;

        [SerializeField]
        bool hasShownTutorial = false;

        public LevelRuntimeData(int currentScore = 0, int currentSoulCount = 0,  float playTime = 0, bool hasShownTutorial = false)
        {
            this.currentScore = currentScore;
            this.currentSoulCount = currentSoulCount;
            this.playTime = playTime;
            this.hasShownTutorial = hasShownTutorial;
        }


        public int CurrentScore { get => currentScore; }
        public int CurrentSoulCount { get => currentSoulCount; }
        public float PlayTime { get => playTime; }
        public bool HasShownTutorial { get => hasShownTutorial; }
    }
}
