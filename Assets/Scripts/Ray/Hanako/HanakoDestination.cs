using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    public class HanakoDestination : MonoBehaviour
    {
        [SerializeField]
        private HanakoDestinationID id;

        [SerializeField]
        protected float interactDuration = 1f;

        [Header("UI")]
        [SerializeField]
        protected GameObject destinationUIPrefab;
        
        [SerializeField]
        protected Transform destinationUIParent;

        [Header("Components")]
        [SerializeField]
        protected List<SpriteRenderer> srs = new();

        [SerializeField]
        protected HanakoLevelManager levelManager;

        [SerializeField]
        protected Transform position;

        protected Animator animator;    
        protected HanakoDestinationUI destinationUI;
        protected HanakoEnemy currentOccupant, lastOccupant;
        protected float durationLeft;
        public event Action<float> OnDurationDepleted;
        public event Action OnDurationEnd;

        public HanakoDestinationID ID { get => id; }
        public Transform Position => position == null ? transform : position;

        public virtual bool IsOccupied { get => currentOccupant!=null; }

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
            destinationUI.transform.localPosition = Vector3.zero;
            destinationUI.Init(ref OnDurationDepleted, ref OnDurationEnd);

            if (srs.Count == 0)
            {
                srs.AddRange(gameObject.GetComponentsInFamily<SpriteRenderer>());
            }
        }

        protected virtual void OnDestroy()
        {
            destinationUI.Exit(ref OnDurationDepleted, ref OnDurationEnd);
        }

        public void Init(HanakoLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

        public virtual IEnumerator Interact(HanakoEnemy enemy)
        {
            currentOccupant = enemy;
            durationLeft = interactDuration;
            WhenInteractStart(enemy);
            while (durationLeft > 0f)
            {
                durationLeft -= Time.deltaTime;
                OnDurationDepleted?.Invoke(durationLeft / interactDuration);
                yield return null;
            }
            OnDurationEnd?.Invoke();
            lastOccupant = currentOccupant;
            currentOccupant = null;
            WhenInteractEnd(enemy);
        }

        protected virtual void WhenInteractStart(HanakoEnemy enemy)
        {

        }

        protected virtual void WhenInteractEnd(HanakoEnemy enemy)
        {

        }

        protected IEnumerator MoveOccupant(HanakoEnemy occupant, Vector2 targetPos, float duration)
        {
            occupant.PlayAnimation(HanakoEnemy.PieceAnimationState.Run);
            var speed = Vector2.Distance(transform.position, targetPos) / duration;
            var time = 0f;
            while (time < duration)
            {
                occupant.transform.position = Vector2.MoveTowards(occupant.transform.position, targetPos, speed * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            if (this.currentOccupant == occupant)
                occupant.PlayAnimation(HanakoEnemy.PieceAnimationState.Idle);
        }
    }
}
