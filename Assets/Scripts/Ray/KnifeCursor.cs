using Hanako.Knife;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityUtility;

namespace Hanako.Knife
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]
    public class KnifeCursor : MonoBehaviour
    {
        public enum CursorInputMode { MousePosition, InputDelta }

        [SerializeField]
        CursorInputMode cursorInputMode = CursorInputMode.MousePosition;

        [SerializeField, ShowIf(nameof(cursorInputMode))]
        float cursorSpeed = 1f;

        [SerializeField, ShowIf(nameof(cursorInputMode))]
        bool isFollowingMouse = true;

        [SerializeField]
        VisualEffect bloodBurst;

        Animator animator;
        Rigidbody2D rb;
        Collider2D col;
        int boo_isClick, flo_moveByX;
        KnifeTile hoveredTile;
        bool isClicking;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            col.isTrigger = true;

            boo_isClick = Animator.StringToHash(nameof(boo_isClick));
            flo_moveByX = Animator.StringToHash(nameof(flo_moveByX));
        }

        private void OnEnable()
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

        private void OnDisable()
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

        private void Update()
        {
            if(cursorInputMode == CursorInputMode.MousePosition && isFollowingMouse)
            {
                var targetPos = Camera.main.ScreenToWorldPoint(new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0));
                transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * cursorSpeed);
            }
        }

        public void Move(Vector2 moveBy)
        {
            transform.position += (Vector3) moveBy;
        }

        public void Click()
        {
            
        }

        public void ClickState(bool isClicking)
        {
            this.isClicking = isClicking;
            animator.SetBool(boo_isClick, isClicking);
            bloodBurst.SetBool("isPlaying", isClicking);

            if (hoveredTile != null)
            {
                if (isClicking)
                    hoveredTile.Clicked();
                else
                    hoveredTile.Unhovered();
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isClicking) return;

            if (collision.TryGetComponentInFamily<KnifeTile>(out var tile))
            {
                if (hoveredTile != null)
                    hoveredTile.Unhovered();

                hoveredTile = tile;
                hoveredTile.Hovered();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponentInFamily<KnifeTile>(out var tile) && hoveredTile == tile)
            {
                hoveredTile.Unhovered();
                hoveredTile = null;
            }
        }

    }
}
