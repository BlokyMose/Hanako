using Sirenix.OdinInspector;
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
        bool isDisplayUI = true;

        [SerializeField, ShowIf(nameof(isDisplayUI))]
        protected GameObject destinationUIPrefab;
        
        [SerializeField, ShowIf(nameof(isDisplayUI))]
        protected Transform destinationUIParent;

        [Header("Interact")]
        [SerializeField]
        protected Transform interactablePos;

        [SerializeField]
        protected Transform postInteractPos;

        [SerializeField]
        protected float durationToPostInteractPos = 0.1f;

        [Header("Components")]
        [SerializeField]
        protected List<SpriteRenderer> coloredSRs = new();

        [SerializeField]
        protected List<SpriteRenderer> hoverSRs = new();

        [SerializeField]
        protected List<SpriteRenderer> excludedHoverSRs = new();

        [SerializeField]
        protected HanakoLevelManager levelManager;


        protected Animator animator;    
        protected HanakoDestinationUI destinationUI;
        protected HanakoEnemy currentOccupant, lastOccupant;
        protected float durationLeft;
        protected Dictionary<SpriteRenderer, Color> cacheHoveredSRs = new();
        protected bool isHovered = false;
        protected Vector2 occupantLastPos;

        public event Action<float> OnDurationDepleting;
        public event Action<float> OnDurationStartDepleting;
        public event Action OnDurationEnd;
        public event Action OnInteractStart;
        public event Action OnInteractEnd;


        public HanakoDestinationID ID { get => id; }
        public Transform InteractablePos => interactablePos == null ? transform : interactablePos;

        public virtual bool IsOccupied { get => currentOccupant!=null; }
        public Transform PostInteractPos { get => postInteractPos;  }

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            if (isDisplayUI)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
                destinationUI.Init(ref OnDurationStartDepleting, ref OnInteractStart, ref OnInteractEnd);
            }

            if (hoverSRs.Count == 0)
            {
                hoverSRs.AddRange(gameObject.GetComponentsInFamily<SpriteRenderer>());
            }

            foreach (var sr in excludedHoverSRs)
            {
                var foundSR = hoverSRs.Find(x => x == sr);
                if (foundSR != null) hoverSRs.Remove(foundSR);
            }

            foreach (var sr in coloredSRs)
            {
                var alpha = sr.color.a;
                sr.color = id.Color.ChangeAlpha(alpha);
            }

            if (postInteractPos == null)
                postInteractPos = transform;
        }

        protected virtual void OnDestroy()
        {
            if (isDisplayUI)
            {
                destinationUI.Exit(ref OnDurationStartDepleting, ref OnInteractStart, ref OnInteractEnd);
            }
        }

        public void Init(HanakoLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

        public virtual IEnumerator Interact(HanakoEnemy enemy)
        {
            currentOccupant = enemy;
            durationLeft = interactDuration;

            occupantLastPos = enemy.transform.position;
            StartCoroutine(MoveOccupant(enemy, postInteractPos.position, durationToPostInteractPos));

            WhenInteractStart(enemy);
            OnDurationStartDepleting?.Invoke(interactDuration);
            while (durationLeft > 0f)
            {
                durationLeft -= Time.deltaTime;
                OnDurationDepleting?.Invoke(durationLeft / interactDuration);
                yield return null;
            }
            OnDurationEnd?.Invoke();
            lastOccupant = currentOccupant;
            currentOccupant = null;
            StartCoroutine(MoveOccupant(enemy, occupantLastPos, durationToPostInteractPos));
            WhenInteractEnd(enemy);
        }

        protected virtual void WhenInteractStart(HanakoEnemy enemy)
        {
            OnInteractStart?.Invoke();
        }

        protected virtual void WhenInteractEnd(HanakoEnemy enemy)
        {
            OnInteractEnd?.Invoke();
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


        public virtual void Hover()
        {
            if (isHovered) return;
            isHovered = true;
            cacheHoveredSRs = new();
            ChangeSRsColor(Color.red);
        }

        public virtual void Unhover()
        {
            if (!isHovered) return;
            isHovered = false;
            ResetSRsColor();
        }

        public void ChangeSRsColor(Color color, bool doCache = true)
        {
            foreach (var sr in hoverSRs)
            {
                if (doCache && !cacheHoveredSRs.ContainsKey(sr))
                    cacheHoveredSRs.Add(sr, sr.color);
                sr.color = color;
            }
        }

        public void ResetSRsColor()
        {
            foreach (var sr in cacheHoveredSRs)
                sr.Key.color = sr.Value;
        }
    }
}
