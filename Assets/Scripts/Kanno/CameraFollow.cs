using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class CameraFollow : MonoBehaviour
    {

        private Transform playerPos;

        [SerializeField]private float offSetX = -6f;

        private Vector3 tempPos;
        // Start is called before the first frame update

        private void Awake()
        {
            FindPlayer();
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void LateUpdate()
        {
            FollowPlayer();
        }

        private void FindPlayer()
        {
            playerPos = GameObject.FindWithTag("Player").transform;
        }

        private void FollowPlayer()
        {
            if (playerPos)
            {
                tempPos = transform.position;
                tempPos.x = playerPos.position.x - offSetX;

                transform.position = tempPos;
            }
        }
    }
}
