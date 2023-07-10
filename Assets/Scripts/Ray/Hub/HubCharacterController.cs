using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hub
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class HubCharacterController : MonoBehaviour
    {
        public enum PieceAnimationState { Die = -1, Idle, Run, Attack }

        [SerializeField]
        float speed = 1f;

        Animator animator;
        int int_motion;
        Vector2 moveDirection;
        Rigidbody2D rb;
        Collider2D col;

        private void Awake()
        {
            animator = gameObject.GetComponentInFamily<Animator>();
            int_motion = Animator.StringToHash(nameof(int_motion));
            rb = gameObject.GetComponent<Rigidbody2D>();
            col = gameObject.GetComponent<Collider2D>();
        }

        public void Init(HubCharacterBrain_Player brain)
        {
            brain.OnMove += Move;
        }

        private void Update()
        {
            rb.AddForce(moveDirection * speed * Time.deltaTime);
        }

        void Move(Vector2 direction)
        {
            moveDirection = direction;
            if (direction.x > 0)
                transform.localEulerAngles = new (transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
            else if (direction.x < 0)
                transform.localEulerAngles = new(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);

            if (direction == Vector2.zero)
            {
                animator.SetInteger(int_motion, (int)PieceAnimationState.Idle);
            }
            else
            {
                animator.SetInteger(int_motion, (int)PieceAnimationState.Run);
            }
        }

    }
}
