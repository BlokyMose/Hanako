using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityUtility;

namespace Hanako.Hub
{
    public class HubDoorLight : MonoBehaviour
    {
        public enum State { Far, Close }

        [System.Serializable]
        public class LightProperties
        {
            [SerializeField]
            Light2D light;

            [SerializeField]
            float intensityFar = 0.25f;

            [SerializeField]
            float intensityClose = 1.2f;

            [SerializeField]
            float animateIntensityRange = 0.25f;

            public Light2D Light { get => light; }
            public float IntensityFar { get => intensityFar; }
            public float IntensityClose { get => intensityClose; }
            public float AnimateIntensityRange { get => animateIntensityRange; }

            public float GetIntensity(State state)
            {
                return state switch
                {
                  State.Close => intensityClose,
                  State.Far => intensityFar,
                    _ => intensityFar
                };
            }
        } 
        
        [System.Serializable]
        public class SRProperties
        {
            [SerializeField]
            SpriteRenderer sr;

            [SerializeField]
            float alphaFar = 0.25f;

            [SerializeField]
            float alphaClose = 1.2f;

            [SerializeField]
            float animateAlphaRange = 0.25f;

            public SpriteRenderer SR { get => sr; }
            public float AlphaFar { get => alphaFar; }
            public float AlphaClose { get => alphaClose; }
            public float AnimateAlphaRange { get => animateAlphaRange; }


            public float GetAlpha(State state)
            {
                return state switch
                {
                    State.Close => alphaClose,
                    State.Far => alphaFar,
                    _ => alphaFar
                };
            }
        }

        [SerializeField]
        float transitionDuration = 0.33f;

        [SerializeField]
        float loopDuration = 1f;

        [SerializeField]
        List<LightProperties> lights = new();

        [SerializeField]
        List<SRProperties> srs = new();
        Coroutine corAnimating;

        private void OnEnable()
        {
            if (TryGetComponent<HubLevelDoor>(out var door))
            {
                door.OnShowLevelInfoPreview += () => Play(State.Close);
                door.OnHideLevelInfoPreview += () => Play(State.Far);
            }

            Play(State.Far);
        }

        private void OnDisable()
        {
            if (TryGetComponent<HubLevelDoor>(out var door))
            {
                door.OnShowLevelInfoPreview -= () => Play(State.Close);
                door.OnHideLevelInfoPreview -= () => Play(State.Far);
            }
        }


        public void Play(State state)
        {
            corAnimating = this.RestartCoroutine(Animating(state), corAnimating);
            IEnumerator Animating(State state)
            {
                var isAnimatingForward = true;
                var lightCurves = new List<AnimationCurve>();
                var srCurves = new List<AnimationCurve>();

                ResetAnimationCurves(isAnimatingForward, transitionDuration);
                var time = 0f;
                while (time < transitionDuration)
                {
                    EvaluateAnimationCurves(lightCurves, srCurves, time);
                    time += Time.deltaTime;
                    yield return null;
                }

                ResetAnimationCurves(isAnimatingForward, loopDuration);
                time = 0f;
                while (true)
                {
                    EvaluateAnimationCurves(lightCurves, srCurves, time);
                    time += Time.deltaTime;

                    if (time > loopDuration)
                    {
                        isAnimatingForward = !isAnimatingForward;
                        time = 0f;
                        ResetAnimationCurves(isAnimatingForward, loopDuration);
                    }

                    yield return null;
                }

                void ResetAnimationCurves(bool isAnimatingForward, float duration)
                {
                    lightCurves = new List<AnimationCurve>();
                    srCurves = new List<AnimationCurve>();

                    foreach (var light in lights)
                        lightCurves.Add(AnimationCurve.Linear(0, light.Light.intensity, duration, light.GetIntensity(state) + (isAnimatingForward ? light.AnimateIntensityRange : 0)));

                    foreach (var sr in srs)
                        srCurves.Add(AnimationCurve.Linear(0, sr.SR.color.a, duration, sr.GetAlpha(state) + (isAnimatingForward ? sr.AnimateAlphaRange : 0)));
                }

                void EvaluateAnimationCurves(List<AnimationCurve> lightCurves, List<AnimationCurve> srCurves, float time)
                {
                    for (int i = 0; i < lights.Count; i++)
                    {
                        var light = lights[i];
                        var curve = lightCurves[i];
                        light.Light.intensity = curve.Evaluate(time);
                    }

                    for (int i = 0; i < srs.Count; i++)
                    {
                        var sr = srs[i];
                        var curve = srCurves[i];
                        sr.SR.color = sr.SR.color.ChangeAlpha(curve.Evaluate(time));
                    }
                }
            }
        }

    }
}
