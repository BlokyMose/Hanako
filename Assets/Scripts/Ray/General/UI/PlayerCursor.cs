using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityUtility;
using static Hanako.Knife.KnifeCursor;

namespace Hanako
{
    public class PlayerCursor : MonoBehaviour
    {
        public enum CursorInputMode { MousePosition, InputDelta }

        [SerializeField]
        protected CursorInputMode cursorInputMode = CursorInputMode.MousePosition;

        [SerializeField, ShowIf(nameof(cursorInputMode))]
        protected float cursorSpeed = 1f;

        [SerializeField, ShowIf(nameof(cursorInputMode))]
        protected bool isFollowingMouse = true;

        [SerializeField]
        protected VisualEffect bloodBurst;

        [Header("SFX")]
        [SerializeField]
        protected AudioSourceRandom audioSource;

        [SerializeField]
        string sfxClickName = "sfxClick";

        [SerializeField]
        protected AudioSource bloodBurstAudioSource;

        protected Animator animator;

        protected Rigidbody2D rb;
        protected Collider2D col;
        protected int boo_isClick, flo_moveByX;
        protected bool isClicking;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            col.isTrigger = true;
            audioSource = audioSource == null ? GetComponent<AudioSourceRandom>() : audioSource;
            bloodBurstAudioSource = bloodBurstAudioSource == null ? bloodBurst.GetComponent<AudioSource>() : bloodBurstAudioSource;

            boo_isClick = Animator.StringToHash(nameof(boo_isClick));
            flo_moveByX = Animator.StringToHash(nameof(flo_moveByX));
        }
        protected virtual void OnDestroy()
        {
            
        }


        protected virtual void OnEnable()
        {
            transform.position = Vector2.zero;

            var playerInput = FindAnyObjectByType<PlayerInputHandler>();
            if (playerInput != null)
            {
                playerInput.HideMouseCursor();
                playerInput.OnClickInput += Click;
                playerInput.OnClickStateInput += ClickState;
                if (cursorInputMode == CursorInputMode.InputDelta)
                {
                    playerInput.OnCursorInput += Move;
                    playerInput.SetMouseCursorToCenter();
                }
            }
            else
            {
                Debug.LogWarning("Cannot find Player Input Handler");
            }


            StartCoroutine(Delay(0.1f));
            IEnumerator Delay(float delay)
            {
                while (true)
                {
                    var previousPos = transform.position;
                    yield return new WaitForSeconds(delay);
                    animator.SetFloat(flo_moveByX, transform.position.x - previousPos.x);
                }
            }
        }

        protected virtual void OnDisable()
        {
            var playerInput = FindAnyObjectByType<PlayerInputHandler>();
            if (playerInput != null)
            {
                playerInput.OnClickInput -= Click;
                playerInput.OnClickStateInput -= ClickState;
                if (cursorInputMode == CursorInputMode.InputDelta)
                {
                    playerInput.OnCursorInput -= Move;
                }

            }
            else
            {
                Debug.LogWarning("Cannot find Player Input Handler when disabling");
            }
        }

        protected virtual void FixedUpdate()
        {
            if (cursorInputMode == CursorInputMode.MousePosition && isFollowingMouse)
            {
                var targetPos = Camera.main.ScreenToWorldPoint(new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0));
                transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * cursorSpeed);
            }
        }
        public virtual void Move(Vector2 moveBy)
        {
            transform.position += (Vector3)moveBy;
        }

        public virtual void Click()
        {
            audioSource.PlayOneClipFromPack(sfxClickName);
        }

        public virtual void ClickState(bool isClicking)
        {
            this.isClicking = isClicking;
            animator.SetBool(boo_isClick, isClicking);
            bloodBurst.SetBool("isPlaying", isClicking);
            if (isClicking)
                bloodBurstAudioSource.Play();
            else
                bloodBurstAudioSource.Stop();
        }


    }
}
