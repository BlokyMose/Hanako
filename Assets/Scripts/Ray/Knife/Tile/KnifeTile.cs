using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

        Animator animator;
        int int_mode;
        TileAnimationMode animationMode = TileAnimationMode.Idle;

        public SortingGroup SortingGroup { get => sortingGroup; }

        public SpriteRenderer SR { get => sr;  }
        public Transform PieceParent { get => pieceParent; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));

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

        public void Hovered(Color tileColor)
        {
            sr.color = tileColor;
            animationMode = TileAnimationMode.Hovered;
            animator.SetInteger(int_mode, (int)animationMode);
        }

        public void Unhovered(Color? tileColor = null)
        {
            sr.color = tileColor == null ? Color.white : (Color) tileColor;
            animationMode = TileAnimationMode.Idle;
            animator.SetInteger(int_mode, (int)animationMode);
        }

        public void Clicked(Color tileColor)
        {
            sr.color = tileColor;
            animationMode = TileAnimationMode.Clicked;
            animator.SetInteger(int_mode, (int)animationMode);
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

    }
}
