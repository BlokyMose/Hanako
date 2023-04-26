using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class StudentMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;

       

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            MoveStudent();
        }

        private void MoveStudent()
        {
            this.transform.Translate(moveSpeed, 0, 0);
        }
    }
}
