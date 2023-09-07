using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Hub
{
    public class HubMinimapIcon : MonoBehaviour
    {
        [SerializeField]
        Image iconImage;

        [SerializeField]
        Image glowImage;

        public void Init(Sprite icon, Color? glowColor = null)
        {
            iconImage.sprite = icon;
            if (glowColor != null)
                glowImage.color = (Color) glowColor;
        }
    }
}
