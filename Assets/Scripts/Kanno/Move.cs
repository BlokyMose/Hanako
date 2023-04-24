using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Move : MonoBehaviour
    {

        [SerializeField] private float moveSpeed = 8f;
        private float startSpeed;

        // Start is called before the first frame update
        void Start()
        {
            startSpeed = moveSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }

        private void FixedUpdate()
        {
            MoveCharactor();
        }

        private void MoveCharactor()
        {
            this.transform.Translate(moveSpeed, 0, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("desk"))
            {
                // ë¨ìxÇÇRï™ÇÃÇPÇ…Ç∑ÇÈÅB
                moveSpeed = moveSpeed * 0.3f;

                Invoke("SpeedReset", 2.0f);
            }
            
            if (other.gameObject.CompareTag("chair"))
            {
                // ë¨ìxÇÇRï™ÇÃÇPÇ…Ç∑ÇÈÅB
                moveSpeed = moveSpeed * 0.3f;

                Invoke("SpeedReset", 1.0f);
            }
            if (other.gameObject.CompareTag("lectern"))
            {
                // ë¨ìxÇÇRï™ÇÃÇPÇ…Ç∑ÇÈÅB
                moveSpeed = moveSpeed * 0.3f;

                Invoke("SpeedReset", 3.0f);
            }





        }

        void SpeedReset()
        {
            // ë¨ìxÇå≥Ç…ñﬂÇ∑ÅB
            moveSpeed = startSpeed;
        }
    }
}
