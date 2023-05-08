using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako
{
    public class ScoreManager : MonoBehaviour
    {
        public Text txtScore;
        int iScore = 0;
        public void SetScore()
        {
            iScore += 1;
            txtScore.text = "スコア：" + iScore.ToString();
        }
    }
}
