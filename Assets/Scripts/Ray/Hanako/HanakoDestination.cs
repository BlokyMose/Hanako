using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    public class HanakoDestination : MonoBehaviour
    {
        [SerializeField]
        HanakoLevelManager levelManager;

        [SerializeField]
        Transform position;

        public Transform Position => position == null ? transform : position;


        public void Init(HanakoLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }
    }
}
