using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class TekeTeke_Move : MonoBehaviour
    {

        [SerializeField] private float moveSpeed = 0.5f;
        private float startSpeed;
        [SerializeField]  private float changeSpeed = 0.1f;

        public float span = 3f;
        private float currentTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            startSpeed = moveSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            //ˆê’èŠÔ‚²‚Æ‚É‘¬“xã¸
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

            currentTime += Time.deltaTime;
            if(moveSpeed <= 1 )
            {
                if (currentTime > span)
                {
                    moveSpeed += changeSpeed;
                    currentTime = 0f;
                }
            }
            
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
                // ‘¬“x‚ğ‚R•ª‚Ì‚P‚É‚·‚éB
                moveSpeed = moveSpeed * 0.3f;

                Invoke("SpeedReset", 2.0f);
            }
            
            if (other.gameObject.CompareTag("chair"))
            {
                // ‘¬“x‚ğ‚R•ª‚Ì‚P‚É‚·‚éB
                moveSpeed = moveSpeed * 0.3f;

                Invoke("SpeedReset", 1.0f);
            }
            if (other.gameObject.CompareTag("lectern"))
            {
                // ‘¬“x‚ğ‚R•ª‚Ì‚P‚É‚·‚éB
                moveSpeed = moveSpeed * 0.3f;

                Invoke("SpeedReset", 3.0f);
            }





        }

        void SpeedReset()
        {
            // ‘¬“x‚ğŒ³‚É–ß‚·B
            moveSpeed = startSpeed;
        }
    }
}
