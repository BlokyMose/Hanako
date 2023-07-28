using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoEnemy : MonoBehaviour
    {
        public enum PieceAnimationState { Die = -1, Idle, Run, Attack }

        [Header("Properties")]
        [SerializeField]
        float moveSpeed = 1;

        [SerializeField]
        List<HanakoDestinationID> destinationSequence = new();

        [Header("Components")]
        [SerializeField]
        GameObject thoughtBubblePrefab;

        [SerializeField]
        Transform thoughtBubbleParent;

        [SerializeField]
        Animator animator;

        HanakoLevelManager levelManager;
        HanakoDestination currentDestination;
        int currentDestinationPointIndex;
        Coroutine corMoving;
        int int_motion;
        bool isKillable = false;
        HanakoThoughtBubble thoughtBubble;

        public HanakoDestination CurrentDestinationPoint { get => currentDestination; }
        public bool IsKillable { get => isKillable; }

        void Awake()
        {
            if (animator == null)
                animator = gameObject.GetComponentInFamily<Animator>();
            int_motion = Animator.StringToHash(nameof(int_motion));
            thoughtBubble = Instantiate(thoughtBubblePrefab, thoughtBubbleParent).GetComponent<HanakoThoughtBubble>();
            thoughtBubble.transform.localPosition = Vector3.zero;
            if (transform.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
                srEditor.ChangeAlpha(0f);
        }

        void Update()
        {
        }

        public void Init(HanakoLevelManager levelManager, List<HanakoDestinationID> destinationSequence)
        {
            this.levelManager = levelManager;
            this.destinationSequence = destinationSequence;
            currentDestinationPointIndex = 0;
            currentDestination = levelManager.GetUnoccupiedDestination(destinationSequence[currentDestinationPointIndex], transform.position);
            if (transform.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
                srEditor.BeOpaqueFromTransparent(0.33f);
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
            if (currentDestination == levelManager.Door) // Prevent going to other destination once door has been reached
                return;

            currentDestinationPointIndex++;
            if (currentDestinationPointIndex < destinationSequence.Count)
                currentDestination = levelManager.GetUnoccupiedDestination(destinationSequence[currentDestinationPointIndex], transform.position);
            else
            {
                currentDestinationPointIndex = -1;
                currentDestination = levelManager.Door;
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
                thoughtBubble.Hide();
                animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);

                if (destination.Occupation == HanakoDestination.OccupationMode.Unoccupied)
                {
                    isKillable = false;
                    yield return StartCoroutine(destination.Interact(this));
                    isKillable = true;

                    MoveToNextDestination();
                }
                else
                {
                    MoveToNextDestination();
                }
            }
        }

        public void PlayAnimation(PieceAnimationState state)
        {
            animator.SetInteger(int_motion, (int)state);
        }
    }
}
