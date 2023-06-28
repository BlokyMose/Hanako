using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Knife
{
    public class PauseCanvas : MonoBehaviour
    {
        public enum ButMode { Hidden = -1, Idle, Hover }

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
        
        [Header("Exit")]
        [SerializeField]
        Image exitBut;
        
        [SerializeField]
        TextMeshProUGUI exitText;

        List<Animator> functionalButs = new();
        int int_mode, tri_click;
        Animator pauseButAnimator, sfxButAnimator, bgmButAnimator, exitButAnimator;

        void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_click = Animator.StringToHash(nameof(tri_click));

            pauseButAnimator = pauseBut.GetComponentInFamily<Animator>();
            sfxButAnimator = sfxBut.GetComponentInFamily<Animator>();
            bgmButAnimator = bgmBut.GetComponentInFamily<Animator>();
            exitButAnimator = exitBut.GetComponentInFamily<Animator>();
            functionalButs = new() { sfxButAnimator, bgmButAnimator, exitButAnimator };

            pauseButAnimator.SetInteger(int_mode, (int)ButMode.Idle);
            
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


            exitBut.AddEventTriggers(
                onEnter: () => { HoverBut(exitButAnimator); },
                onExit: () => { IdleBut(exitButAnimator); },
                onClick: () => { ClickBut(exitButAnimator); ExitGame(); } );

            Hide();
        }

        public void Hide()
        {
            hitAreaHide.gameObject.SetActive(false);
            IdleBut(pauseButAnimator);
            foreach (var but in functionalButs)
                HideBut(but);
        }

        public void Show()
        {
            hitAreaHide.gameObject.SetActive(true);
            HoverBut(pauseButAnimator);

            foreach (var but in functionalButs)
                IdleBut(but);
        }

        public void HideBut(Animator animator) => animator.SetInteger(int_mode, (int)ButMode.Hidden);
        public void HoverBut(Animator animator) => animator.SetInteger(int_mode, (int)ButMode.Hover);
        public void IdleBut(Animator animator) => animator.SetInteger(int_mode, (int)ButMode.Idle);
        public void ClickBut(Animator animator) => animator.SetTrigger(tri_click);

        public void ChangeSFXVolume()
        {

        }


        public void ChangeBGMVolume()
        {

        }

        public void ExitGame()
        {

        }
    }
}
