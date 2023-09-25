using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityUtility;
using static Hanako.Hanako.HanakoEnemySequence;
using static Hanako.Hanako.HanakoLevelManager;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    [RequireComponent(typeof(Collider2D))]
    public class HanakoEnemy : MonoBehaviour
    {
        public enum HighlightMode { None, Detecting, Attackable, Distractable }

        [Header("Initialization")]
        [SerializeField]
        bool autoTransparent = true;

        [Header("Properties")]
        [SerializeField]
        float moveSpeed = 1;

        [SerializeField]
        List<DestinationProperties> destinationSequence = new();

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

        [Header("Rig: Flashlight")]
        [SerializeField]
        ChainIKConstraint flashlightRigChainIK;

        [SerializeField]
        Transform flashlight;

        [SerializeField, ShowIf("@"+nameof(flashlight))]
        Transform flashlightTarget;

        [SerializeField, ShowIf("@" + nameof(flashlight))]
        Transform flashlightPointingPos;

        [SerializeField]
        List<GameObject> gosToDeactivateWhenNotMoving = new();

        [Header("Rig: Eyes")]
        [SerializeField]
        MultiPositionConstraint eyeRigR;

        [SerializeField]
        MultiPositionConstraint eyeRigL;

        [SerializeField]
        Transform eyesTarget;

        [Header("Body Parts")]
        [SerializeField]
        Transform handL;

        [SerializeField]
        Transform handR;

        [Header("Customizations")]
        [SerializeField]
        HanakoColors colors;

        [SerializeField]
        HanakoIcons icons;

        Collider2D col;
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
        Vector2 flashlightAimOriginLocalPos, flashlightOriginLocalPos;

        public HanakoDestination CurrentDestinationPoint { get => currentDestination; }
        public bool IsKillable { get => isKillable; }
        public bool IsAlive { get => isAlive; }
        public List<DestinationProperties> DestinationSequence { get => destinationSequence; }
        public Transform HandL { get => handL; }
        public Transform HandR { get => handR; }

        public event Func<HanakoDestinationID, int, HanakoDestination> GetDestinationByID;
        public event Func<HanakoGameState> GetGameState;
        public event Action OnReachedExitDoor;
        public event Action OnDie;

        void Awake()
        {
            flashlightAimOriginLocalPos = flashlightTarget.localPosition;
            flashlightOriginLocalPos = flashlight.localPosition;
            col = GetComponent<Collider2D>();
            col.enabled = false;

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
                UnholdFlashlight();
                DeactivateGOs(gosToDeactivateWhenNotMoving);
            }

            if (colDetectArea!=null)
                colDetectArea.DisableCollider();

            ResetLook();
        }

        public void Init(
            List<DestinationProperties> destinationSequence,
            HanakoDestination_ExitDoor exitDoor,
            Func<HanakoDestinationID, int, HanakoDestination> getDestinationByID,
            Func<HanakoGameState> getGameState,
            HanakoColors colors,
            HanakoIcons icons,
            Action onDie,
            Action onReachedExitDoor)
        {
            this.destinationSequence = destinationSequence;
            this.exitDoor = exitDoor;
            this.GetDestinationByID = getDestinationByID;
            this.GetGameState = getGameState;
            this.colors = colors;
            this.icons = icons;
            this.OnDie += onDie;
            this.OnReachedExitDoor += onReachedExitDoor;
        }

        public void StartInitialMove()
        {
            col.enabled = true;
            hasInitialMove = true;
            currentDestinationPointIndex = 0;
            currentDestination = GetDestinationByID(destinationSequence[currentDestinationPointIndex].ID, destinationSequence[currentDestinationPointIndex].Index);
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
                currentDestination = GetDestinationByID(destinationSequence[currentDestinationPointIndex].ID, destinationSequence[currentDestinationPointIndex].Index);
            else
            {
                currentDestinationPointIndex = -1;
                currentDestination = exitDoor;
            }

            MoveToCurrentDestination();
        }

        public void MoveTo(HanakoDestination destination)
        {
            if (!isAlive || (GetGameState !=null && GetGameState() != HanakoGameState.Play)) return;

            flashlightTarget.localPosition = flashlightAimOriginLocalPos;
            flashlight.localPosition = flashlightOriginLocalPos;
            corMoving = this.RestartCoroutine(Moving(), corMoving);
            IEnumerator Moving()
            {
                col.enabled = true;
                thoughtBubble.Show(destination.ID.GetLogo(destination.IndexOfSameID), destination.ID.Color);
                animator.SetInteger(int_motion, (int)CharacterMotion.Run);
                ActivateGOs(gosToDeactivateWhenNotMoving);
                HoldFlashlight();
                colDetectArea.EnableCollider();
                isKillable = true;

                // Moving
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

                if (destination.Occupation == HanakoDestination.OccupationMode.Unoccupied)
                {
                    col.enabled = false;
                    Highlight(HighlightMode.None); // in case RemoveEnemyInDetectArea not called because "isKillable = false" below executed first
                    thoughtBubble.Hide();
                    animator.SetInteger(int_motion, (int)CharacterMotion.Idle);
                    UnholdFlashlight();
                    DeactivateGOs(gosToDeactivateWhenNotMoving);
                    colDetectArea.DisableCollider();
                    isKillable = false;

                    yield return StartCoroutine(destination.Occupy(this));
                }

                MoveToNextDestination();
            }
        }

        public void DetectHanako(Vector2 hanakoPos)
        {
            this.StopCoroutineIfExists(corMoving);
            animator.SetInteger(int_motion, (int)CharacterMotion.PointingScared);
            var isFacingRight = hanakoPos.x > transform.position.x;
            transform.localEulerAngles = new(0, isFacingRight ? 0 : 180, 0);
            thoughtBubble.Hide();
            flashlightTarget.position = hanakoPos;
        }

        public void PlayAnimation(CharacterMotion state)
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
            animator.SetInteger(int_motion, (int)CharacterMotion.Pushed);
            colDetectArea.DisableCollider();
            UnholdFlashlight();
            DeactivateGOs(gosToDeactivateWhenNotMoving);

            StartCoroutine(Animating());
            IEnumerator Animating()
            {
                yield return new WaitForSeconds(delayScalingAnimation);
                this.StopCoroutineIfExists(corMoving);
                OnDie?.Invoke();

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
            if (!hasInitialMove || !isAlive || !isKillable) return;

            switch (highlightMode)
            {
                case HighlightMode.None:
                    ResetColor();
                    HideHighlightIcon();
                    break;
                case HighlightMode.Detecting:
                    SetColor(colors.DetectingColor);
                    SetHighlightIcon(icons.WarningIcon, colors.DetectingColor);
                    break;
                case HighlightMode.Attackable:
                    SetColor(colors.AttackableColor);
                    SetHighlightIcon(icons.AttackIcon, colors.AttackableColor);
                    break;                
                case HighlightMode.Distractable:
                    SetColor(colors.AttackableColor);
                    SetHighlightIcon(icons.DistractionIcon, colors.AttackableColor);
                    break;
            }
        }

        void SetColor(Color color)
        {
            colorSetter.ChangeColor(color);
        }

        void ResetColor()
        {
            colorSetter.ResetColorExceptAlpha();
        }

        void SetHighlightIcon(Sprite sprite, Color color)
        {
            highlightIcon.sprite = sprite;
            highlightIcon.color = color.ChangeAlpha(highlightIcon.color.a);
        }

        void HideHighlightIcon()
        {
            highlightIcon.sprite = null;
        }

        public void ReceiveDistraction(Vector2 distractionPos)
        {
            if (!isAlive || !IsKillable) return;
            this.StopCoroutineIfExists(corMoving);
            var isFacingRight = transform.position.x < distractionPos.x;
            transform.localEulerAngles = new(0, isFacingRight ? 0 : 180, 0);
            animator.SetInteger(int_motion, (int)CharacterMotion.Stiffed);

            if (flashlight != null)
            {
                flashlight.position = flashlightPointingPos.position;
                flashlightTarget.position = distractionPos;
            }
        }

        public void ReachedExitDoor(float fadeOutDuration)
        {
            isAlive = false;
            isKillable = false;
            HideHighlightIcon();
            UnholdFlashlight();
            DeactivateGOs(gosToDeactivateWhenNotMoving);
            if (transform.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
                srEditor.BeTransparent(fadeOutDuration);
            OnReachedExitDoor?.Invoke();

            StartCoroutine(Delay(fadeOutDuration));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                animator.SetInteger(int_motion, (int)CharacterMotion.Idle);
                gameObject.SetActive(false);
            }
        }

        public void LookAt(Transform target)
        {
            eyeRigL.weight = 1f;
            eyeRigR.weight = 1f;

        }

        public void ResetLook()
        {
            eyeRigL.weight = 0f;
            eyeRigR.weight = 0f;
        }

        public void HoldFlashlight()
        {
            flashlightRigChainIK.weight = 1f;
            flashlight.gameObject.SetActive(true);
        }

        public void UnholdFlashlight()
        {
            flashlightRigChainIK.weight = 0f;
            flashlight.gameObject.SetActive(false);
        }
    }
}
