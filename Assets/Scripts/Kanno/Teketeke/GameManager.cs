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
                //�X�R�A��10�����̎��̃X�R�A��\��
                if (iScore < 10)
                {
                    GameEnd1.GameEndShowPanel1();
                }
                //�X�R�A��10�ȏ�20�����̎��X�R�A��\��
                if ((10 <= iScore)&&(iScore < 20))
                {
                    GameEnd2.GameEndShowPanel2();
                }
                //�X�R�A��20�ȏ�̎��̃X�R�A��\��
                if (20 <= iScore)
                {
                    GameEnd3.GameEndShowPanel3();
                }


            }
        }

        public void SetScore()
        {
            //�X�R�A���J�E���g����
            iScore += 1;
            txtScore.text = "�X�R�A�F" + iScore.ToString();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //���ɓ���������
            if (other.gameObject.CompareTag("desk")) 
            {
                countdownSeconds -= 10; 
            }
            //�֎q�ɓ���������
            if (other.gameObject.CompareTag("chair"))
            {
                countdownSeconds -= 15;
            }
            //����ɓ���������
            if (other.gameObject.CompareTag("lectern"))
            {
                countdownSeconds -= 20;
            }
        }
    }
}
