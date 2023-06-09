using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Knife
{
    public class RoundCheckBox : MonoBehaviour
    {
        [SerializeField]
        Image border;

        [SerializeField]
        Image fill;

        [SerializeField]
        Image dot;

        [SerializeField]
        List<Sprite> borderVariants = new();

        [SerializeField]
        List<Sprite> fillVariants = new();

        [SerializeField]
        List<Sprite> dotVariants = new();

        bool isChecked = false;
        bool isDotted = false;
        Animator borderAnimator;
        int tri_jiggle;
        public bool IsChecked { get => isChecked; }
        public bool IsDotted { get => isDotted; }

        private void Awake()
        {
            borderAnimator = border.GetComponent<Animator>();
            tri_jiggle = Animator.StringToHash(nameof(tri_jiggle));
        }

        public void Init(bool randomize = true)
        {
            if (randomize)
            {
                border.sprite = borderVariants.GetRandom();
                fill.sprite = fillVariants.GetRandom();
                dot.sprite = dotVariants.GetRandom();
            }
            fill.gameObject.SetActive(false);
            dot.gameObject.SetActive(false);
        }

        public void Dot()
        {
            if (isDotted) return;
            borderAnimator.SetTrigger(tri_jiggle);
            isDotted = true;
            dot.gameObject.SetActive(true);
            fill.gameObject.SetActive(false);
        }
        
        public void CancelDot()
        {
            isDotted = false;
            dot.gameObject.SetActive(false);
        }

        public void Check()
        {
            if (isChecked) return;

            isChecked = true;
            fill.gameObject.SetActive(true);
            dot.gameObject.SetActive(false);
        }

         public void CancelCheck()
        {
            isChecked = false;
            fill.gameObject.SetActive(false);
        }


    }
}
