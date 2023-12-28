using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityUtility;
using Random = UnityEngine.Random;

namespace Hanako
{
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class LeaderboardCanvas : MonoBehaviour
    {
        public class PlayerScore
        {
            PlayerID playerID;
            LeaderboardItemPanel panel;

            public PlayerScore(PlayerID playerID)
            {
                this.playerID = playerID;
            }

            int totalScore;
            Dictionary<string,int> gameScores = new();

            public PlayerID PlayerID { get => playerID; }
            public Dictionary<string,int> GameScores { get => gameScores; }
            public int TotalScore { get => totalScore; }
            public LeaderboardItemPanel Panel { get => panel; set => panel = value; }

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

        [SerializeField]
        TMP_InputField searchBar;

        [SerializeField]
        TextMeshProUGUI playerCountText;

        [SerializeField]
        string playerCountSuffix = " players";

        [Header("Header")]
        [SerializeField]
        List<GameInfo> gameInfos = new();

        [SerializeField]
        Image totalHeaderBut;

        [SerializeField]
        GameObject totalHeaderSelectionIcon;

        [SerializeField]
        Transform gameHeadersParent;

        [SerializeField]
        LeaderboardGameHeader gameHeaderButPrefab;

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;
        List<PlayerScore> playerScores = new();
        List<LeaderboardGameHeader> gameHeaderButs = new();
        PlayerInputHandler playerInputHandler;

        public List<PlayerScore> PlayerScores { get => playerScores; }

        void Awake()
        {
            var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfoManager == null)
                return;
            var allGamesInfo = allGamesInfoManager.AllGamesInfo;

            playerInputHandler = FindObjectOfType<PlayerInputHandler>();

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            playerScores = CreatePlayerScores(allGamesInfo);
            CreateLeaderboard(SortPlayerScores(playerScores));
            CreateGameHeaders(gameInfos);
            totalHeaderBut.AddEventTrigger(()=> { SortPanelsBy(); HideAllSelectionIcons(); totalHeaderSelectionIcon.SetActive(true); });
            closeBut.AddEventTrigger(Hide);
            searchBar.onValueChanged.AddListener(Search);
            playerCountText.text = allGamesInfo.PlayerIDs.Count.ToString() + playerCountSuffix;

            Hide();
        }

        static List<PlayerScore> CreatePlayerScores(AllGamesInfo allGamesInfo)
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
        
        static List<PlayerScore> SortPlayerScores(List<PlayerScore> playerScores, string gameID)
        {
            var playerScoresSorted = new List<PlayerScore>();
            foreach (var player in playerScores)
            {
                bool isInserted = false;
                for (int i = 0; i < playerScoresSorted.Count; i++)
                {
                    if (player.GameScores.TryGetValue(gameID, out var newPlayerScore) &&
                        playerScoresSorted[i].GameScores.TryGetValue(gameID, out var sortedPlayerScore) &&
                        newPlayerScore > sortedPlayerScore)
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
            itemPanelsParent.DestroyChildren();

            int rank = 1;
            foreach (var player in playerScoresSorted)
            {
                var panel = Instantiate(itemPanelPrefab, itemPanelsParent);
                panel.Init(rank, player.PlayerID.DisplayName, player.TotalScore, player.GameScores, gameInfos);
                player.Panel = panel;
                rank++;
            }
        }

        void CreateGameHeaders(List<GameInfo> gameInfos)
        {
            gameHeaderButPrefab.gameObject.SetActive(false);
            gameHeadersParent.DestroyChildren();
            foreach (var gameInfo in gameInfos)
            {
                var gameID = gameInfo.GameID;
                var gameHeaderBut = Instantiate(gameHeaderButPrefab, gameHeadersParent);
                gameHeaderBut.gameObject.SetActive(true);
                gameHeaderBut.Init(gameInfo.TitleIcon, () => OnClick(gameID, gameHeaderBut));
                gameHeaderButs.Add(gameHeaderBut);

            }

            void OnClick(string gameID, LeaderboardGameHeader gameHeaderBut)
            {
                SortPanelsBy(gameID);
                HideAllSelectionIcons();
                gameHeaderBut.Select();
            }
        }

        public void Show()
        {
            animator.SetBool(boo_show, true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (playerInputHandler != null)
                playerInputHandler.DisableMove();
        }

        public void Hide()
        {
            animator.SetBool(boo_show, false);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (playerInputHandler != null)
                playerInputHandler.EnableMove();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameID">if empty, sorted by total score</param>
        void SortPanelsBy(string gameID = "")
        {
            if (string.IsNullOrEmpty(gameID))
                CreateLeaderboard(SortPlayerScores(playerScores));
            else
                CreateLeaderboard(SortPlayerScores(playerScores, gameID));
        }

        void HideAllSelectionIcons()
        {
            foreach (var but in gameHeaderButs)
                but.Unselect();
            totalHeaderSelectionIcon.SetActive(false);
        }

        void Search(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                foreach (var player in playerScores)
                    player.Panel.gameObject.SetActive(true);
            }
            else
            {
                foreach (var player in playerScores)
                    if (!player.PlayerID.DisplayName.Contains(text, StringComparison.InvariantCultureIgnoreCase))
                        player.Panel.gameObject.SetActive(false);
                    else 
                        player.Panel.gameObject.SetActive(true);
            }
        }

        public PlayerScore GetPlayerScore(PlayerID playerID)
        {
            return playerScores.Find(x => x.PlayerID.ID == playerID.ID);
        }
    }
}
