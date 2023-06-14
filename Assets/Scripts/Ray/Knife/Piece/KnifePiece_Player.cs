using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_Player : KnifePiece_Living
    {
        [Header("Animation Duration")]
        [SerializeField]
        float attackDuration = 0.5f;

        public void Attack()
        {
            var currentState = animator.GetInteger(int_motion);
            animator.SetInteger(int_motion, (int)PieceAnimationState.Attack);

            StartCoroutine(Delay(attackDuration-0.05f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                animator.SetInteger(int_motion, currentState);
            }
        }
    }
}
