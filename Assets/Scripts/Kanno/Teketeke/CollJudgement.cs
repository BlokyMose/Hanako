using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class CollJudgement : MonoBehaviour
    {
       
        private void Start()
        {
           
            
        }

        private void Update()
        {
           
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
               
            }

        }
    }
}
