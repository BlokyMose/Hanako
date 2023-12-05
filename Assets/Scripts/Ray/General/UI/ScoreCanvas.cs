using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako
{
    [RequireComponent(typeof(Animator))]
    public class ScoreCanvas : MonoBehaviour
    {
        enum ThresholdTextAnimation { Idle, Active}

        [System.Serializable]
        public class ScoreTower
        {
            [SerializeField]
            Animator soulIcon;

            [SerializeField]
            Image fill;

            [SerializeField]
            TextMeshProUGUI threshold;

            [SerializeField]
            Animator thresholdAnimator;

            public Animator SoulIcon { get => soulIcon; }
            public Image Fill { get => fill; }
            public TextMeshProUGUI Threshold { get => threshold; }
            public Animator ThresholdAnimator { get => thresholdAnimator; }
        }

        [SerializeField]
        LevelInfo levelInfo;

        [SerializeField]
        float ruleAnimationDuration = 1f;

        [Header("Components")]
        [SerializeField]
        TextMeshProUGUI scoreText;

        [SerializeField]
        Transform scoreDetailsParent;

        [SerializeField]
        TextMeshProUGUI levelNameText;

        [SerializeField]
        ScoreDetailText scoreDetailTextPrefab;

        [SerializeField]
        List<ScoreTower> scoreTowers = new();

        [SerializeField]
        List<GameObject> hiScoreRank = new();

        [SerializeField]
        List<TextMeshProUGUI> leaderboardTexts = new();


        [Header("Buttons")]
        [SerializeField]
        Image againBut;  
        
        [SerializeField]
        Image hubBut;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom audioSource;
        
        [SerializeField]
        AudioSourceRandom audioSourceButton;

        [SerializeField]
        string sfxRiser = "sfxRiser";

        [SerializeField]
        string sfxConfirmName = "sfxConfirm";

        [SerializeField]
        string sfxHoverName = "sfxHover";

        [SerializeField]
        string sfxClickName = "sfxClick";

        int int_mode, tri_confirm, boo_overScore, boo_hiScore, boo_leaderboard;
        Animator animator, againButAnimator, hubButAnimator;

        void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_confirm = Animator.StringToHash(nameof(tri_confirm));
            boo_overScore = Animator.StringToHash(nameof(boo_overScore));
            boo_hiScore = Animator.StringToHash(nameof(boo_hiScore));
            boo_leaderboard = Animator.StringToHash(nameof(boo_leaderboard));

            againButAnimator = againBut.GetComponent<Animator>();
            againBut.AddEventTriggers(
                onEnter:OnEnterAgainBut,
                onExit:OnExitAgainBut,
                onDown:OnDownAgainBut,
                onClick:OnClickAgainBut
                );

            hubButAnimator = hubBut.GetComponent<Animator>();
            hubBut.AddEventTriggers(
                onEnter: OnEnterHubBut,
                onExit: OnExitHubBut,
                onDown: OnDownHubBut,
                onClick: OnClickHubBut
                );

            foreach (var text in hiScoreRank)
                text.SetActive(false);

            #region [Methods: Again But]


            void OnEnterAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Hover);
                audioSourceButton.PlayOneClipFromPack(sfxHoverName);
            }

            void OnExitAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                audioSourceButton.PlayOneClipFromPack(sfxHoverName);
            }

            void OnDownAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);
                audioSourceButton.PlayOneClipFromPack(sfxClickName);
            }

            void OnClickAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                var sceneLoading = FindObjectOfType<SceneLoadingManager>();
                var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
                if (sceneLoading != null && allGamesInfo != null)
                    sceneLoading.LoadScene(allGamesInfo.AllGamesInfo.CurrentLevel);
            }

            #endregion

            #region [Methods: Hub But]


            void OnEnterHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Hover);
                audioSourceButton.PlayOneClipFromPack(sfxHoverName);
            }

            void OnExitHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                audioSourceButton.PlayOneClipFromPack(sfxHoverName);
            }

            void OnDownHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);
                audioSourceButton.PlayOneClipFromPack(sfxClickName);
            }

            void OnClickHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                var sceneLoading = FindObjectOfType<SceneLoadingManager>();
                var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
                if (sceneLoading != null && allGamesInfo != null)
                    sceneLoading.LoadScene(allGamesInfo.AllGamesInfo.HubLevelInfo);
            }

            #endregion
        }

        private void Start()
        {
            StartCoroutine(DelayButAnimation(1f));
            IEnumerator DelayButAnimation(float delay)
            {
                yield return new WaitForSeconds(delay);
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            }
        }

        public void Init(LevelInfo levelInfo, List<ScoreDetail> scoreDetails)
        {
            var hiScore = -1;
            this.levelInfo = levelInfo;
            scoreDetailsParent.DestroyChildren();
            UpdateDescriptions(levelInfo);
            CalculateScore(levelInfo, scoreDetails, out var score, out var soulCount, out var playTime);
            hiScore = CheckHiScore(levelInfo, score);
            SaveNewScore(levelInfo, score, soulCount, playTime);
            UpdateLeaderboard(levelInfo);
            StartCoroutine(AnimatingUI(hiScore));

            #region [Methods]

            void UpdateDescriptions(LevelInfo levelInfo)
            {
                levelNameText.text = levelInfo.GameInfo.GameDisplayName + " - " + levelInfo.LevelName;
                scoreText.text = levelInfo.CurrentScore.ToString();
                for (int i = 0; i < scoreTowers.Count; i++)
                    if (i < levelInfo.ScoreThresholds.Count)
                        scoreTowers[i].Threshold.text = levelInfo.ScoreThresholds[i].ToString();
            }

            void UpdateLeaderboard(LevelInfo levelInfo)
            {
                var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
                if (allGamesInfo != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (levelInfo.Leaderboard.Count > i)
                        {
                            var score = levelInfo.Leaderboard[i].Score;
                            var playerID = levelInfo.Leaderboard[i].PlayerID;
                            var playerData = allGamesInfo.AllGamesInfo.GetPlayerData(playerID);
                            var playerDisplayName = playerData != null ? playerData.DisplayName : "anon";
                            leaderboardTexts[i].text = $"{score} - {playerDisplayName}";
                        }
                        else
                        {
                            leaderboardTexts[i].text = "n/a";
                        }
                    }
                }
            }

            void CalculateScore(LevelInfo levelInfo, List<ScoreDetail> scoreDetails, out int score, out int soulCount, out int playTime)
            {
                score = 0;
                playTime = GetValue(scoreDetails, "playTime");
                soulCount = 0;

                for (int i = 0; i < levelInfo.ScoreRules.Rules.Count; i++)
                {
                    var rule = levelInfo.ScoreRules.Rules[i];
                    var toReceiveScore = GetValue(scoreDetails, rule.ParamName) * rule.ScoreMultiplier;
                    score += toReceiveScore;
                }

                foreach (var threshold in levelInfo.ScoreThresholds)
                    soulCount += score >= threshold ? 1 : 0;

            }

            IEnumerator AnimatingUI(int hiScore)
            {
                yield return new WaitForSeconds(1f);

                var score = 0f;
                var playTime = GetValue(scoreDetails, "playTime");
                var filledTowerCount = 0;
                if (levelInfo.ScoreRules.Rules.Count < 3)
                    audioSource.PlayOneClipFromPack(sfxRiser);

                for (int i = 0; i < levelInfo.ScoreRules.Rules.Count; i++)
                {
                    if (levelInfo.ScoreRules.Rules.Count - i == 3)
                        audioSource.PlayOneClipFromPack(sfxRiser);

                    var rule = levelInfo.ScoreRules.Rules[i];
                    var toReceiveScore = GetValue(scoreDetails, rule.ParamName) * rule.ScoreMultiplier;
                    MakeScoreDetailText(scoreDetailTextPrefab, scoreDetailsParent, levelInfo.ScoreRules, rule, scoreDetails);

                    yield return StartCoroutine(AnimatingScoreUI(score, toReceiveScore));

                    score += toReceiveScore;
                    scoreText.text = ((int)score).ToString();
                    filledTowerCount = FillTowers(levelInfo, score);
                    animator.SetTrigger(tri_confirm);
                    audioSource.PlayOneClipFromPack(sfxConfirmName);
                }

                if (score >= levelInfo.ScoreThresholds.GetLast())
                    animator.SetBool(boo_overScore, true);

                if (hiScore > -1)
                {
                    animator.SetBool(boo_hiScore, true);
                    if (hiScore > 0)
                        hiScoreRank.GetAt(hiScore-1, 0).SetActive(true);
                }

                yield return new WaitForSeconds(1f);
                animator.SetBool(boo_leaderboard, true);
            }

            int CheckHiScore(LevelInfo levelInfo, int score)
            {
                var hiScore = -1;
                var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
                if (allGamesInfoManager == null) return hiScore;
                var allGamesInfo = allGamesInfoManager.AllGamesInfo;    

                if (allGamesInfo.CurrentGameMode == GameMode.Solo && levelInfo.CurrentScore < score)
                {
                    hiScore = 0;
                }
                else if (allGamesInfo.CurrentGameMode == GameMode.Arcade)
                {
                    var hiScoredPlayerCount = levelInfo.Leaderboard.Count > 3 ? 3 : levelInfo.Leaderboard.Count;
                    for (int i = 0; i < hiScoredPlayerCount; i++)
                        if (levelInfo.Leaderboard[i].Score < score)
                            return i + 1;
                }
                return hiScore;
            }

            void SaveNewScore(LevelInfo levelInfo, int score, int soulCount, int playTime)
            {
                var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
                if (allGamesInfoManager == null) return;
                var allGamesInfo = allGamesInfoManager.AllGamesInfo;

                if (allGamesInfo.CurrentGameMode == GameMode.Solo && levelInfo.CurrentScore < score)
                {
                    var newScore = score;
                    var newSoulCount = levelInfo.CurrentSoulCount < soulCount ? soulCount : levelInfo.CurrentSoulCount;
                    var newPlayTime = playTime;
                    levelInfo.SetRuntimeData(new(
                        newScore,
                        newSoulCount,
                        newPlayTime,
                        true
                        ));
                }

                else if (allGamesInfo.CurrentGameMode == GameMode.Arcade)
                {
                    var recurringPlayer = levelInfo.Leaderboard.Find(x => x.PlayerID == allGamesInfo.CurrentPlayerID.ID);
                    if (recurringPlayer != null)
                    {
                        if (recurringPlayer.Score >= score)
                            return;
                        levelInfo.Leaderboard.Remove(recurringPlayer);
                    }

                    for (int i = 0; i < levelInfo.Leaderboard.Count; i++)
                    {
                        if (levelInfo.Leaderboard[i].Score < score)
                        {
                            levelInfo.Leaderboard.Insert(i, new(score, allGamesInfo.CurrentPlayerID.ID));
                            break;
                        }
                    }
                }
            }

            int GetValue(List<ScoreDetail> scoreDetails, string ruleName)
            {
                var valueDetail = scoreDetails.Find(x => x.ParamName == ruleName);
                if (valueDetail != null)
                    return valueDetail.Value;
                else
                {
                    Debug.LogWarning("Cannot find rule with paramName: " + ruleName);
                    return 0;
                }
            }

            void MakeScoreDetailText(ScoreDetailText scoreDetailTextPrefab, Transform scoreDetailsParent, ScoreRules scoreRule, ScoreRules.Rule rule, List<ScoreDetail> scoreDetails)
            {
                var detailText = Instantiate(scoreDetailTextPrefab, scoreDetailsParent);
                var detailTextContent = rule.DisplayFormat;
                detailTextContent = detailTextContent.Replace(
                    scoreRule.OpenToken + "displayName" + scoreRule.CloseToken,
                    rule.DisplayName);

                detailTextContent = detailTextContent.Replace(
                    scoreRule.OpenToken + "scoreMultiplier" + scoreRule.CloseToken,
                    rule.ScoreMultiplier.ToString());

                var valueDetail = scoreDetails.Find(x => x.ParamName == rule.ParamName);
                var value = valueDetail != null ? valueDetail.Value : 0;
                detailTextContent = detailTextContent.Replace(
                    scoreRule.OpenToken + "value" + scoreRule.CloseToken,
                    value.ToString());

                detailText.Init(detailTextContent, rule.FontColor);
            }

            int FillTowers(LevelInfo levelInfo, float score)
            {
                var scoreLeft = score;
                int thresholdIndex = 0;
                int filledTowerCount = 0;
                foreach (var threshold in levelInfo.ScoreThresholds)
                {
                    var tower = scoreTowers[thresholdIndex];
                    var previousThreshold = thresholdIndex > 0 ? levelInfo.ScoreThresholds[thresholdIndex - 1] : 0;
                    thresholdIndex++;
                    var relativeThreshold = threshold - previousThreshold;
                    scoreLeft -= relativeThreshold;

                    if (scoreLeft < 0)
                    {
                        tower.Fill.fillAmount = 1 - (-scoreLeft) / relativeThreshold;
                        tower.SoulIcon.SetInteger(int_mode, (int)SoulIconState.Dead);
                        tower.ThresholdAnimator.SetInteger(int_mode, (int)ThresholdTextAnimation.Idle);
                        break;
                    }
                    else
                    {
                        if (thresholdIndex > scoreTowers.Count) break;
                        tower.Fill.fillAmount = 1f;
                        tower.SoulIcon.SetInteger(int_mode, (int)SoulIconState.Alive);
                        tower.ThresholdAnimator.SetInteger(int_mode, (int)ThresholdTextAnimation.Active);
                        filledTowerCount++;
                    }
                }

                for (int i = thresholdIndex; i < scoreTowers.Count; i++)
                {
                    var tower = scoreTowers[i];
                    tower.Fill.fillAmount = 0;
                    tower.SoulIcon.SetInteger(int_mode, (int)SoulIconState.Dead);
                    tower.ThresholdAnimator.SetInteger(int_mode, (int)ThresholdTextAnimation.Idle);
                }

                return filledTowerCount;
            }

            IEnumerator AnimatingScoreUI(float previousTotalScore, float toReceiveScore)
            {
                var time = 0f;
                var curve = AnimationCurve.Linear(0, 0, ruleAnimationDuration, toReceiveScore);
                var animatedScore = previousTotalScore;
                while (time < ruleAnimationDuration)
                {
                    animatedScore = previousTotalScore + curve.Evaluate(time);
                    scoreText.text = ((int)animatedScore).ToString();
                    FillTowers(levelInfo, animatedScore);
                    time += Time.deltaTime;
                    yield return null;
                }
            }

            #endregion

        }

    }
}
