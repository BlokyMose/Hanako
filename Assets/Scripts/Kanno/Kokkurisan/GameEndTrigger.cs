using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hanako
{
    public class GameEndTrigger : MonoBehaviour
    {
        public GameObject scorePanel; // スコアを表示するパネル
        public TMP_Text scoreText; // スコアを表示するテキスト
        private bool gameEnded = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !gameEnded)
            {
                gameEnded = true;

                // ゲームを停止（必要に応じてゲームオーバー画面などを表示）
                Time.timeScale = 0f;

                // スコアを取得して表示
                int score = FindObjectOfType<AlphaChange>().GetScore(); // AlphaChange スクリプトからスコアを取得
                scoreText.text = "Score: " + score;

                // スコアパネルを表示
                scorePanel.SetActive(true);
            }
        }
    }
}
