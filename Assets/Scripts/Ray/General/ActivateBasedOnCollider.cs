using Hanako.Hub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class ActivateBasedOnCollider : MonoBehaviour
    {
        [SerializeField]
        Collider2D col;

        [SerializeField]
        List<GameObject> gos;

        private void Awake()
        {
            if(col == null)
                col = GetComponent<Collider2D>();

            foreach (var go in gos)
                go.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<HubCharacterBrain_Player>(out var player))
            {
                foreach (var go in gos)
                    go.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<HubCharacterBrain_Player>(out var player))
            {
                foreach (var go in gos)
                    go.SetActive(false);
            }
        }
    }
}
