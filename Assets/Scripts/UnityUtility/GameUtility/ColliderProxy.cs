using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    [RequireComponent(typeof(Collider2D))]
    public class ColliderProxy : MonoBehaviour
    {
        public Action<Collider2D> OnEnter;
        public Action<Collider2D> OnExit;
        public Action<Collision2D> OnCollide;
        public Action<Collision2D> OnCollideExit;
        Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        public void EnableCollider()
        {
            if (col == null) Awake();
            col.enabled = true;
        }

        public void DisableCollider()
        {
            if (col == null) Awake();
            col.enabled = false;
        }

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
