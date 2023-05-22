using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Hanako
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI txtScore;
        int iScore = 0;
        public void SetScore()
        {
            iScore += 1;
            txtScore.text = "スコア：" + iScore.ToString();
        }
    }
}
