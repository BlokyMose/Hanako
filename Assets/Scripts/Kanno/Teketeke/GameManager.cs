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
                if(iScore < 10)
                {
                    GameEnd1.GameEndShowPanel1();
                }

                if (10 <= iScore)
                {
                    GameEnd2.GameEndShowPanel2();
                }

                if (50 <= iScore)
                {
                    GameEnd3.GameEndShowPanel3();
                }


            }
        }

        public void SetScore()
        {
            iScore += 1;
            txtScore.text = "スコア：" + iScore.ToString();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("desk"))
            {
                countdownSeconds -= 10;
            }

            if (other.gameObject.CompareTag("chair"))
            {
                countdownSeconds -= 10;
            }
            if (other.gameObject.CompareTag("lectern"))
            {
                countdownSeconds -= 10;
            }
        }
    }
}
