using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityUtility
{
    public class ProxyInvoker : MonoBehaviour
    {
        [Serializable]
        public class EventProperty
        {
            public string name;
            public UnityEvent onInvoke;
        }

        public List<EventProperty> events = new();

        public void InvokeEvent(string name)
        {
            events.Find(e => e.name == name).onInvoke.Invoke();
        }
    }
}
