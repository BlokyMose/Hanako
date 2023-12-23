using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class EnableSRWhenRotate : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer sr;

        [SerializeField]
        Transform targetTransform;

        [SerializeField]
        float displayOnY = 0;

        void Awake()
        {
            if (sr == null)
                sr = GetComponent<SpriteRenderer>();
            if (targetTransform == null)
                targetTransform = transform;
        }

        void Update()
        {
            sr.enabled = targetTransform.localEulerAngles.y == displayOnY;
        }
    }
}
