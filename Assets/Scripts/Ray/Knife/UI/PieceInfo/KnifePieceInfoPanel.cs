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

            public Information(string name, string desc, Color? color = null)
            {
                this.name = name;
                this.desc = desc;
                this.color = color;
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

        [SerializeField]
        Animator animator;

        [SerializeField]
        int transitionVariantCount = 3;

        int tri_transition, int_variant;
        bool isCleared = false;

        private void Awake()
        {
            tri_transition = Animator.StringToHash(nameof(tri_transition));
            int_variant = Animator.StringToHash(nameof(int_variant));
            Clear();
        }

        public void SetInformation(Information info)
        {
            isCleared = false;

            nameText.text = info.name;
            descTetx.text = info.desc;

            if (info.Color != null)
                foreach (var image in coloredImages)
                    image.color = (Color)info.Color;

            if (animator != null)
            {
                animator.SetInteger(int_variant, Random.Range(0, transitionVariantCount));
                animator.SetTrigger(tri_transition);
            }

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
            if (animator != null)
            {
                animator.SetTrigger(tri_transition);
            }
            RefreshCanvas();
        }
    }
}
