using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Hanako
{
    public class HanakoDestinationUI : MonoBehaviour
    {
        public enum PlayerHereMode { Idle, Hide }

        [SerializeField]
        Image fillImage;

        [SerializeField]
        Animator playerHere;

        int int_mode;

        private void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));
            HideFill();
        }

        public void Init(ref Action<float> onShowFill, ref Action onHideFill)
        {
            onShowFill += ShowFill;
            onHideFill += HideFill;
        }


        public void Exit(ref Action<float> onShowFill, ref Action onHideFill)
        {
            onShowFill -= ShowFill;
            onHideFill -= HideFill;
        }

        public void ShowFill(float fill)
        {
            fillImage.enabled = true;
            fillImage.fillAmount = 1f-fill;
        }

        public void HideFill()
        {
            fillImage.enabled = false;
        }

        public void ShowPlayerHere()
        {
            playerHere.SetInteger(int_mode, (int)PlayerHereMode.Idle);
        }

        public void HidePlayerHere()
        {
            playerHere.SetInteger(int_mode, (int)PlayerHereMode.Hide);
        }


    }
}
