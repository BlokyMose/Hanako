using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class StudentMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;

        GameManager sManager;

        // Start is called before the first frame update
        void Start()
        {
            sManager = GameObject.Find("TekeTeke").GetComponent<GameManager>();
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            sManager.SetScore();
        }
    }
}
