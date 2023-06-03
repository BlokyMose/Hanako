using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

namespace Tsutaeru
{
    public class DelayedInvoker : MonoBehaviour
    {
        [System.Serializable]
        public class DelayedEvent
        {
            [SerializeField]
            string eventName;

            [SerializeField]
            float delay;

            [SerializeField]
            UnityEvent onInvoke;

            public string EventName { get => eventName; }
            public float Delay { get => delay; }
            public UnityEvent OnInvoke { get => onInvoke; }

            public IEnumerator Invoke()
            {
                yield return new WaitForSeconds(Delay);
                OnInvoke.Invoke();
            }
        }

        [SerializeField, HorizontalGroup("Auto")]
        bool isAutoInvoke = true;

        [SerializeField,ShowIf(nameof(isAutoInvoke)), HorizontalGroup("Auto"), LabelWidth(0.1f)]
        UnityInitialMethod invokeOn = UnityInitialMethod.Awake;


        [SerializeField]
        List<DelayedEvent> events = new();

        private void Awake()
        {
            if (isAutoInvoke && invokeOn == UnityInitialMethod.Awake)
                InvokeAll();
        }

        private void OnEnable()
        {
            if (isAutoInvoke && invokeOn == UnityInitialMethod.OnEnable)
                InvokeAll();
        }

        private void Start()
        {
            if (isAutoInvoke && invokeOn == UnityInitialMethod.Start)
                InvokeAll();
        }

        public void Invoke(string eventName)
        {
            var foundEvent = events.Find(e => e.EventName == eventName);
            if (foundEvent != null)
                StartCoroutine(foundEvent.Invoke());
        }

        public void InvokeAll()
        {
            foreach (var delayedEvent in events)
                StartCoroutine(delayedEvent.Invoke());
        }
    }
}
