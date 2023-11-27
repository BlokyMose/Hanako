using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako
{
    public class Ray_Teke_GameManager : GameManager
    {
        [Header("Ray")]
        [SerializeField]
        LevelInfo levelInfo;

        [SerializeField]
        string playTimeParamName = "playTime";

        [SerializeField]
        string scoreParamName = "score";
        bool isGameOver = false;
        float playTime = 0;

        // Update is called once per frame
        void Update()
        {
            playTime += Time.deltaTime;
            countdownSeconds -= Time.deltaTime;

            countdownSeconds = Mathf.Max(countdownSeconds, 0);

            var span = new TimeSpan(0, 0, (int)countdownSeconds);
            timeText.text = span.ToString(@"mm\:ss");

            if (countdownSeconds <= 0 && !isGameOver)
            {
                isGameOver = true;

                var scoreCanvas = FindObjectOfType<ScoreCanvas>(true);
                scoreCanvas.gameObject.SetActive(true);
                scoreCanvas.Init(levelInfo, new List<ScoreDetail>()
                {
                    new ScoreDetail(playTimeParamName, (int)playTime),
                    new ScoreDetail(scoreParamName, iScore)
                });
            }
        }

    }
}
