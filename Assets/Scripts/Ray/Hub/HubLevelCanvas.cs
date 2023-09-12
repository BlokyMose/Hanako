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
        public enum SoulIconState { Dead, Alive }
        public enum PlayButState { Pressed, Idle, Hover }

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
        TextMeshProUGUI playTimeText;

        [SerializeField]
        Image playBut;

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
            playButAnimator.SetInteger(int_mode, (int)PlayButState.Idle);
            playBut.AddEventTriggers(
                onClick: () => { if (!isLoadingScene) PlayLevel(); },
                onEnter: () => { if (!isLoadingScene) playButAnimator.SetInteger(int_mode,(int)PlayButState.Hover); },
                onDown: () => { if (!isLoadingScene) playButAnimator.SetInteger(int_mode,(int)PlayButState.Pressed); },
                onExit: () => { if (!isLoadingScene) playButAnimator.SetInteger(int_mode,(int)PlayButState.Idle); }
                );
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

            playTimeText.text = levelInfo.PlayTime > 0f 
                ? MathUtility.SecondsToTimeString(levelInfo.PlayTime)
                : "";


            StartCoroutine(Delay(0.5f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                playButAnimator.SetInteger(int_mode, (int)PlayButState.Idle);
            }
        }

        public void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            animator.SetBool(boo_show, false);
            playButAnimator.SetInteger(int_mode, (int)PlayButState.Pressed);
        }

        void PlayLevel()
        {
            if (isLoadingScene) return;
            isLoadingScene = true;
            OnLoadScene?.Invoke(currentLevelInfo);
        }
    }
}
