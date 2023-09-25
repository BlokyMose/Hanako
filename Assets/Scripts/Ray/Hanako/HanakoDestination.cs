using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    public class HanakoDestination : MonoBehaviour
    {
        public enum OccupationMode { Unoccupied, Enemy, Player }

        #region [Variables]

        [SerializeField]
        private HanakoDestinationID id;

        [SerializeField]
        int indexOfAllDestinations;

        [SerializeField]
        int indexOfSameID;

        [SerializeField]
        protected float interactDuration = 1f;

        [Header("UI")]

        [SerializeField]
        SpriteRenderer logoSR;

        [SerializeField]
        GameObject actionIconPrefab;

        [SerializeField, ShowIf("@" + nameof(actionIconPrefab))]
        Transform actionIconParent;

        [SerializeField]
        protected GameObject destinationUIPrefab;
        
        [SerializeField, ShowIf("@" + nameof(destinationUIPrefab))]
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
        protected List<SpriteRenderer> coloredSRsByID = new();

        [Header("Customizations")]
        [SerializeField]
        protected HanakoColors colors;

        [SerializeField]
        protected HanakoIcons icons;

        #endregion

        #region [Variables: Data Handlers]

        protected Animator animator;
        protected Animator actionIconAnimator;
        protected SpriteRenderer actionIconSR;
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

        #endregion

        #region [Events]

        public event Action<float> OnDurationDepleting;
        public event Action<float> OnDurationStartDepleting;
        public event Action<float> OnDurationStartFilling;
        public event Action OnDurationEnd;
        public event Action OnOccupationStart;
        public event Action OnOccupationEnd;
        public event Action<HanakoEnemy, Vector2> OnMoveOccupantEnd;
        public event Func<HanakoLevelManager.HanakoGameState> GetGameState;

        #endregion

        #region [Public Getters]

        public HanakoDestinationID ID { get => id; }
        public Transform InteractablePos => interactablePos == null ? transform : interactablePos;

        public virtual OccupationMode Occupation { get => occupationMode; }
        public Transform PostInteractPos { get => postInteractPos;  }
        public Transform PostAttackPos { get => postAttackPos;  }
        public int IndexOfAllDestinations { get => indexOfAllDestinations; }
        public int IndexOfSameID { get => indexOfSameID; }

        #endregion

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_transition = Animator.StringToHash(nameof(tri_transition));

            colorSetter = GetComponent<SpriteRendererColorSetter>();
            colorSetter.RemoveSR(actionIconSR);
            if (destinationUIPrefab!=null)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
                destinationUI.Init(true, ref OnDurationStartDepleting, ref OnDurationStartFilling, ref OnOccupationStart, ref OnOccupationEnd);
            }

            if (actionIconPrefab != null)
            {
                var actionIconGO = Instantiate(actionIconPrefab, actionIconParent);
                actionIconAnimator = actionIconGO.GetComponent<Animator>();
                actionIconSR = actionIconGO.GetComponentInChildren<SpriteRenderer>();
                colorSetter.RemoveSR(actionIconSR);
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
            if (destinationUIPrefab != null)
            {
                destinationUI.Exit(ref OnDurationStartDepleting, ref OnDurationStartFilling, ref OnOccupationStart, ref OnOccupationEnd);
            }
        }

        public void Init(HanakoColors colors, HanakoIcons icons, Func<HanakoLevelManager.HanakoGameState> getGameState, int indexOfAllDestinations, int indexOfSameID)
        {
            this.colors = colors;
            this.icons = icons;
            this.GetGameState = getGameState;
            this.indexOfAllDestinations = indexOfAllDestinations;
            this.indexOfSameID = indexOfSameID;
            logoSR.sprite = id.GetLogo(indexOfSameID);
            logoSR.color = id.Color.ChangeAlpha(logoSR.color.a);
        }

        public virtual IEnumerator Occupy(HanakoEnemy enemy)
        {
            occupationMode = OccupationMode.Enemy;
            currentOccupant = enemy;
            occupantLastPos = enemy.transform.position;
            ChangeColor(colors.OccupiedColor);
            StartCoroutine(MoveOccupant(enemy, postInteractPos.position, durationToPostInteractPos));
            WhenOccupationStart(enemy);

            yield return StartCoroutine(DepletingDuration());

            if (GetGameState != null && GetGameState() == HanakoLevelManager.HanakoGameState.Lost)
                yield break;

            lastOccupant = currentOccupant;
            currentOccupant = null;
            ResetColor();
            StartCoroutine(MoveOccupant(enemy, occupantLastPos, durationToPostInteractPos));
            WhenOccupationEnd(enemy);
            occupationMode = OccupationMode.Unoccupied;

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
            occupant.PlayAnimation(CharacterMotion.Run);
            var speed = Vector2.Distance(transform.position, targetPos) / duration;
            var time = 0f;
            while (time < duration)
            {
                occupant.transform.position = Vector2.MoveTowards(occupant.transform.position, targetPos, speed * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            if (this.currentOccupant == occupant)
                occupant.PlayAnimation(CharacterMotion.Idle);

            OnMoveOccupantEnd?.Invoke(occupant, targetPos);
        }


        public virtual void Hover()
        {
            if (isHovered || occupationMode == OccupationMode.Enemy) return;
            isHovered = true;
            cacheHoveredSRs = new();
            ChangeColor(colors.HoverColor);
            ShowActionIcon();
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

        protected virtual void ShowActionIcon()
        {
            actionIconSR.sprite = icons.ArrownDownIcon;
            actionIconSR.color = colors.PlayerColor;
            actionIconAnimator.SetInteger(int_mode, (int)icons.ArrowDownAnimation);
            actionIconAnimator.SetTrigger(tri_transition);
        }

        protected void HideActionIcon()
        {
            actionIconAnimator.SetInteger(int_mode, (int)icons.HideAnimation);
            actionIconAnimator.SetTrigger(tri_transition);
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
