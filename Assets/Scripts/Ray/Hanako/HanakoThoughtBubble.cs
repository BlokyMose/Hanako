using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    public class HanakoThoughtBubble : MonoBehaviour
    {
        public enum Mode { Idle, Hide }

        [SerializeField]
        Image logo;

        Animator animator;
        int int_mode;

        void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            Hide();
        }

        public void Show(Sprite sprite, Color color)
        {
            animator.SetInteger(int_mode, (int)Mode.Idle);
            logo.color = color;
            logo.sprite = sprite;
        }

        public void Hide()
        {
            animator.SetInteger(int_mode, (int)Mode.Hide);
        }
    }
}
