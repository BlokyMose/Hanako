using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hanako
{
    public class GameEndTrigger : MonoBehaviour
    {
        public GameObject scorePanel; // �X�R�A��\������p�l��
        public TMP_Text scoreText; // �X�R�A��\������e�L�X�g
        private bool gameEnded = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !gameEnded)
            {
                gameEnded = true;

                // �Q�[�����~�i�K�v�ɉ����ăQ�[���I�[�o�[��ʂȂǂ�\���j
                Time.timeScale = 0f;

                // �X�R�A���擾���ĕ\��
                int score = FindObjectOfType<AlphaChange>().GetScore(); // AlphaChange �X�N���v�g����X�R�A���擾
                scoreText.text = "Score: " + score;

                // �X�R�A�p�l����\��
                scorePanel.SetActive(true);
            }
        }
    }
}
