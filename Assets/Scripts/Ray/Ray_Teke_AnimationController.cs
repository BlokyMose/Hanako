using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [RequireComponent(typeof(Animator))]
    public class Ray_Teke_AnimationController : MonoBehaviour
    {
        Animator animator;
        int boo_isJumping, tri_attack, tri_damaged;

        void Awake()
        {
            animator = GetComponent<Animator>();
            boo_isJumping = Animator.StringToHash(nameof(boo_isJumping));
            tri_attack = Animator.StringToHash(nameof(tri_attack));
            tri_damaged = Animator.StringToHash(nameof(tri_damaged));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetBool(boo_isJumping,true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger(tri_attack);
            }
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground"))
            {
                animator.SetBool(boo_isJumping,false);
            }

            if (collision.CompareTag("desk") || collision.CompareTag("chair") || collision.CompareTag("lectern"))
            {
                animator.SetTrigger(tri_damaged);
            }
        }

    }
}
