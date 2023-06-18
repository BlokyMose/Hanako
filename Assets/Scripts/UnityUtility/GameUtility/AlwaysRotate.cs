using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public class AlwaysRotate : MonoBehaviour
    {
        public Vector3 localEulerAngles = new();

        void Update()
        {
            transform.localEulerAngles = localEulerAngles;
        }
    }
}
