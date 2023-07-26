using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace UnityUtility
{
    public class VisualEffectSetter : MonoBehaviour
    {
        [Serializable]
        public class SetterProperty
        {
            public enum VarType { Bool, Float, Int}

            public string setterName;
            public string varName;
            public VarType varType;

            [ShowIf("@"+nameof(varType)+ "=="+ nameof(VarType)+"."+nameof(VarType.Bool)), LabelText("value")]
            public bool boolValue;
            
            [ShowIf("@"+nameof(varType)+ "=="+nameof(VarType)+"."+nameof(VarType.Float)), LabelText("value")]
            public float floatValue;

            [ShowIf("@" + nameof(varType) + "==" +nameof(VarType)+"."+ nameof(VarType.Int)), LabelText("value")]
            public int intValue;


            public void Invoke(VisualEffect vfx)
            {
                switch (varType)
                {
                    case VarType.Bool:
                        vfx.SetBool(varName, boolValue);
                        break;
                    case VarType.Float:
                        vfx.SetFloat(varName, floatValue);
                        break;
                    case VarType.Int:
                        vfx.SetInt(varName, intValue);
                        break;
                }
            }
        }

        [SerializeField]
        VisualEffect vfx;

        [SerializeField]
        List<SetterProperty> setters = new();


        void Awake()
        {
            if (TryGetComponent<VisualEffect>(out var visualEffect))
                vfx = visualEffect;
        }

        public void InvokeSetter(string setterName)
        {
            var foundSetter = setters.Find(s => s.setterName == setterName);
            if (foundSetter!=null)
            {
                foundSetter.Invoke(vfx);
            }
        }

    }
}
