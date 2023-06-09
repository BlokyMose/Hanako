using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [RequireComponent(typeof(Animator))]
    public class KnifeTile : MonoBehaviour
    {
        public enum TileAnimationMode { Idle, Hovered, Clicked }

        [SerializeField]
        SpriteRenderer sr;

        [SerializeField]
        Collider2D col;

        Animator animator;
        int int_mode;
        TileAnimationMode animationMode = TileAnimationMode.Idle;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));

            if (sr == null)
            {
                sr = UnityUtility.ComponentUtility.GetComponentInFamily<SpriteRenderer>(this);
                if (sr == null) Debug.LogWarning($"{gameObject.name} has no SR");
            }

            if (col == null)
            {
                col = UnityUtility.ComponentUtility.GetComponentInFamily<PolygonCollider2D>(this);
                if (col == null) Debug.LogWarning($"{gameObject.name} has no Collider2D");
            }

            col.isTrigger = true;
        }

        public void Hovered()
        {
            sr.color = Color.red;
            animationMode = TileAnimationMode.Hovered;
            animator.SetInteger(int_mode, (int)animationMode);
        }

        public void Unhovered()
        {
            sr.color = Color.white;
            animationMode = TileAnimationMode.Idle;
            animator.SetInteger(int_mode, (int)animationMode);
        }

        public void Clicked()
        {
            sr.color = Color.yellow;
            animationMode = TileAnimationMode.Clicked;
            animator.SetInteger(int_mode, (int)animationMode);
        }
    }
}
