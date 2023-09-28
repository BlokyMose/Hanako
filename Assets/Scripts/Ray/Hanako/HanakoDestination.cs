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

        [SerializeField, ReadOnly]
        int indexOfAllDestinations;

        [SerializeField, ReadOnly]
        int indexOfSameID;

        [Header("Interact")]
        [SerializeField]
        protected Transform interactablePos;

        [SerializeField]
        protected Transform postInteractPos;

        [SerializeField]
        protected float durationToPostInteractPos = 0.1f;

        [SerializeField]
        protected float interactDuration = 1f;

        [Header("UI")]

        [SerializeField]
        SpriteRenderer logoSR;

        [SerializeField]
        protected GameObject destinationUIPrefab;
        
        [SerializeField, ShowIf("@" + nameof(destinationUIPrefab))]
        protected Transform destinationUIParent;
        

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
        protected bool isHovering = false;
        protected Vector2 occupantLastPos;
        protected OccupationMode occupationMode = OccupationMode.Unoccupied;
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
        public Transform PostInteractPos => postInteractPos == null ? transform : postInteractPos;
        public virtual OccupationMode Occupation { get => occupationMode; }
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

            foreach (var sr in coloredSRsByID)
                sr.color = id.Color.ChangeAlpha(sr.color.a);

            if (postInteractPos == null)
                postInteractPos = transform;
        }

        protected virtual void OnDestroy()
        {
            if (destinationUIPrefab != null)
                destinationUI.Exit(ref OnDurationStartDepleting, ref OnDurationStartFilling, ref OnOccupationStart, ref OnOccupationEnd);
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
            yield return StartCoroutine(MoveOccupant(enemy, occupantLastPos, durationToPostInteractPos));
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
            const float animationDelay = 0.15f;
            yield return new WaitForSeconds(animationDelay);

            var curveX = AnimationCurve.Linear(0, occupant.transform.position.x, duration, targetPos.x);
            var curveY = AnimationCurve.Linear(0, occupant.transform.position.y, duration, targetPos.y);
            var time = 0f;
            while (time < duration)
            {
                occupant.transform.position = new(curveX.Evaluate(time), curveY.Evaluate(time));
                time += Time.deltaTime;
                yield return null;
            }
            occupant.PlayAnimation(CharacterMotion.Idle);
            OnMoveOccupantEnd?.Invoke(occupant, targetPos);
        }


        protected void ChangeColor(Color color)
        {
            colorSetter.ChangeColor(color);
        }

        protected void ResetColor()
        {
            colorSetter.ResetColor();
        }
    }
}
