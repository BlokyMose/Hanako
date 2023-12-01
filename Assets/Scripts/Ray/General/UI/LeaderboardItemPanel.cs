using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako
{
    public class LeaderboardItemPanel : MonoBehaviour
    {
        [SerializeField]
        List<Image> rankIcons = new();
        
        [SerializeField]
        TextMeshProUGUI rankText;

        [SerializeField]
        TextMeshProUGUI nameText;

        [SerializeField]
        TextMeshProUGUI totalScoreText;

        [SerializeField]
        Transform scoreTextsParent;

        [SerializeField]
        TextMeshProUGUI scoreTextPrefab;

        [SerializeField]
        List<GameInfo> gameInfosOrder = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameScores">Make sure the scoreText is displayed in order with the header</param>
        public void Init(int rank, string name, int totalScore, Dictionary<string, int> gameScores)
        {
            scoreTextsParent.DestroyChildren();

            if (rank > rankIcons.Count)
                rankText.text = rank.ToString() + ".";
            else
                Destroy(rankText.gameObject);

            for (int i = 0; i < rankIcons.Count; i++)
                if (i != rank - 1)
                    Destroy(rankIcons[i].gameObject);


            nameText.text = name;
            totalScoreText.text = totalScore.ToString();
            for (int i = 0; i < gameInfosOrder.Count; i++)
            {
                var scoreText = Instantiate(scoreTextPrefab, scoreTextsParent);
                if (gameScores.TryGetValue(gameInfosOrder[i].GameID, out var score))
                    scoreText.text = score.ToString();
                else
                    scoreText.text = "0";
            }
            

            Destroy(scoreTextPrefab.gameObject);
        }
    }
}
