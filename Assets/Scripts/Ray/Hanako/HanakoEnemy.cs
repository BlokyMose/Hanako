using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    public class HanakoEnemy : MonoBehaviour
    {
        public enum PieceAnimationState { Die = -1, Idle, Run, Attack, Pushed, Scared = 11 }

        public enum HighlightMode { None, Detecting, Attackable }

        [Header("Initialization")]
        [SerializeField]
        bool autoTransparent = true;

        [Header("Properties")]
        [SerializeField]
        float moveSpeed = 1;

        [SerializeField]
        List<HanakoDestinationID> destinationSequence = new();

        [Header("Components")]
        [SerializeField]
        ColliderProxy colDetectArea;

        [SerializeField]
        GameObject thoughtBubblePrefab;

        [SerializeField]
        Transform thoughtBubbleParent;

        [SerializeField]
        SpriteRenderer highlightIcon;

        [SerializeField]
        Animator animator;

        [SerializeField]
        List<GameObject> gosToDeactivateWhenNotMoving = new();

        [Header("Customizations")]
        [SerializeField]
        HanakoColors colors;

        [SerializeField]
        HanakoIcons icons;

        SpriteRendererColorSetter colorSetter;
        HanakoDestination currentDestination;
        int currentDestinationPointIndex;
        Coroutine corMoving;
        int int_motion;
        bool isKillable = false;
        bool isAlive = true;
        bool hasInitialMove = false;
        HanakoThoughtBubble thoughtBubble;
        HanakoDestination_ExitDoor exitDoor;

        public HanakoDestination CurrentDestinationPoint { get => currentDestination; }
        public bool IsKillable { get => isKillable; }
        public bool IsAlive { get => isAlive; }
        public List<HanakoDestinationID> DestinationSequence { get => destinationSequence; }
        public event Func<HanakoDestinationID, Vector2, HanakoDestination> GetUnoccupiedDestination;

        void Awake()
        {
            colorSetter = GetComponent<SpriteRendererColorSetter>();
            if (animator == null)
                animator = gameObject.GetComponentInFamily<Animator>();
            int_motion = Animator.StringToHash(nameof(int_motion));
            thoughtBubble = Instantiate(thoughtBubblePrefab, thoughtBubbleParent).GetComponent<HanakoThoughtBubble>();
            thoughtBubble.transform.localPosition = Vector3.zero;
            highlightIcon.sprite = null;

            if (autoTransparent)
            {
                if (transform.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
                    srEditor.ChangeAlpha(0f);

                DeactivateGOs(gosToDeactivateWhenNotMoving);
            }

            if (colDetectArea!=null)
                colDetectArea.DisableCollider();
        }

        public void Init(
            List<HanakoDestinationID> destinationSequence, 
            HanakoDestination_ExitDoor exitDoor, 
            Func<HanakoDestinationID, Vector2, HanakoDestination> getUnoccupiedDestination,
            HanakoColors colors,
            HanakoIcons icons)
        {
            this.destinationSequence = destinationSequence;
            this.exitDoor = exitDoor;
            this.GetUnoccupiedDestination = getUnoccupiedDestination;
            this.colors = colors;
            this.icons = icons;
        }

        public void StartInitialMove()
        {
            hasInitialMove = true;
            currentDestinationPointIndex = 0;
            currentDestination = GetUnoccupiedDestination(destinationSequence[currentDestinationPointIndex], transform.position);
            MoveToCurrentDestination();
            if (transform.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
                srEditor.BeOpaqueFromTransparent(0.33f);

            if (colDetectArea != null)
            {
                colDetectArea.OnEnter += OnEnter;
                colDetectArea.OnExit += OnExit;
                colDetectArea.EnableCollider();

                void OnEnter(Collider2D col)
                {
                    if (col.TryGetComponent<HanakoDestination>(out var destination))
                    {
                        destination.AddDetectedBy(this);
                    }
                }

                void OnExit(Collider2D col)
                {
                    if (col.TryGetComponent<HanakoDestination>(out var destination))
                    {
                        destination.RemoveDetectedBy(this);
                    }
                }
            }
        }

        public void MoveToCurrentDestination()
        {
            if (currentDestination != null)
                MoveTo(currentDestination);
            else
                MoveToNextDestination();
        }

        public void MoveToNextDestination()
        {
            if (currentDestination == exitDoor) // Prevent going to other destination once door has been reached
                return;

            currentDestinationPointIndex++;
            if (currentDestinationPointIndex < destinationSequence.Count)
                currentDestination = GetUnoccupiedDestination(destinationSequence[currentDestinationPointIndex], transform.position);
            else
            {
                currentDestinationPointIndex = -1;
                currentDestination = exitDoor;
            }

            MoveToCurrentDestination();
        }

        public void MoveTo(HanakoDestination destination)
        {
            corMoving = this.RestartCoroutine(Moving(), corMoving);
            IEnumerator Moving()
            {
                isKillable = true;
                thoughtBubble.Show(destination.ID.Logo, destination.ID.Color);
                animator.SetInteger(int_motion, (int)PieceAnimationState.Run);
                ActivateGOs(gosToDeactivateWhenNotMoving);
                colDetectArea.EnableCollider();
                while (true)
                {
                    var destinationX = destination.InteractablePos.position.x;
                    var currentX = transform.position.x;
                    var isDirectionRight = destinationX > currentX;
                    transform.localEulerAngles = new(transform.localEulerAngles.x, isDirectionRight ? 0 : 180);
                    transform.position = new(transform.position.x + (isDirectionRight ? 1 : -1) * moveSpeed * Time.deltaTime, transform.position.y);

                    var distance = Mathf.Abs(destinationX - currentX);
                    if (distance < 0.1f)
                        break;

                    yield return null;
                }

                if (!isAlive) yield break;

                thoughtBubble.Hide();
                animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);
                DeactivateGOs(gosToDeactivateWhenNotMoving);
                colDetectArea.DisableCollider();

                if (destination.Occupation == HanakoDestination.OccupationMode.Unoccupied)
                {
                    isKillable = false;
                    yield return StartCoroutine(destination.Occupy(this));
                    isKillable = true;

                    MoveToNextDestination();
                }
                else
                {
                    MoveToNextDestination();
                }


            }
        }

        public void DetectHanako(Vector2 hanakoPos)
        {
            this.StopCoroutineIfExists(corMoving);
            animator.SetInteger(int_motion, (int)PieceAnimationState.Scared);
            var isFacingRight = hanakoPos.x > transform.position.x;
            transform.localEulerAngles = new(0, isFacingRight ? 0 : 180, 0);
            thoughtBubble.Hide();
        }

        public void PlayAnimation(PieceAnimationState state)
        {
            animator.SetInteger(int_motion, (int)state);
        }

        void DeactivateGOs(List<GameObject> gos)
        {
            foreach (var go in gos)
                go.SetActive(false);
        }

        void ActivateGOs(List<GameObject> gos)
        {
            foreach (var go in gos)
                go.SetActive(true);
        }

        public void ReceiveAttack(HanakoDestination_Toilet toilet, float delayScalingAnimation, float scalingAnimationDuration)
        {
            if (!isKillable) return;
            isKillable = false;
            isAlive = false; // this will prevent corMoving from entering a destination

            thoughtBubble.Hide();
            animator.SetInteger(int_motion, (int)PieceAnimationState.Pushed);
            colDetectArea.DisableCollider();
            DeactivateGOs(gosToDeactivateWhenNotMoving);

            StartCoroutine(Animating());
            IEnumerator Animating()
            {
                yield return new WaitForSeconds(delayScalingAnimation);
                this.StopCoroutineIfExists(corMoving);

                var time = 0f;
                var originPos = transform.position;
                var originScale = transform.localScale;
                while (time<scalingAnimationDuration)
                {
                    transform.localScale = Vector2.Lerp(originScale, Vector2.zero, time / scalingAnimationDuration);
                    transform.position = Vector2.Lerp(originPos, toilet.PostAttackPos.position,time/scalingAnimationDuration);
                    time += Time.deltaTime;
                    yield return null;
                }

                transform.position = toilet.PostInteractPos.position;
                transform.localScale = Vector2.zero;
            }
        }

        public void Highlight(HighlightMode highlightMode)
        {
            if (!hasInitialMove) return;

            switch (highlightMode)
            {
                case HighlightMode.None:
                    ResetColor();
                    ResetFeedbackSign();
                    break;
                case HighlightMode.Detecting:
                    ChangeColor(colors.DetectingColor);
                    ChangeFeedbackSign(icons.WarningIcon, colors.DetectingColor);
                    break;
                case HighlightMode.Attackable:
                    ChangeColor(colors.AttackableColor);
                    ChangeFeedbackSign(icons.OkCircleIcon, colors.AttackableColor);
                    break;
            }
        }

        void ChangeColor(Color color)
        {
            colorSetter.ChangeColor(color);
        }

        void ResetColor()
        {
            colorSetter.ResetColorExceptAlpha();
        }

        void ChangeFeedbackSign(Sprite sprite, Color color)
        {
            highlightIcon.sprite = sprite;
            highlightIcon.color = color.ChangeAlpha(highlightIcon.color.a);
        }

        void ResetFeedbackSign()
        {
            highlightIcon.sprite = null;
        }
    }
}
