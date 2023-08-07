using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hanako
{
    public class GameManager : MonoBehaviour
    {

        public float countdownMinutes = 3;

        private float countdownSeconds;

        public TextMeshProUGUI timeText;

        public TextMeshProUGUI txtScore;
        int iScore = 0;

        // Start is called before the first frame update
        void Start()
        {
            countdownSeconds = countdownMinutes * 60;
        }

        // Update is called once per frame
        void Update()
        {
            countdownSeconds -= Time.deltaTime;
            var span = new TimeSpan(0, 0, (int)countdownSeconds);
            timeText.text = span.ToString(@"mm\:ss");

            if (countdownSeconds <= 0)
            {
                Time.timeScale = 0;
                //スコアが10未満の時のスコアを表示
                if (iScore < 10)
                {
                    GameEnd1.GameEndShowPanel1();
                }
                //スコアが10以上20未満の時スコアを表示
                if ((10 <= iScore)&&(iScore < 20))
                {
                    GameEnd2.GameEndShowPanel2();
                }
                //スコアが20以上の時のスコアを表示
                if (20 <= iScore)
                {
                    GameEnd3.GameEndShowPanel3();
                }


            }
        }

        public void SetScore()
        {
            //スコアをカウントする
            iScore += 1;
            txtScore.text = "スコア：" + iScore.ToString();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //机に当たった時
            if (other.gameObject.CompareTag("desk")) 
            {
                countdownSeconds -= 10; 
            }
            //椅子に当たった時
            if (other.gameObject.CompareTag("chair"))
            {
                countdownSeconds -= 15;
            }
            //教卓に当たった時
            if (other.gameObject.CompareTag("lectern"))
            {
                countdownSeconds -= 20;
            }
        }
    }
}
