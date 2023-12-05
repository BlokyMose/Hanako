using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityUtility
{
    public class AutoAnimatorParamSetter : AutoInvoke
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        List<GameplayUtilityClass.AnimatorParameterStatic> parameters = new List<GameplayUtilityClass.AnimatorParameterStatic>();

        protected override void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            foreach (var param in parameters)
                param.Init();

            base.Awake();
        }

        protected override IEnumerator Invoking()
        {
            yield return base.Invoking();
            
            foreach (var param in parameters)
                param.SetParam(animator);
        }

    }
}
