using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public class AlwaysRotateAbsolute : MonoBehaviour
    {
        public Vector3 eulerAngles = new();

        void Update()
        {
            transform.eulerAngles = eulerAngles;
        }
    }
}
