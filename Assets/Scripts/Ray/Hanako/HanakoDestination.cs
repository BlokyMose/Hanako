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
        public enum OccupationMode { Unoccupied, Enemy, Player }

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
        protected Transform postAttackPos;

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

        [Header("Customizations")]
        [SerializeField]
        protected HanakoColors colors;


        protected Animator animator;    
        protected HanakoDestinationUI destinationUI;
        protected HanakoEnemy currentOccupant, lastOccupant;
        protected float durationLeft;
        protected Dictionary<SpriteRenderer, Color> cacheHoveredSRs = new();
        protected bool isHovered = false;
        protected Vector2 occupantLastPos;
        protected OccupationMode occupationMode = OccupationMode.Unoccupied;
        protected HashSet<HanakoEnemy> detectingEnemies = new();

        public event Action<float> OnDurationDepleting;
        public event Action<float> OnDurationStartDepleting;
        public event Action OnDurationEnd;
        public event Action OnOccupationStart;
        public event Action OnOccupationEnd;


        public HanakoDestinationID ID { get => id; }
        public Transform InteractablePos => interactablePos == null ? transform : interactablePos;

        public virtual OccupationMode Occupation { get => occupationMode; }
        public Transform PostInteractPos { get => postInteractPos;  }
        public Transform PostAttackPos { get => postAttackPos;  }

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            if (isDisplayUI)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
                destinationUI.Init(ref OnDurationStartDepleting, ref OnOccupationStart, ref OnOccupationEnd);
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
                destinationUI.Exit(ref OnDurationStartDepleting, ref OnOccupationStart, ref OnOccupationEnd);
            }
        }

        public void Init(HanakoLevelManager levelManager)
        {
            this.levelManager = levelManager;
            this.colors = levelManager.Colors;
        }

        public virtual IEnumerator Occupy(HanakoEnemy enemy)
        {
            currentOccupant = enemy;
            occupantLastPos = enemy.transform.position;
            occupationMode = OccupationMode.Enemy;
            if (colors == null) Debug.Log("AAA :"+levelManager+" : "+gameObject.name);
            ChangeSRsColor(colors.OccupiedColor);
            StartCoroutine(MoveOccupant(enemy, postInteractPos.position, durationToPostInteractPos));
            WhenOccupationStart(enemy);

            yield return StartCoroutine(DepletingDuration());

            lastOccupant = currentOccupant;
            currentOccupant = null;
            occupationMode = OccupationMode.Unoccupied;
            ResetSRsColor();
            StartCoroutine(MoveOccupant(enemy, occupantLastPos, durationToPostInteractPos));
            WhenOccupationEnd(enemy);

            IEnumerator DepletingDuration()
            {
                OnDurationStartDepleting?.Invoke(interactDuration);
                durationLeft = interactDuration;
                while (durationLeft > 0f)
                {
                    durationLeft -= Time.deltaTime;
                    OnDurationDepleting?.Invoke(durationLeft / interactDuration);
                    yield return null;
                }
                OnDurationEnd?.Invoke();
            }
        }

        protected virtual void WhenOccupationStart(HanakoEnemy enemy)
        {
            OnOccupationStart?.Invoke();
        }

        protected virtual void WhenOccupationEnd(HanakoEnemy enemy)
        {
            OnOccupationEnd?.Invoke();
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
            if (isHovered || occupationMode != OccupationMode.Unoccupied) return;
            isHovered = true;
            cacheHoveredSRs = new();
            ChangeSRsColor(colors.HoverColor);
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
                {
                    cacheHoveredSRs.Add(sr, sr.color);
                }
                sr.color = color;
            }
        }

        public void ResetSRsColor()
        {
            foreach (var sr in cacheHoveredSRs)
                sr.Key.color = sr.Value;
        }

        public void AddDetectedBy(HanakoEnemy enemy)
        {
            detectingEnemies.Add(enemy);
        }

        public void RemoveDetectedBy(HanakoEnemy enemy)
        {
            detectingEnemies.Remove(enemy);
        }
    }
}
