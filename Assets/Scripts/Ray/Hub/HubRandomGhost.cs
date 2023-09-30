using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Hanako.Hub
{
    public class HubRandomGhost : MonoBehaviour
    {
        class LightData
        {
            Light2D light;
            float intensity;

            public LightData(Light2D light, float intensity)
            {
                this.light = light;
                this.intensity = intensity;
            }

            public Light2D Light { get => light; }
            public float Intensity { get => intensity; }
        }

        [SerializeField]
        Vector2 moveDirection = new(1,0);

        [SerializeField]
        float moveSpeed = 1f;

        [SerializeField]
        float fadeIn = 1f;

        [SerializeField]
        float visibleDuration = 2f;

        [SerializeField]
        float fadeOut = 1f;

        [Header("Components")]
        [SerializeField]
        SpriteRendererEditor srEditor;

        [SerializeField]
        List<Light2D> lights = new();
        List<LightData> lightData = new();

        void Start()
        {
            lightData = new();
            foreach (var light in lights)
            {
                lightData.Add(new(light, light.intensity));
                light.intensity = 0f;
            }

            StartCoroutine(AnimatingAplha());
            IEnumerator AnimatingAplha()
            {
                srEditor.BeOpaqueFromTransparent(fadeIn);
                var curves = new List<AnimationCurve>();
                foreach (var light in lightData)
                    curves.Add(AnimationCurve.Linear(0, 0f, fadeIn, light.Intensity));

                var time = 0f;
                while (time < fadeIn)
                {
                    for (int i = 0; i < lightData.Count; i++)
                        lightData[i].Light.intensity = curves[i].Evaluate(time);
                    time += Time.deltaTime;
                    yield return null;
                }

                yield return new WaitForSeconds(visibleDuration);

                srEditor.BeTransparent(fadeOut);
                curves = new List<AnimationCurve>();
                foreach (var light in lightData)
                    curves.Add(AnimationCurve.Linear(0, light.Light.intensity, fadeOut, 0f));

                time = 0f;
                while (time < fadeOut)
                {
                    for (int i = 0; i < lightData.Count; i++)
                        lightData[i].Light.intensity = curves[i].Evaluate(time);
                    time += Time.deltaTime;
                    yield return null;
                }

                Destroy(gameObject, 1f);
            }
        }

        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + moveDirection, Time.deltaTime * moveSpeed);
        }
    }
}
