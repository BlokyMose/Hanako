using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

namespace Hanako
{
    public class Ray_Kokkuri_GameEndTrigger : MonoBehaviour
    {

        [SerializeField]
        PlayerCursor cursor;
        
        [SerializeField]
        LevelInfo levelInfo;

        [SerializeField]
        string playTimeParamName = "playTime";

        [SerializeField]
        string scoreParamName = "score";

        [SerializeField]
        List<ColliderProxy> cols = new();

        [SerializeField]
        UnityEvent onEndGame;

        bool gameEnded;
        float playTime = 0;

        private void Awake()
        {
            foreach (var col in cols)
                col.OnEnter += OnEnter;

        }

        private void Update()
        {
            playTime += Time.deltaTime;
        }

        public void StartGame()
        {
            cursor.gameObject.SetActive(false);
        }

        void OnEnter(Collider2D other)
        {
            if (other.CompareTag("Player") && !gameEnded)
            {
                gameEnded = true;
                int score = FindObjectOfType<AlphaChange>().GetScore(); 

                var scoreCanvas = FindObjectOfType<ScoreCanvas>(true);
                scoreCanvas.gameObject.SetActive(true);
                scoreCanvas.Init(levelInfo, new List<ScoreDetail>() 
                {
                    new ScoreDetail(playTimeParamName, (int)playTime),
                    new ScoreDetail(scoreParamName, score)
                });

                cursor.gameObject.SetActive(true);
                onEndGame.Invoke();
            }
        }
    }
}
