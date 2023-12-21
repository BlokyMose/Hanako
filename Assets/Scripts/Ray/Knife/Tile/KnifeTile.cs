using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityUtility;

namespace Hanako.Knife
{
    [RequireComponent(typeof(Animator))]
    public class KnifeTile : MonoBehaviour
    {
        public enum TileAnimationMode { Idle, Hovered, Clicked }

        [SerializeField]
        SpriteRenderer sr;

        [SerializeField]
        bool useSRAsPieceParent = true;

        [SerializeField, ShowIf(nameof(useSRAsPieceParent))]
        Transform pieceParent;

        [SerializeField]
        SortingGroup sortingGroup;

        [SerializeField]
        Collider2D col;

        [SerializeField]
        SpriteRenderer interactionIconSR;

        [SerializeField]
        Animator interactionIconAnimator;

        Animator animator;
        int int_mode, tri_transition;
        TileAnimationMode animationMode = TileAnimationMode.Idle;
        float animatingColorDuration = 1f;

        public SortingGroup SortingGroup { get => sortingGroup; }
        public SpriteRenderer SR { get => sr;  }
        public Transform PieceParent { get => pieceParent; }
        Coroutine corAnimatingColor;

        void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_transition = Animator.StringToHash(nameof(tri_transition));

            if (useSRAsPieceParent || (!useSRAsPieceParent && pieceParent == null))
            {
                pieceParent = sr.transform;
            }

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

        public void CallAwake() => Awake();

        public void Hovered(Color tileColor, bool animateColor = true, ActionIconPack interactionIcon = null)
        {
            sr.color = tileColor;
            if (animateColor)
                corAnimatingColor = this.RestartCoroutine(AnimatingColor(sr, Color.white, tileColor, animatingColorDuration), corAnimatingColor);
            else
                this.StopCoroutineIfExists(corAnimatingColor);

            animationMode = TileAnimationMode.Hovered;
            animator.SetInteger(int_mode, (int)animationMode);

            if (interactionIcon != null)
            {
                interactionIconSR.sprite = interactionIcon.Icon;
                if (interactionIcon.IsOverrideColor)
                    interactionIconSR.color = interactionIcon.Color;
                interactionIconAnimator.SetInteger(int_mode, (int)interactionIcon.Animation);
                interactionIconAnimator.SetTrigger(tri_transition);
            }
        }

        public void Idle(Color? tileColor = null)
        {
            sr.color = tileColor == null ? Color.white : (Color) tileColor;
            this.StopCoroutineIfExists(corAnimatingColor);
            animationMode = TileAnimationMode.Idle;
            animator.SetInteger(int_mode, (int)animationMode);
            interactionIconAnimator.SetInteger(int_mode, (int)ActionIconMode.Hide);
            interactionIconAnimator.SetTrigger(tri_transition);
        }

        public void Hinted(Color tileColor, bool animateColor = true)
        {
            sr.color = tileColor;
            if (animateColor)
                corAnimatingColor = this.RestartCoroutine(AnimatingColor(sr, Color.white, tileColor, animatingColorDuration), corAnimatingColor);
            else
                this.StopCoroutineIfExists(corAnimatingColor);

            animationMode = TileAnimationMode.Idle;
            animator.SetInteger(int_mode, (int)animationMode);
            interactionIconAnimator.SetInteger(int_mode, (int)ActionIconMode.Hide);
            interactionIconAnimator.SetTrigger(tri_transition);

        }

        public void Clicked(Color tileColor)
        {
            sr.color = tileColor;
            this.StopCoroutineIfExists(corAnimatingColor);
            animationMode = TileAnimationMode.Clicked;
            animator.SetInteger(int_mode, (int)animationMode);
            interactionIconAnimator.SetInteger(int_mode, (int)ActionIconMode.Hide);
            interactionIconAnimator.SetTrigger(tri_transition);

        }

        public void SetAsParentOf(GameObject child)
        {
            child.transform.parent = pieceParent;
        }

        public bool TryGetPiece(out KnifePiece piece)
        {
            piece = pieceParent.GetComponentInChildren<KnifePiece>();
            return piece != null;
        }

        public KnifePiece GetPiece()
        {
            return pieceParent.GetComponentInChildren<KnifePiece>();
        }

        IEnumerator AnimatingColor(SpriteRenderer sr, Color colorA, Color colorB, float duration)
        {
            var time = 0f;
            var currentTargetColor = colorA;
            while (true)
            {
                sr.color = sr.color.TransitionColor(currentTargetColor, Time.deltaTime);
                time += Time.deltaTime;
                if (time > duration)
                {
                    currentTargetColor = currentTargetColor == colorA ? colorB : colorA;
                    time = 0;
                }
                yield return null;
            }
        }
    }
}
