using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class PlayerJumpController : MonoBehaviour
    {

        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private int jumpForce;

        private int jumpCount = 0;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && this.jumpCount < 1)
            {
                Jump();
                jumpCount++;
            }
        }

        void Jump()
        {
            
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground"))
            {
                jumpCount = 0;
            }
        }
    }
}
