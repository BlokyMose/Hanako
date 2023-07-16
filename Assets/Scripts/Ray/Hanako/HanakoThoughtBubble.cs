using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Hanako
{
    public class HanakoThoughtBubble : MonoBehaviour
    {
        [SerializeField]
        Image logo;

        void Awake()
        {
            Hide();
        }

        public void Show(Sprite sprite)
        {
            logo.color = new Color(1, 1, 1, 1);
            logo.sprite = sprite;
        }

        public void Hide()
        {
            logo.color = new Color(1, 1, 1, 0);
            logo.sprite = null;
        }
    }
}
