using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    public interface IHanakoInteractableByCursor 
    {
        public enum DetectAreaAnimation { Hide, Show }
        public Animator GetDetectAreaAnimator();
        public void Hover();
        public void Unhover();
    }
}
