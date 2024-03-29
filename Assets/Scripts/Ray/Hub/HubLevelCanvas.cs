using Encore.Utility;
using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Hub
{
    [RequireComponent(typeof(CanvasGroup), typeof(Animator))]
    public class HubLevelCanvas : MonoBehaviour
    {
        [SerializeField]
        string levelNamePrefix = "Level: ";

        [Header("Components")]
        [SerializeField]
        Image titleLogo;

        [SerializeField]
        TextMeshProUGUI levelNameText;

        [SerializeField]
        TextMeshProUGUI gameSummaryText;

        [SerializeField]
        Transform soulIconParent;

        [SerializeField]
        Animator soulIconPrefab;

        [SerializeField]
        List<TextMeshProUGUI> leaderboardTexts = new();

        [SerializeField]
        Image playBut;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom showAudioSource;

        [SerializeField]
        AudioSourceRandom playButAudioSource;

        [SerializeField]
        string sfxShowName = "sfxShow";

        [SerializeField]
        string sfxHover = "sfxHover";

        [SerializeField]
        string sfxClick = "sfxClick";

        [Header("Extra")]
        [SerializeField]
        PauseCanvas pauseCanvas;

        int int_mode, boo_show;
        bool isLoadingScene = false;
        CanvasGroup canvasGroup;
        Animator animator, playButAnimator;
        Action<LevelInfo> OnLoadScene;
        LevelInfo currentLevelInfo;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            boo_show = Animator.StringToHash(nameof(boo_show));

            playButAnimator = playBut.GetComponent<Animator>();
            playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            playBut.AddEventTriggers(
                onClick: OnClickPlayBut,
                onEnter: OnEnterPlayBut,
                onDown: OnDownPlayBut,
                onExit: OnExitPlayBut
                );

            void OnClickPlayBut()
            {
                if (isLoadingScene) return;
                PlayLevel();
            }

            void OnEnterPlayBut()
            {
                if (isLoadingScene) return;
                playButAudioSource.PlayOneClipFromPack(sfxHover);
                playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Hover);
            }

            void OnDownPlayBut()
            {
                if (isLoadingScene) return;
                playButAudioSource.PlayOneClipFromPack(sfxClick);
                playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);
            }

            void OnExitPlayBut()
            {
                if (isLoadingScene) return;
                playButAudioSource.PlayOneClipFromPack(sfxHover);
                playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            }
        }

        void Start()
        {
            Hide();
        }

        public void Init(Action<LevelInfo> onLoadScene)
        {
            this.OnLoadScene = onLoadScene;
        }

        public void Show(LevelInfo levelInfo)
        {
            currentLevelInfo = levelInfo;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            animator.SetBool(boo_show, true);

            titleLogo.sprite = levelInfo.GameInfo.TitleLogo;
            levelNameText.text = levelNamePrefix + levelInfo.LevelName;
            gameSummaryText.text = levelInfo.GameInfo.GameSummary;

            soulIconParent.DestroyChildren();
            for (int i = 0; i < levelInfo.MaxSoulCount; i++)
            {
                var soulIconAnimator = Instantiate(soulIconPrefab, soulIconParent);
                if (i <= levelInfo.CurrentSoulCount - 1)
                    soulIconAnimator.SetInteger(int_mode, (int)SoulIconState.Alive);
            }

            var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfo != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (levelInfo.Leaderboard.Count > i)
                    {
                        var score = levelInfo.Leaderboard[i].Score;
                        var playerID = levelInfo.Leaderboard[i].PlayerID;
                        var playerData = allGamesInfo.AllGamesInfo.GetPlayerID(playerID);
                        var playerDisplayName = playerData != null ? playerData.DisplayName : "anon";
                        leaderboardTexts[i].text = $"{score} - {playerDisplayName}";
                    }
                    else
                    {
                        leaderboardTexts[i].text = "n/a";
                    }
                }
            }

            showAudioSource.PlayAllClipsFromPack(sfxShowName);

            StartCoroutine(Delay(0.5f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            }


            if (pauseCanvas != null)
            {
                pauseCanvas.SetCanHide(true);
                pauseCanvas.Hide();
            }
        }

        public void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            animator.SetBool(boo_show, false);
            playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);

            if (pauseCanvas != null)
            {
                pauseCanvas.Show();
                pauseCanvas.SetCanHide(false);
            }
        }

        void PlayLevel()
        {
            if (isLoadingScene) return;
            isLoadingScene = true;
            OnLoadScene?.Invoke(currentLevelInfo);
        }
    }
}
