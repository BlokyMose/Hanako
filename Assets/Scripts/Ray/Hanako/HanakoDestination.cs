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
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    public class HanakoDestination : MonoBehaviour
    {
        public enum OccupationMode { Unoccupied, Enemy, Player }
        public enum ActionIconMode { Hide, Tilting, Nodding }

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

        [Header("Action Icon")]
        [SerializeField]
        protected SpriteRenderer actionIconSR;

        [SerializeField]
        protected Animator actionIconAnimator;

        [SerializeField]
        protected Sprite actionIcon;

        [SerializeField]
        protected ActionIconMode actionIconMode = ActionIconMode.Tilting;

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
        protected List<SpriteRenderer> coloredSRsByID = new();

        [Header("Customizations")]
        [SerializeField]
        protected HanakoColors colors;


        protected Animator animator;    
        protected SpriteRendererColorSetter colorSetter;
        protected HanakoDestinationUI destinationUI;
        protected HanakoEnemy currentOccupant, lastOccupant;
        protected float durationLeft;
        protected Dictionary<SpriteRenderer, Color> cacheHoveredSRs = new();
        protected bool isHovered = false;
        protected Vector2 occupantLastPos;
        protected OccupationMode occupationMode = OccupationMode.Unoccupied;
        protected HashSet<HanakoEnemy> enemiesDetecting = new();
        protected int int_mode, tri_transition;

        public event Action<float> OnDurationDepleting;
        public event Action<float> OnDurationStartDepleting;
        public event Action OnDurationEnd;
        public event Action OnOccupationStart;
        public event Action OnOccupationEnd;
        public event Func<HanakoLevelManager.HanakoGameState> GetGameState;


        public HanakoDestinationID ID { get => id; }
        public Transform InteractablePos => interactablePos == null ? transform : interactablePos;

        public virtual OccupationMode Occupation { get => occupationMode; }
        public Transform PostInteractPos { get => postInteractPos;  }
        public Transform PostAttackPos { get => postAttackPos;  }

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_transition = Animator.StringToHash(nameof(tri_transition));

            colorSetter = GetComponent<SpriteRendererColorSetter>();
            colorSetter.RemoveSR(actionIconSR);
            if (isDisplayUI)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
                destinationUI.Init(ref OnDurationStartDepleting, ref OnOccupationStart, ref OnOccupationEnd);
            }

            foreach (var sr in coloredSRsByID)
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

        public void Init(HanakoColors colors, Func<HanakoLevelManager.HanakoGameState> getGameState)
        {
            this.colors = colors;
            this.GetGameState = getGameState;   
        }

        public virtual IEnumerator Occupy(HanakoEnemy enemy)
        {
            currentOccupant = enemy;
            occupantLastPos = enemy.transform.position;
            occupationMode = OccupationMode.Enemy;
            ChangeColor(colors.OccupiedColor);
            StartCoroutine(MoveOccupant(enemy, postInteractPos.position, durationToPostInteractPos));
            WhenOccupationStart(enemy);

            yield return StartCoroutine(DepletingDuration());

            if (GetGameState() == HanakoLevelManager.HanakoGameState.Lost)
                yield break;

            lastOccupant = currentOccupant;
            currentOccupant = null;
            occupationMode = OccupationMode.Unoccupied;
            ResetColor();
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
            if (isHovered || occupationMode == OccupationMode.Enemy) return;
            isHovered = true;
            cacheHoveredSRs = new();
            ChangeColor(colors.HoverColor);
            ShowActionIcon();
        }

        protected virtual void ShowActionIcon()
        {
            actionIconSR.sprite = actionIcon;
            actionIconSR.color = colors.PlayerColor;
            actionIconAnimator.SetInteger(int_mode, (int)actionIconMode);
            actionIconAnimator.SetTrigger(tri_transition);
        }

        protected void HideActionIcon()
        {
            actionIconAnimator.SetInteger(int_mode, (int)ActionIconMode.Hide);
            actionIconAnimator.SetTrigger(tri_transition);
        }

        public virtual void Unhover()
        {
            if (!isHovered) return;
            HideActionIcon();

            if (occupationMode == OccupationMode.Player)
            {
                isHovered = false;
                ChangeColor(colors.PlayerColor);
            }
            else
            {
                isHovered = false;
                ResetColor();
            }
        }

        public void ChangeColor(Color color)
        {
            colorSetter.ChangeColor(color);
        }

        public void ResetColor()
        {
            colorSetter.ResetColor();
        }

        public virtual void AddDetectedBy(HanakoEnemy enemy)
        {
            enemiesDetecting.Add(enemy);
        }

        public virtual void RemoveDetectedBy(HanakoEnemy enemy)
        {
            enemiesDetecting.Remove(enemy);
        }
    }
}
