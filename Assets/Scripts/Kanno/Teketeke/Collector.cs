using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Collector : MonoBehaviour
    {
       
        //�I�u�W�F�N�g������
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Ground") ||collision.CompareTag("BG") || collision.CompareTag("desk") || collision.CompareTag("chair") || collision.CompareTag("lectern")|| collision.CompareTag("student"))
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
}
