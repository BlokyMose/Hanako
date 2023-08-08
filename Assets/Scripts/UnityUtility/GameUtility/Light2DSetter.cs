using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityUtility
{
    public class Light2DSetter : MonoBehaviour
    {
        [Serializable]
        public class LightEvent
        {
            public string eventName;
            public List<LightSetProperties> properties = new();
            public void Invoke()
            {
                foreach (var light in properties)
                    light.light.intensity = light.targetIntensity;
            }
        }

        [Serializable]
        public class LightSetProperties
        {
            public Light2D light;
            public float targetIntensity = 1f;
        }

        public List<LightEvent> events = new();

        public void InvokeEvent(string eventName)
        {
            var foundEvent = events.Find(x => x.eventName == eventName);
            if (foundEvent != null)
                foundEvent.Invoke();
        }
    }
}
