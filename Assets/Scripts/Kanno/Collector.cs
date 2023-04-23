using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Collector : MonoBehaviour
    {
       

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("BG"))
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
}
