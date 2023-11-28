using Encore.Utility;
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
        Transform soulIconParent;

        [SerializeField]
        Animator soulIconPrefab;

        [SerializeField]
        TextMeshProUGUI scoreText;

        [SerializeField]
        TextMeshProUGUI authorText;

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

            Hide();

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

            soulIconParent.DestroyChildren();
            for (int i = 0; i < levelInfo.MaxSoulCount; i++)
            {
                var soulIconAnimator = Instantiate(soulIconPrefab, soulIconParent);
                if (i <= levelInfo.CurrentSoulCount - 1)
                    soulIconAnimator.SetInteger(int_mode, (int)SoulIconState.Alive);
            }

            scoreText.text = levelInfo.CurrentScore.ToString();

            showAudioSource.PlayAllClipsFromPack(sfxShowName);

            StartCoroutine(Delay(0.5f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            }
        }

        public void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            animator.SetBool(boo_show, false);
            playButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);
        }

        void PlayLevel()
        {
            if (isLoadingScene) return;
            isLoadingScene = true;
            OnLoadScene?.Invoke(currentLevelInfo);
        }
    }
}
