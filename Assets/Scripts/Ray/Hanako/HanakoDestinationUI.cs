using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    public class HanakoDestinationUI : MonoBehaviour
    {
        public enum PlayerHereMode { Idle, Hide }
        public enum FillMode { Idle, Full }

        [SerializeField]
        Animator fillAnimator;

        bool hasPreAnimation; // example FromEmptyToFull animation will have FullToEmpty animation first for some seconds
        const float preAnimationDuration = 0.33f;
        int int_mode, flo_speed, boo_show;
        Animator animator;
        Coroutine corFilling;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            int_mode = Animator.StringToHash(nameof(int_mode));
            flo_speed = Animator.StringToHash(nameof(flo_speed));
            Hide();
        }

        public void Init(bool hasPreAnimation, ref Action<float> onFullToEmpy, ref Action<float> onEmptyToFull, ref Action onShow, ref Action onHide)
        {
            this.hasPreAnimation = hasPreAnimation;
            onFullToEmpy += StartFullToEmpty;
            onEmptyToFull += StartEmptyToFull;
            onShow += Show;
            onHide += Hide;
        }


        public void Exit(ref Action<float> onFullToEmpty, ref Action<float> onEmptyToFull, ref Action onShow, ref Action onHide)
        {
            onFullToEmpty -= StartFullToEmpty;
            onEmptyToFull -= StartEmptyToFull;
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

        public void StartFullToEmpty(float duration)
        {
            if (hasPreAnimation)
            {
                corFilling = this.RestartCoroutine(Delay(), corFilling);
                IEnumerator Delay()
                {
                    fillAnimator.SetInteger(int_mode, (int)FillMode.Full);
                    fillAnimator.SetFloat(flo_speed, 1f / preAnimationDuration);

                    yield return new WaitForSeconds(preAnimationDuration);

                    fillAnimator.SetInteger(int_mode, (int)FillMode.Idle);
                    fillAnimator.SetFloat(flo_speed, 1f / (duration - preAnimationDuration));
                }
            }
            else
            {
                fillAnimator.SetInteger(int_mode, (int)FillMode.Idle);
                fillAnimator.SetFloat(flo_speed, 1f / duration);
            }

        }

        public void StartEmptyToFull(float duration)
        {
            if (hasPreAnimation)
            {
                corFilling = this.RestartCoroutine(Delay(), corFilling);
                IEnumerator Delay()
                {
                    fillAnimator.SetInteger(int_mode, (int)FillMode.Idle);
                    fillAnimator.SetFloat(flo_speed, 1f / preAnimationDuration);

                    yield return new WaitForSeconds(preAnimationDuration);

                    fillAnimator.SetInteger(int_mode, (int)FillMode.Full);
                    fillAnimator.SetFloat(flo_speed, 1f / (duration - preAnimationDuration));
                }
            }
            else
            {
                fillAnimator.SetInteger(int_mode, (int)FillMode.Full);
                fillAnimator.SetFloat(flo_speed, 1f / duration);
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
