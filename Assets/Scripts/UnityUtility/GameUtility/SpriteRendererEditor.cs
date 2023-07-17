using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityUtility;

namespace Hanako
{
    public class SpriteRendererEditor : MonoBehaviour
    {
        public List<SpriteRenderer> srs = new();

        [Button("Get All SRs")]
        public void GetAllSpriteRenderersInChildren()
        {
            srs.AddRange(transform.GetComponentsInChildren<SpriteRenderer>());
        }

        public void BeTransparent(float duration)
        {
            ChangeAlpha(duration, 0f);
        }

        public void BeTransparentFromOpaque(float duration)
        {
            ChangeAlpha(duration, 0f, 1f);
        }

        public void BeOpaque(float duration)
        {
            ChangeAlpha(duration, 1f);
        }

        public void BeOpaqueFromTransparent(float duration)
        {
            ChangeAlpha(duration, 1f, 0f);
        }

        /// <summary>
        /// The parameters are duration, followed by a semi-colon, then the alpha value<br></br>
        /// Example: "0.15;1"
        /// </summary>
        public void ChangeAlpha(string parameters)
        {
            var parametersSplitted = parameters.Split(';');
            if (float.TryParse(parametersSplitted[0], out float duration))
            {
                if (float.TryParse(parametersSplitted[1], out float alpha))
                {
                    ChangeAlpha(duration, alpha);
                }
            }

            Debug.LogWarning("Failed to change alpha because the string format is wrong; it should be 'duration;alpha'");
        }

        public void ChangeAlpha(float duration, float alpha, float? alphaOrigin = null)
        {
            StopAllCoroutines();
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                var curve = AnimationCurve.EaseInOut(0, alphaOrigin == null ? srs[0].color.a : (float)alphaOrigin, duration, alpha);
                var time = 0f;
                while (true)
                {
                    foreach (var sr in srs)
                    {
                        sr.color = sr.color.ChangeAlpha(curve.Evaluate(time));
                    }
                    time += Time.deltaTime;

                    if (time > duration) break;
                    yield return null;
                }
            }
        }

        public void ChangeAlpha(float alpha)
        {
            foreach (var sr in srs)
            {
                sr.color = sr.color.ChangeAlpha(alpha);
            }
        }
    }
}
