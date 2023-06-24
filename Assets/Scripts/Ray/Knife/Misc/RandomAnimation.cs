using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class RandomAnimation : MonoBehaviour
    {
        [Header("Rotation")]
        public float rotSpeed = 1f;

        [Header("Scale")]
        public float scaleSpeed = 0.01f;
        public float scaleMax = 2f;
        public float scaleMin = 0.1f;

        private void Start()
        {
            StartCoroutine(Rotating());
            IEnumerator Rotating()
            {
                while (true)
                {
                    transform.localEulerAngles = new(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + rotSpeed * Time.deltaTime);
                    yield return null;
                }
            }

            StartCoroutine(Scaling());
            IEnumerator Scaling()
            {
                while (true)
                {
                    while (true)
                    {
                        if (transform.localScale.x < scaleMax)
                            transform.localScale = new(transform.localScale.x + scaleSpeed, transform.localScale.y + scaleSpeed);
                        else
                            break;
                        yield return null;
                    }

                    while (true)
                    {
                        if (transform.localScale.x > scaleMin)
                            transform.localScale = new(transform.localScale.x - scaleSpeed, transform.localScale.y - scaleSpeed);
                        else
                            break;
                        yield return null;
                    }
                    yield return null;
                }

            }
        }

    }
}
