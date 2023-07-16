using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Hanako
{
    public class HanakoDestinationUI : MonoBehaviour
    {
        [SerializeField]
        Image fillImage;

        private void Awake()
        {
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
    }
}
