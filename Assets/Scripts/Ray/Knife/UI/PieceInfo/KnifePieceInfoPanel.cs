using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Knife
{
    public class KnifePieceInfoPanel : MonoBehaviour
    {
        [System.Serializable]
        public class Information
        {
            public string name;
            [Multiline]
            public string desc;
            Color? color;
            public Color? Color => color != null ? color : hasDefaultColor ? defaultColor : null;
            [SerializeField]
            bool hasDefaultColor = false;
            [SerializeField]
            Color defaultColor = new (1,1,1,1);
            public Sprite logo;

            public Information(string name, string desc, Color? color = null, Sprite logo = null)
            {
                this.name = name;
                this.desc = desc;
                this.color = color;
                this.logo = logo;
            }

            public Information(KnifePieceInformation info)
            {
                this.name = info.PieceName;
                this.desc = info.Desc;
                this.color = info.Color;
            }

            public Information(KnifeInteraction.Information info)
            {
                this.name = info.Name;
                this.desc = info.Desc;
                this.logo = info.Logo;
            }
        }

        [Header("Components")]
        [SerializeField]
        TextMeshProUGUI nameText;

        [SerializeField]
        TextMeshProUGUI descTetx;

        [SerializeField]
        Image logoImage;

        [SerializeField]
        List<HorizontalOrVerticalLayoutGroup> layoutGroups = new();

        [SerializeField]
        List<Image> coloredImages = new();

        bool isCleared = false;

        private void Awake()
        {
            Clear();
        }

        public void SetInformation(Information info)
        {
            isCleared = false;

            nameText.text = info.name;
            descTetx.text = info.desc;
            
            if (info.logo!=null)
                logoImage.sprite = info.logo;

            if (info.Color != null)
                foreach (var image in coloredImages)
                    image.color = (Color)info.Color;



            RefreshCanvas();
        }

        private void RefreshCanvas()
        {
            Canvas.ForceUpdateCanvases();
            foreach (var group in layoutGroups)
                group.enabled = false;

            foreach (var group in layoutGroups)
                group.enabled = true;
        }

        public void FlipXLogo(bool isFlippedX)
        {
            logoImage.transform.localScale = new((isFlippedX ? -1 : 1), 1, 1);
        }

        public void Clear()
        {
            if (isCleared) return;
            isCleared = true;

            FlipXLogo(false);

            RefreshCanvas();
        }
    }
}
