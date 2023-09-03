using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Hub
{
    public class HubLevelCanvas : MonoBehaviour
    {
        enum SoulIconState { Dead, Alive }
        enum PlayButState { Pressed, Idle, Hover }

        [SerializeField]
        LevelScore levelScoreToPreview;

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

        List<Animator> soulIcons = new();
        int int_mode;
        bool isLoadingScene = false;

        void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));

            if (levelScoreToPreview != null)
                Show(levelScoreToPreview);

            var playbutAnimator = playBut.GetComponent<Animator>();
            playbutAnimator.SetInteger(int_mode, (int)PlayButState.Idle);
            playBut.AddEventTriggers(
                onClick: () => { playbutAnimator.SetInteger(int_mode,(int)PlayButState.Pressed); PlayLevel(); },
                onEnter: () => { if (!isLoadingScene) playbutAnimator.SetInteger(int_mode,(int)PlayButState.Hover); },
                onExit: () => { if (!isLoadingScene) playbutAnimator.SetInteger(int_mode,(int)PlayButState.Idle); }
                );
        }

        public void Show(LevelScore levelScore)
        {
            titleLogo.sprite = levelScore.GameInfo.TitleLogo;
            levelNameText.text = levelScore.LevelName;
            soulIconParent.DestroyChildren();
            for (int i = 0; i < levelScore.MaxSoulCount; i++)
            {
                var soulIconAnimator = Instantiate(soulIconPrefab, soulIconParent);
                if (i <= levelScore.CurrentSoulCount - 1)
                    soulIconAnimator.SetInteger(int_mode, (int)SoulIconState.Alive);
                soulIcons.Add(soulIconAnimator);
            }
            playTimeText.text = MathUtility.SecondsToTimeString(levelScore.PlayTime);
        }

        public void Hide()
        {

        }

        void PlayLevel()
        {
            if (isLoadingScene) return;
            isLoadingScene = true;
        }
    }
}
