using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako
{
    public class SpriteRendererColorSetter : MonoBehaviour
    {
        [SerializeField]
        bool autoGetAllSRs = true;

        [SerializeField, HideIf(nameof(autoGetAllSRs))]
        List<SpriteRenderer> srs = new();

        [SerializeField, ShowIf(nameof(autoGetAllSRs))]
        List<SpriteRenderer> excludeSRs = new();

        Dictionary<SpriteRenderer, Color> initialColors = new();

        public void Awake()
        {
            if (autoGetAllSRs)
            {
                srs.AddRange(gameObject.GetComponentsInFamily<SpriteRenderer>());
                foreach (var sr in excludeSRs)
                {
                    var foundSR = srs.Find(x => x == sr);
                    if (foundSR != null) srs.Remove(foundSR);
                }
            }

            foreach (var sr in srs)
                initialColors.AddIfHasnt(sr, sr.color);
        }

        public void ChangeColor(Color color)
        {
            foreach (var sr in srs)
                sr.color = color;
        }
                
        public void ChangeColorExceptAlpha(Color color)
        {
            foreach (var sr in srs)
            {
                var currentAlpha = sr.color.a;
                sr.color = color.ChangeAlpha(currentAlpha);
            }
        }

        public void ResetColor()
        {
            foreach (var sr in initialColors)
                sr.Key.color = sr.Value;
        }        
        
        public void ResetColorExceptAlpha()
        {
            foreach (var sr in initialColors)
            {
                var currentAlpha = sr.Key.color.a;
                sr.Key.color = sr.Value.ChangeAlpha(currentAlpha);
            }
        }

        public void RemoveSR(SpriteRenderer sr)
        {
            srs.RemoveIfHas(sr);
            excludeSRs.Add(sr);
        }
    }
}
