using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
//using static UnityEditor.Rendering.FilterWindow;

namespace Hanako.Knife
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animator))]
    public class GameOverCanvas : MonoBehaviour
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
        TextMeshProUGUI soulCountText;

        [SerializeField]
        string soulTemplate;

        [Header("Round")]
        [SerializeField]
        TextMeshProUGUI roundCountText;

        [SerializeField]
        string roundTemplate;
        
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

            var levelManager = FindObjectOfType<KnifeLevelManager>();
            if (levelManager != null)
            {
                levelManager.OnGameOver += (isWon)=> { Init(isWon, levelManager); };
            }

        }

        void OnDestroy()
        {
            var levelManager = FindObjectOfType<KnifeLevelManager>();
            if (levelManager != null)
            {
                levelManager.OnGameOver -= (isPlayerDead)=> { Init(isPlayerDead, levelManager); };
            }
        }


        public void Init(bool isPlayerDead, KnifeLevelManager levelManager)
        {
            if (isPlayerDead)
                Won(levelManager);
            else
                Lost(levelManager);
        }

        void Won(KnifeLevelManager levelManager)
        {
            titleText.text = titleTemplate.Replace("{}", levelManager.LevelProperties.LevelName);
            soulCountText.text = soulTemplate.ReplaceFirst("{}", levelManager.SoulCount.ToString());
            soulCountText.text = soulCountText.text.ReplaceFirst("{}", (levelManager.LevelProperties.PiecesPattern.GetLivingPieces().Count).ToString());
            roundCountText.text = roundTemplate.ReplaceFirst("{}", levelManager.RoundCount.ToString());
            roundCountText.text = roundCountText.text.ReplaceFirst("{}", levelManager.LevelProperties.RoundCount.ToString());
            timeText.text = timeTemplate.Replace("{}", MathUtility.SecondsToTimeString(levelManager.GameTime));

            ShowCanvas();

        }

        void Lost(KnifeLevelManager levelManager)
        {
            titleText.text = titleLostTemplate.Replace("{}", levelManager.LevelProperties.LevelName);
            soulCountText.text = soulTemplate.ReplaceFirst("{}", levelManager.SoulCount.ToString());
            soulCountText.text = soulCountText.text.ReplaceFirst("{}", (levelManager.LevelProperties.PiecesPattern.GetLivingPieces().Count).ToString());
            roundCountText.text = roundTemplate.ReplaceFirst("{}", levelManager.RoundCount.ToString());
            roundCountText.text = roundCountText.text.ReplaceFirst("{}", levelManager.LevelProperties.RoundCount.ToString());
            timeText.text = timeTemplate.Replace("{}", MathUtility.SecondsToTimeString(levelManager.GameTime));

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

        void IdleButton(Animator animator) => animator.SetInteger(int_mode, (int) ElementMode.Idle);

        void HoverButton(Animator animator) => animator.SetInteger(int_mode, (int) ElementMode.Hover);

        public void PlayAgain()
        {

        }

        public void GoToHub()
        {

        }
    }
}
