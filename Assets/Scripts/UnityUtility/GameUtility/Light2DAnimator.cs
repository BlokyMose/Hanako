using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityUtility
{
    public class Light2DAnimator : MonoBehaviour
    {
        class LightProperties
        {
            Light2D light;
            float originalIntensity;

            public LightProperties(Light2D light, float originalIntensity)
            {
                this.light = light;
                this.originalIntensity = originalIntensity;
            }

            public Light2D Light { get => light; }
            public float OriginalIntensity { get => originalIntensity; }
        }

        [SerializeField]
        List<Light2D> targetLights = new();
        List<LightProperties> lights = new();
        Coroutine corAnimating;

        void Awake()
        {
            foreach (var targetLight in targetLights)
                lights.Add(new(targetLight, targetLight.intensity));
        }

        public void BeOpaque(float duration)
        {
            corAnimating = this.RestartCoroutine(ChangeIntensity(true, duration), corAnimating);
        }

        public void BeTransparent(float duration)
        {
            corAnimating = this.RestartCoroutine(ChangeIntensity(false, duration), corAnimating);
        }

        public void BeOpaque()
        {
            this.StopCoroutineIfExists(corAnimating);
            foreach (var light in lights)
                light.Light.intensity = light.OriginalIntensity;
        }

        public void BeTransparent()
        {
            this.StopCoroutineIfExists(corAnimating);
            foreach (var light in lights)
                light.Light.intensity = 0f;
        }

        IEnumerator ChangeIntensity(bool beOpaque, float duration)
        {
            var lightCurves = new List<AnimationCurve>();
            foreach (var light in lights)
                lightCurves.Add(AnimationCurve.Linear(0, light.Light.intensity, duration, beOpaque ? light.OriginalIntensity : 0));

            var time = 0f;
            while (time < duration)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    var light = lights[i];
                    var curve = lightCurves[i];
                    light.Light.intensity = curve.Evaluate(time);
                }
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
