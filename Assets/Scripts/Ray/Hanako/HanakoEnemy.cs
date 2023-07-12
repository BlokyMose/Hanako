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
        List<int> destinationPointSequence = new();

        [Header("Components")]
        [SerializeField]
        HanakoLevelManager levelManager;

        [SerializeField]
        Animator animator;

        HanakoDestination currentDestinationPoint;
        int currentDestinationPointIndex;
        Coroutine corMoving;
        int int_motion;

        public HanakoDestination CurrentDestinationPoint { get => currentDestinationPoint; }

        void Awake()
        {
            if (animator == null)
                animator = gameObject.GetComponentInFamily<Animator>();
            int_motion = Animator.StringToHash(nameof(int_motion));
        }

        void Update()
        {
        }

        public void Init(HanakoLevelManager levelManager, List<int> destinationPointSequence)
        {
            this.levelManager = levelManager;
            this.destinationPointSequence = destinationPointSequence;
            currentDestinationPointIndex = 0;
            currentDestinationPoint = levelManager.GetDestinationPoint(destinationPointSequence[currentDestinationPointIndex]);
        }

        public void MoveToCurrentDestination()
        {
            MoveTo(currentDestinationPoint);
        }

        public void MoveToNextDestination()
        {
            if (currentDestinationPoint == levelManager.Door) // Prevent going to other destination once door has been reached
                return;

            currentDestinationPointIndex++;
            if (currentDestinationPointIndex < destinationPointSequence.Count)
                currentDestinationPoint = levelManager.GetDestinationPoint(destinationPointSequence[currentDestinationPointIndex]);
            else
            {
                currentDestinationPointIndex = -1;
                currentDestinationPoint = levelManager.Door;
            }
            
            MoveToCurrentDestination();
        }

        public void MoveTo(HanakoDestination destinationPoint)
        {

            corMoving = this.RestartCoroutine(Moving(), corMoving);
            IEnumerator Moving()
            {
                animator.SetInteger(int_motion, (int)PieceAnimationState.Run);
                while (true)
                {
                    var destinationX = destinationPoint.Position.position.x;
                    var currentX = transform.position.x;
                    var isDirectionRight = destinationX > currentX;
                    transform.localEulerAngles = new(transform.localEulerAngles.x, isDirectionRight ? 0 : 180);
                    transform.position = new(transform.position.x + (isDirectionRight ? 1 : -1) * moveSpeed * Time.deltaTime, transform.position.y);

                    var distance = Mathf.Abs(destinationX - currentX);
                    if (distance < 0.1f)
                        break;

                    yield return null;
                }
                animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);

                MoveToNextDestination();
            }
        }

    }
}
