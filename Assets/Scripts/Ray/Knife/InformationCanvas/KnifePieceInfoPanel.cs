using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Knife
{
    public class KnifePieceInfoPanel : MonoBehaviour
    {
        public class Information
        {
            public string name;
            public string desc;
            public Sprite logo;

            public Information(string name, string desc, Sprite logo)
            {
                this.name = name;
                this.desc = desc;
                this.logo = logo;
            }
        }

        [SerializeField]
        TextMeshProUGUI nameText;

        [SerializeField]
        TextMeshProUGUI descTetx;

        [SerializeField]
        Image logoImage;

        [SerializeField]
        CanvasGroup logoCG;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup allParent;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup textsParent;

        private void Awake()
        {
            Clear();
        }

        public void SetInformation(Information info)
        {
            nameText.text = info.name;
            descTetx.text = info.desc;
            logoImage.sprite = info.logo;
            logoCG.alpha = 1f;

            Canvas.ForceUpdateCanvases();
            allParent.enabled = false;
            allParent.enabled = true;
            textsParent.enabled = false;
            textsParent.enabled = true;
        }

        public void FlipXLogo(bool isFlippedX)
        {
            logoImage.transform.localScale = new((isFlippedX ? -1 : 1), 1, 1);
        }

        public void Clear()
        {
            nameText.text = "";
            descTetx.text = "";
            logoCG.alpha = 0f;
            FlipXLogo(false);
        }
    }
}
