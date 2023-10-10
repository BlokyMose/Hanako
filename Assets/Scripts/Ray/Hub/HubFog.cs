using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using Random = UnityEngine.Random;

namespace Hanako.Hub
{
    [RequireComponent(typeof(SpriteRendererEditor), typeof(Light2DAnimator))]
    public class HubFog : MonoBehaviour
    {
        [HorizontalGroup("moveSpeed"), SerializeField, LabelWidth(85f)]
        float moveSpeed = 1f;

        [HorizontalGroup("moveSpeed"), SerializeField, LabelText("+"), LabelWidth(10f)]
        float moveSpeedRandomRange = 0.25f;

        [HorizontalGroup("duration"), SerializeField, LabelWidth(85f)]
        float duration = 3f;

        [HorizontalGroup("duration"), SerializeField, LabelText("+"), LabelWidth(10f)]
        float durationRandomRange = 2f;

        [SerializeField,ReadOnly]
        SpriteRendererEditor srEditor;

        [SerializeField,ReadOnly]
        Light2DAnimator light2DAnimator;

        public event Action OnMoveEnd;

        Coroutine corMoving;

        void Awake()
        {
            srEditor = GetComponent<SpriteRendererEditor>();
            light2DAnimator = GetComponent<Light2DAnimator>();
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            corMoving = this.RestartCoroutine(Update(), corMoving);
            IEnumerator Update()
            {
                var moveSpeed = this.moveSpeed + Random.Range(0, moveSpeedRandomRange);
                var duration = this.duration + Random.Range(0, durationRandomRange);
                
                light2DAnimator.BeTransparent();
                srEditor.BeTransparent();
                light2DAnimator.BeOpaque(duration / 2);
                srEditor.BeOpaque(duration / 2);

                var isFadingOut = false;
                var time = 0f;
                while (time < duration)
                {
                    transform.position = new(transform.position.x + (moveSpeed * Time.deltaTime), transform.position.y);
                    time += Time.deltaTime;
                    if (time > duration / 2 && !isFadingOut)
                    {
                        isFadingOut = true;
                        light2DAnimator.BeTransparent(duration / 2.1f);
                        srEditor.BeTransparent(duration / 2.1f);
                    }
                    yield return null;
                }
                yield return new WaitForSeconds(1f);
                OnMoveEnd?.Invoke();
            }
        }
    }
}
