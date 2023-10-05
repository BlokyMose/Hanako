using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class PushForce : MonoBehaviour
    {
        public float bounceForce = 10.0f; // ”½”­‚Ì—Í

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}
