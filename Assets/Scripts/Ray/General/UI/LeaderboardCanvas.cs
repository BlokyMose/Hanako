using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityUtility;

namespace Hanako
{
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class LeaderboardCanvas : MonoBehaviour
    {
        class PlayerScore
        {
            PlayerID playerID;

            public PlayerScore(PlayerID playerID)
            {
                this.playerID = playerID;
            }

            int totalScore;
            Dictionary<string,int> gameScores = new();

            public PlayerID PlayerID { get => playerID; }
            public Dictionary<string,int> GameScores { get => gameScores; }
            public int TotalScore { get => totalScore; }

            public void AddGameScore(string gameID, int score)
            {
                if (gameScores.ContainsKey(gameID))
                    gameScores[gameID] += score;
                else
                    gameScores.Add(gameID, score);

                totalScore += score;
            }
        }

        [SerializeField]
        LeaderboardItemPanel itemPanelPrefab;

        [SerializeField]
        Transform itemPanelsParent;

        [SerializeField]
        Image closeBut;

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            itemPanelsParent.DestroyChildren();

            var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfoManager == null)
                return;

            var allGamesInfo = allGamesInfoManager.AllGamesInfo;
            var playerScores = CreatePlayerScore(allGamesInfo);
            var playerScoresSorted = SortPlayerScores(playerScores);
            CreateLeaderboard(playerScoresSorted);

            closeBut.AddEventTrigger(Hide);

            Hide();

            static List<PlayerScore> CreatePlayerScore(AllGamesInfo allGamesInfo)
            {
                var playerScores = new List<PlayerScore>();

                foreach (var player in allGamesInfo.PlayerIDs)
                {
                    var playerScore = new PlayerScore(player);
                    foreach (var levelInfo in allGamesInfo.LevelInfos)
                    {
                        var leaderboardItem = levelInfo.Leaderboard.Find(x => x.PlayerID == player.ID);
                        if (leaderboardItem != null)
                            playerScore.AddGameScore(levelInfo.GameInfo.GameID, leaderboardItem.Score);
                        else
                            playerScore.AddGameScore(levelInfo.GameInfo.GameID, 0);
                    }

                    playerScores.Add(playerScore);
                }
                return playerScores;
            }
            
            static List<PlayerScore> SortPlayerScores(List<PlayerScore> playerScores)
            {
                var playerScoresSorted = new List<PlayerScore>();
                foreach (var player in playerScores)
                {
                    bool isInserted = false;
                    for (int i = 0; i < playerScoresSorted.Count; i++)
                    {
                        if (player.TotalScore > playerScoresSorted[i].TotalScore)
                        {
                            playerScoresSorted.Insert(i, player);
                            isInserted = true;
                            break;
                        }
                    }

                    if (!isInserted)
                        playerScoresSorted.Add(player);
                }
                return playerScoresSorted;
            }

            void CreateLeaderboard(List<PlayerScore> playerScoresSorted)
            {
                int rank = 1;
                foreach (var player in playerScoresSorted)
                {
                    var panel = Instantiate(itemPanelPrefab, itemPanelsParent);
                    panel.Init(rank, player.PlayerID.DisplayName, player.TotalScore, player.GameScores);
                    rank++;
                }
            }
        }

        public void Show()
        {
            animator.SetBool(boo_show, true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            animator.SetBool(boo_show, false);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
