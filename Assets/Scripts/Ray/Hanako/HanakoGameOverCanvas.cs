using Encore.Utility;
using Hanako.Knife;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoGameOverCanvas : MonoBehaviour
    {
        public enum ElementMode { Hidden = -1, Idle, Hover }
        public enum CanvasMode { Hidden = -1, Idle }


        [SerializeField]
        List<Animator> uiElementAnimators = new();

        [SerializeField]
        float interval = 0.33f;

        [Header("Title")]
        [SerializeField]
        TextMeshProUGUI titleText;

        [SerializeField]
        string titleTemplate;

        [SerializeField]
        string titleLostTemplate;

        [Header("Soul")]
        [SerializeField]
        TextMeshProUGUI killCountText;

        [SerializeField]
        string killTemplate;

        [Header("Time")]
        [SerializeField]
        TextMeshProUGUI timeText;

        [SerializeField]
        string timeTemplate;

        [Header("Buttons")]
        [SerializeField]
        Image playAgainBut;

        [SerializeField]
        Image goToHubBut;

        Animator animator, playAgainAnimator, goToHubAnimator;
        int int_mode, boo_isShown;
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            animator.SetInteger(int_mode, (int)CanvasMode.Hidden);
            boo_isShown = Animator.StringToHash(nameof(boo_isShown));

            playAgainAnimator = playAgainBut.GetComponent<Animator>();
            goToHubAnimator = goToHubBut.GetComponent<Animator>();

            playAgainBut.AddEventTriggers(
                onClick: PlayAgain,
                onEnter: () => { HoverButton(playAgainAnimator); },
                onExit: () => { IdleButton(playAgainAnimator); }
                );

            goToHubBut.AddEventTriggers(
                onClick: GoToHub,
                onEnter: () => { HoverButton(goToHubAnimator); },
                onExit: () => { IdleButton(goToHubAnimator); }
                );
        }

        public void Init(bool isPlayerDead, HanakoLevel level, int killCount, int enemyCount, float gameTime)
        {
            if (isPlayerDead)
                Won(level, killCount, enemyCount, gameTime);
            else
                Lost(level, killCount, enemyCount, gameTime);
        }

        void Won(HanakoLevel level, int killCount, int enemyCount, float gameTime)
        {
            titleText.text = titleTemplate.Replace("{}", level.LevelName);
            killCountText.text = killTemplate.ReplaceFirst("{}", killCount.ToString());
            killCountText.text = killCountText.text.ReplaceFirst("{}", enemyCount.ToString());
            timeText.text = timeTemplate.Replace("{}", MathUtility.SecondsToTimeString(gameTime));

            ShowCanvas();

        }

        void Lost(HanakoLevel level, int killCount, int enemyCount, float gameTime)
        {
            titleText.text = titleLostTemplate.Replace("{}", level.LevelName);
            killCountText.text = killTemplate.ReplaceFirst("{}", killCount.ToString());
            killCountText.text = killCountText.text.ReplaceFirst("{}", enemyCount.ToString());
            timeText.text = timeTemplate.Replace("{}", MathUtility.SecondsToTimeString(gameTime));

            ShowCanvas();
        }

        void ShowCanvas()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            animator.SetInteger(int_mode, (int)CanvasMode.Idle);
            StartCoroutine(ShowingElements());
        }

        IEnumerator ShowingElements()
        {
            foreach (var element in uiElementAnimators)
            {
                element.SetBool(boo_isShown, true);
                yield return new WaitForSeconds(interval);
            }
        }

        void IdleButton(Animator animator) => animator.SetInteger(int_mode, (int)ElementMode.Idle);

        void HoverButton(Animator animator) => animator.SetInteger(int_mode, (int)ElementMode.Hover);

        public void PlayAgain()
        {

        }

        public void GoToHub()
        {
            var sceneLoadingManager = FindObjectOfType<SceneLoadingManager>();
            var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
            if (sceneLoadingManager != null && allGamesInfoManager != null)
            {
                sceneLoadingManager.LoadScene(allGamesInfoManager.AllGamesInfo.HubLevelInfo);
            }
        }
    }
}
