using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Attack : MonoBehaviour
    {
       [SerializeField] private Collider2D  coll;

        // Start is called before the first frame update
        void Start()
        {
            coll.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            


            if (Input.GetMouseButtonDown(0))
            {
                coll.enabled = true;
                Invoke(nameof(AttackInvalid), 1f);
            }

        }

        private void AttackInvalid()
        {
            coll.enabled = false;
        }

    }
}
