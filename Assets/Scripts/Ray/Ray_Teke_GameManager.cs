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
        string scoreParamName = "score";
        bool isGameOver = false;
        float playTime = 0;

        // Update is called once per frame
        void Update()
        {
            playTime += Time.deltaTime;
            countdownSeconds -= Time.deltaTime;
            var span = new TimeSpan(0, 0, (int)countdownSeconds);
            timeText.text = span.ToString(@"mm\:ss");

            if (countdownSeconds <= 0 && !isGameOver)
            {
                isGameOver = true;

                var scoreCanvas = FindObjectOfType<ScoreCanvas>(true);
                scoreCanvas.gameObject.SetActive(true);
                scoreCanvas.Init(levelInfo, new List<ScoreDetail>()
                {
                    new ScoreDetail(scoreParamName, iScore),
                    new ScoreDetail("playTime", (int)playTime)
                });

                //Time.timeScale = 0;

                ////�X�R�A��10�����̎��̃X�R�A��\��
                //if (iScore < 10)
                //{
                //    GameEnd1.GameEndShowPanel1();
                //}
                ////�X�R�A��10�ȏ�20�����̎��X�R�A��\��
                //if ((10 <= iScore) && (iScore < 20))
                //{
                //    GameEnd2.GameEndShowPanel2();
                //}
                ////�X�R�A��20�ȏ�̎��̃X�R�A��\��
                //if (20 <= iScore)
                //{
                //    GameEnd3.GameEndShowPanel3();
                //}
            }
        }

    }
}
