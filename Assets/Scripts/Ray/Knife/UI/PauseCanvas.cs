using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Knife
{
    public class PauseCanvas : MonoBehaviour
    {
        public enum ButMode { Hidden = -1, Idle, Hover }

        [SerializeField]
        AllGamesInfo allGamesInfo;

        [Header("Tutorial")]
        [SerializeField]
        TutorialCanvas tutorialCanvasPrefab;

        [SerializeField]
        float delayAutoShowTutorial = 2.5f;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom audioSource;

        [SerializeField]
        string hoverSFXName = "sfxHover";

        [SerializeField]
        string clickSFXName = "sfxClick";
        
        [SerializeField]
        string openSFXName = "sfxOpen";


        [Header("Pause")]
        [SerializeField]
        Image pauseBut;

        [SerializeField]
        Image hitAreaHide;

        [Header("SFX")]
        [SerializeField]
        Image sfxBut;

        [SerializeField]
        TextMeshProUGUI sfxVolumeText;

        [Header("BGM")]
        [SerializeField]
        Image bgmBut;

        [SerializeField]
        TextMeshProUGUI bgmVolumeText;

        [Header("Tutorial")]
        [SerializeField]
        Image tutorialBut;

        [SerializeField]
        TextMeshProUGUI tutorialText;
        
        [Header("Exit")]
        [SerializeField]
        Image exitBut;

        [SerializeField]
        TextMeshProUGUI exitText;

        [SerializeField]
        LevelInfo hubLevelInfo;

        List<Animator> functionalButs = new();
        int int_mode, tri_click;
        Animator pauseButAnimator, sfxButAnimator, bgmButAnimator, tutorialButAnimator, exitButAnimator;
        bool isInPause = false;

        void Awake()
        {
            if (allGamesInfo == null)
            {
                var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
                if (allGamesInfoManager != null) allGamesInfo = allGamesInfoManager.AllGamesInfo;
            }

            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_click = Animator.StringToHash(nameof(tri_click));

            pauseButAnimator = pauseBut.GetComponentInFamily<Animator>();
            sfxButAnimator = sfxBut.GetComponentInFamily<Animator>();
            bgmButAnimator = bgmBut.GetComponentInFamily<Animator>();
            tutorialButAnimator = tutorialBut.GetComponentInFamily<Animator>();
            exitButAnimator = exitBut.GetComponentInFamily<Animator>();
            functionalButs = new() { sfxButAnimator, bgmButAnimator, tutorialButAnimator, exitButAnimator };

            pauseBut.AddEventTrigger(Show, EventTriggerType.PointerEnter);
            hitAreaHide.AddEventTrigger(Hide, EventTriggerType.PointerEnter);

            sfxBut.AddEventTriggers(
                onEnter: () => { HoverBut(sfxButAnimator); },
                onExit: () => { IdleBut(sfxButAnimator); },
                onClick: () => { ClickBut(sfxButAnimator); ChangeSFXVolume(); } );

            bgmBut.AddEventTriggers(
                onEnter: () => { HoverBut(bgmButAnimator); },
                onExit: () => { IdleBut(bgmButAnimator); },
                onClick: () => { ClickBut(bgmButAnimator); ChangeBGMVolume(); } );

            tutorialBut.AddEventTriggers(
                onEnter: () => { HoverBut(tutorialButAnimator); },
                onExit: () => { IdleBut(tutorialButAnimator); },
                onClick: () => { ClickBut(tutorialButAnimator); ShowTutorialCanvas(); } );

            exitBut.AddEventTriggers(
                onEnter: () => { HoverBut(exitButAnimator); },
                onExit: () => { IdleBut(exitButAnimator); },
                onClick: () => { ClickBut(exitButAnimator); ExitGame(); } );

            hitAreaHide.gameObject.SetActive(false);
            pauseButAnimator.SetInteger(int_mode, (int)ButMode.Idle);
        }

        void Start()
        {
            UpdateBGMVolumeText();
            UpdateSFXVolumeText();

            StartCoroutine(Delay(delayAutoShowTutorial));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                if (allGamesInfo != null)
                {
                    var currentLevel = allGamesInfo.CurrentLevel;
                    if (!currentLevel.HasShownTutorial && currentLevel.TutorialPreview.IsAutoShow)
                        ShowTutorialCanvas();
                }
            }
        }

        public void Hide()
        {
            if (!isInPause) return;

            hitAreaHide.gameObject.SetActive(false);
            IdleBut(pauseButAnimator);
            foreach (var but in functionalButs)
                HideBut(but);
            isInPause = false;
            audioSource.PlayOneClipFromPack(openSFXName);
        }

        public void Show()
        {
            if (isInPause) return;
            isInPause = true;
            hitAreaHide.gameObject.SetActive(true);
            HoverBut(pauseButAnimator);
            foreach (var but in functionalButs)
                IdleBut(but);
            audioSource.PlayOneClipFromPack(openSFXName);
        }

        public void HideBut(Animator animator)
        {
            if (isInPause || animator == pauseButAnimator)
                animator.SetInteger(int_mode, (int)ButMode.Hidden);
        }

        public void HoverBut(Animator animator)
        {
            if (isInPause || animator == pauseButAnimator)
                animator.SetInteger(int_mode, (int)ButMode.Hover);
            audioSource.PlayOneClipFromPack(hoverSFXName);
        }

        public void IdleBut(Animator animator)
        {
            if (isInPause || animator == pauseButAnimator)
                animator.SetInteger(int_mode, (int)ButMode.Idle);
        }

        public void ClickBut(Animator animator)
        {
            if (isInPause || animator == pauseButAnimator)
                animator.SetTrigger(tri_click);
            audioSource.PlayOneClipFromPack(clickSFXName);
        }

        public void ChangeSFXVolume()
        {
            if (allGamesInfo == null) return;

            allGamesInfo.SpinSFXVolume();
            UpdateSFXVolumeText();
        }

        public void ChangeBGMVolume()
        {
            if (allGamesInfo == null) return;

            allGamesInfo.SpinBGMVolume();
            allGamesInfo.SpinAmbienceVolume();
            UpdateBGMVolumeText();

        }

        void UpdateBGMVolumeText()
        {
            if (allGamesInfo == null) return;

            switch (allGamesInfo.BGMVolume)
            {
                case AllGamesInfo.AudioVolume.Mute:
                    bgmVolumeText.text = "0%";
                    break;
                case AllGamesInfo.AudioVolume.Low:
                    bgmVolumeText.text = "25%";
                    break;
                case AllGamesInfo.AudioVolume.Mid:
                    bgmVolumeText.text = "50%";
                    break;
                case AllGamesInfo.AudioVolume.High:
                    bgmVolumeText.text = "75%";
                    break;
                case AllGamesInfo.AudioVolume.Full:
                    bgmVolumeText.text = "100%";
                    break;
            }
        }        
        
        void UpdateSFXVolumeText()
        {
            if (allGamesInfo == null) return;

            switch (allGamesInfo.SFXVolume)
            {
                case AllGamesInfo.AudioVolume.Mute:
                    sfxVolumeText.text = "0%";
                    break;
                case AllGamesInfo.AudioVolume.Low:
                    sfxVolumeText.text = "25%";
                    break;
                case AllGamesInfo.AudioVolume.Mid:
                    sfxVolumeText.text = "50%";
                    break;
                case AllGamesInfo.AudioVolume.High:
                    sfxVolumeText.text = "75%";
                    break;
                case AllGamesInfo.AudioVolume.Full:
                    sfxVolumeText.text = "100%";
                    break;
            }
        }

        public void ShowTutorialCanvas()
        {
            if (allGamesInfo == null) return;

            var currentLevel = allGamesInfo.CurrentLevel;
            currentLevel.SetRuntimeData(new(currentLevel.CurrentSoulCount, currentLevel.CurrentScore, currentLevel.PlayTime, true));
            var tutorialCanvas = Instantiate(tutorialCanvasPrefab);
            tutorialCanvas.Init(currentLevel.GameInfo.TutorialInfo, currentLevel.TutorialPreview);
        }

        public void ExitGame()
        {
            var sceneLoadingManager = FindObjectOfType<SceneLoadingManager>();
            if (sceneLoadingManager != null)
                sceneLoadingManager.LoadScene(hubLevelInfo);
        }
    }
}
