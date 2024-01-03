using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class AnimatorLayerSetter : MonoBehaviour
    {
        [Serializable]
        public class LayerSetter
        {
            public int layerIndex;
            public float targetWeight;

            public void SetLayer(Animator animator)
            {
                animator.SetLayerWeight(layerIndex, targetWeight);
            }
        }
        
        [SerializeField]
        Animator animator;

        [SerializeField]
        List<LayerSetter> layers = new();


        private void OnEnable()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            foreach (var layer in layers)
                layer.SetLayer(animator);
        }
    }
}
