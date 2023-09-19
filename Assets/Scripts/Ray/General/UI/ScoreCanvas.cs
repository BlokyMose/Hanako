using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako
{
    public class ScoreCanvas : MonoBehaviour
    {
        [System.Serializable]
        public class ScoreTower
        {
            public Animator soulIcon;
            public Image fill;
            public TextMeshProUGUI threshold;

            public ScoreTower(Animator soulIcon, Image fill, TextMeshProUGUI threshold)
            {
                this.soulIcon = soulIcon;
                this.fill = fill;
                this.threshold = threshold;
            }
        }

        [SerializeField]
        LevelInfo levelInfo;

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

        [Header("Buttons")]
        [SerializeField]
        Image againBut;

        [SerializeField]
        Image hubBut;

        int int_mode;

        void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));

            var againButAnimator = againBut.GetComponent<Animator>();
            againBut.AddEventTriggers(
                onEnter:OnEnterAgainBut,
                onExit:OnExitAgainBut,
                onDown:OnDownAgainBut,
                onClick:OnClickAgainBut
                );

            var hubButAnimator = hubBut.GetComponent<Animator>();
            hubBut.AddEventTriggers(
                onEnter: OnEnterHubBut,
                onExit: OnExitHubBut,
                onDown: OnDownHubBut,
                onClick: OnClickHubBut
                );


            StartCoroutine(Delay(1f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            }

            Init(levelInfo, new List<ScoreDetail>()
            {
                new ("kill",1),
                new ("multiKill",1),
                new ("playTime",10)
            });


            #region [Methods: Again But]


            void OnEnterAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Hover);
            }

            void OnExitAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);

            }

            void OnDownAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);

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
            }

            void OnExitHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);

            }

            void OnDownHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);

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

        public void Init(LevelInfo levelInfo, List<ScoreDetail> scoreDetails)
        {
            this.levelInfo = levelInfo;
            scoreDetailsParent.DestroyChildren();

            levelNameText.text = levelInfo.GameInfo.GameName + " - " + levelInfo.LevelName;
            scoreText.text = levelInfo.Score.ToString();

            for (int i = 0; i < scoreTowers.Count; i++)
            {
                var tower = scoreTowers[i];
                if (i < levelInfo.ScoreThresholds.Count)
                {
                    tower.threshold.text = levelInfo.ScoreThresholds[i].ToString();
                }
            }

            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(1f);

                var score = 0f;
                foreach (var rule in levelInfo.ScoreRule.Rules)
                {
                    var detailText = Instantiate(scoreDetailTextPrefab, scoreDetailsParent);

                    var detailTextContent = rule.DisplayFormat;
                    detailTextContent = detailTextContent.Replace(
                        levelInfo.ScoreRule.OpenToken + "displayName" + levelInfo.ScoreRule.CloseToken,
                        rule.DisplayName);

                    detailTextContent = detailTextContent.Replace(
                        levelInfo.ScoreRule.OpenToken + "scoreMultiplier" + levelInfo.ScoreRule.CloseToken,
                        rule.ScoreMultiplier.ToString());

                    var valueDetail = scoreDetails.Find(x => x.ParamName == rule.ParamName);
                    var value = valueDetail != null ? valueDetail.Value : 0;
                    detailTextContent = detailTextContent.Replace(
                        levelInfo.ScoreRule.OpenToken + "value" + levelInfo.ScoreRule.CloseToken,
                        value.ToString());

                    detailText.Init(detailTextContent);

                    var previousScore = score;
                    var receivedScore = value * rule.ScoreMultiplier;
                    var targetScore = previousScore + receivedScore;
                    var isTargetScorePositive = score < targetScore;
                    var increment = isTargetScorePositive ? 1 : -1;

                    while ( (isTargetScorePositive && score < targetScore) ||
                            (!isTargetScorePositive && score > targetScore))
                    {
                        score+=increment;
                        scoreText.text = ((int)score).ToString();
                        yield return null;
                    }

                }

            }



            //tower.soulIcon.SetInteger(int_mode, (int)SoulIconState.Alive);
        }
    }
}
