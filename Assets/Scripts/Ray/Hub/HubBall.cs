using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hub
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HubBall : MonoBehaviour
    {
        [SerializeField]
        float rollSpeed = 1f;

        [SerializeField]
        Transform ballSR;

        Rigidbody2D rb;
        int isRollToLeft;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            isRollToLeft = collision.transform.position.x > transform.position.x ? 1 : -1;
        }

        void Update()
        {
            ballSR.localEulerAngles = new(0, 0, ballSR.localEulerAngles.z + isRollToLeft * rb.velocity.magnitude * rollSpeed);
        }

    }
}
