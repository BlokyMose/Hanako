using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [RequireComponent(typeof(Collider2D))]
    public class MoveStimulator : MonoBehaviour
    {
        [SerializeField]
        HanakoStudent student;

        Collider2D col;
        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private void OnMouseDown()
        {
            if(student != null)
                student.SetMoveTarget(transform);
        }
    }
}
