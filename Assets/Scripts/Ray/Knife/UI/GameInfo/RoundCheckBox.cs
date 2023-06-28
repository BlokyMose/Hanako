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
        Image checkBox;

        [SerializeField]
        Image fill;

        [SerializeField]
        Image dot;

        [SerializeField]
        List<Sprite> checkBoxVariants = new();

        [SerializeField]
        List<Sprite> fillVariants = new();

        [SerializeField]
        List<Sprite> dotVariants = new();

        bool isChecked = false;
        bool isDotted = false;

        public bool IsChecked { get => isChecked; }
        public bool IsDotted { get => isDotted; }

        public void Init(bool randomize = true)
        {
            if (randomize)
            {
                checkBox.sprite = checkBoxVariants.GetRandom();
                fill.sprite = fillVariants.GetRandom();
                dot.sprite = dotVariants.GetRandom();
            }
            fill.gameObject.SetActive(false);
            dot.gameObject.SetActive(false);
        }

        public void Dot()
        {
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
