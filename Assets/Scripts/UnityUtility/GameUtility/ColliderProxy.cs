using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public class ColliderProxy : MonoBehaviour
    {
        public Action<Collider2D> OnEnter;
        public Action<Collider2D> OnExit;
        public Action<Collision2D> OnCollide;
        public Action<Collision2D> OnCollideExit;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnEnter?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            OnExit?.Invoke(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollide?.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            OnCollideExit?.Invoke(collision);
        }
    }
}
