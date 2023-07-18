using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    public class HanakoDestinationUI : MonoBehaviour
    {
        public enum PlayerHereMode { Idle, Hide }
        public enum FillMode { Idle, Full }

        [SerializeField]
        Animator fillAnimator;

        int int_mode, flo_speed, boo_show;
        Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            int_mode = Animator.StringToHash(nameof(int_mode));
            flo_speed = Animator.StringToHash(nameof(flo_speed));
            Hide();
        }

        public void Init(ref Action<float> onStartFill, ref Action onShow, ref Action onHide)
        {
            onStartFill += StartFullToIdle;
            onShow += Show;
            onHide += Hide;
        }


        public void Exit(ref Action<float> onStartFill, ref Action onShow, ref Action onHide)
        {
            onStartFill -= StartFullToIdle;
            onShow -= Show;
            onHide -= Hide;
        }

        public void Show()
        {
            animator.SetBool(boo_show, true);
        }

        public void Hide()
        {
            animator.SetBool(boo_show, false);
        }

        public void StartFullToIdle(float duration)
        {
            const float durationToFull = 0.33f;
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                fillAnimator.SetInteger(int_mode, (int)FillMode.Full);
                fillAnimator.SetFloat(flo_speed, 1f/durationToFull);

                yield return new WaitForSeconds(durationToFull);

                fillAnimator.SetInteger(int_mode, (int)FillMode.Idle);
                fillAnimator.SetFloat(flo_speed, 1f/(duration-durationToFull));
            }

        }

        public void HideFill()
        {
        }

        public void ShowPlayerHere()
        {
            //playerHere.SetInteger(int_mode, (int)PlayerHereMode.Idle);
        }

        public void HidePlayerHere()
        {
            //playerHere.SetInteger(int_mode, (int)PlayerHereMode.Hide);
        }


    }
}
