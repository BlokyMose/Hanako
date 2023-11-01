using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityUtility;

namespace Hanako.Hub
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class HubCharacterController : MonoBehaviour
    {
        public enum HandsScaleState { One, Double }

        [SerializeField]
        CharID charID;

        [SerializeField, SuffixLabel("x1000")]
        float walkSpeed = 2f;
        
        [SerializeField, SuffixLabel("x1000")]
        float runSpeed = 3f;


        [Header("Rigs")]
        [SerializeField]
        string handsAnimatorLayerName = "hands";

        [SerializeField]
        Transform handsUpTarget;

        [SerializeField]
        Rig handsUpRig;

        [Header("Minimap")]
        [SerializeField]
        HubMinimapIcon minimapIconPrefab;

        [SerializeField]
        Transform minimapIconParent;


        Animator animator;
        int int_motion, int_hands, boo_isRunning;
        const float WALK_SPEED_MULTIPLIER = 1000;
        bool isRunning;
        float currentWalkSpeed;
        Vector2 moveDirection;
        Rigidbody2D rb;
        Collider2D col;


        private void Awake()
        {
            animator = gameObject.GetComponentInFamily<Animator>();
            int_motion = Animator.StringToHash(nameof(int_motion));
            int_hands = Animator.StringToHash(nameof(int_hands));
            boo_isRunning = Animator.StringToHash(nameof(boo_isRunning));
            animator.SetLayerWeight(animator.GetLayerIndex(handsAnimatorLayerName), 1f);

            rb = gameObject.GetComponent<Rigidbody2D>();
            col = gameObject.GetComponent<Collider2D>();
            handsUpRig.weight = 0;

            var minimapIcon = Instantiate(minimapIconPrefab, minimapIconParent);
            minimapIcon.Init(charID.Icon);

            currentWalkSpeed = walkSpeed;
        }

        public void Init(HubCharacterBrain_Player brain)
        {
            brain.OnMove += Move;
            brain.OnRun += Run;
        }

        private void Update()
        {
            rb.AddForce(moveDirection * currentWalkSpeed * WALK_SPEED_MULTIPLIER * Time.deltaTime);
        }

        void Move(Vector2 direction)
        {
            moveDirection = direction;
            if (direction.x > 0)
                transform.localEulerAngles = new (transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
            else if (direction.x < 0)
                transform.localEulerAngles = new(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);

            if (direction == Vector2.zero)
                animator.SetInteger(int_motion, (int)CharacterMotion.Idle);
            else
                animator.SetInteger(int_motion, (int)CharacterMotion.Run);
        }

        void Run(bool isRunning)
        {
            this.isRunning = isRunning;
            currentWalkSpeed = isRunning ? runSpeed : walkSpeed;
            animator.SetBool(boo_isRunning, isRunning);
        }

        public Transform test;
        [Button]
        public void Test()
        {
            HoldUp(test);
        }

        Transform heldObject;
        public void HoldUp(Transform target)
        {
            heldObject = target;
            target.parent = handsUpTarget;
            target.localPosition = Vector2.zero;
            target.localEulerAngles = Vector2.zero;
            animator.SetInteger(int_hands, (int)HandsScaleState.Double);

            if (target.TryGetComponentInFamily<Collider2D>(out var targetCol))
            {
                targetCol.enabled = false;
            }            
            
            if (target.TryGetComponentInFamily<Rigidbody2D>(out var targetRB))
            {
                targetRB.isKinematic = true;
            }

            handsUpRig.weight = 1;
        }

        [Button]
        public void Throw()
        {
            heldObject.parent = null;
            heldObject.localEulerAngles = Vector2.zero;
            animator.SetInteger(int_hands, (int)HandsScaleState.One);


            if (heldObject.TryGetComponentInFamily<Collider2D>(out var targetCol))
            {
                targetCol.enabled = true;
            }

            if (heldObject.TryGetComponentInFamily<Rigidbody2D>(out var targetRB))
            {
                targetRB.isKinematic = false;
            }

            handsUpRig.weight = 0f;
        }

    }
}
