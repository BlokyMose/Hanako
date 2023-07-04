using UnityUtility;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace UnityUtility
{
    public static class ComponentUtility
    {
        public static T GetComponentInFamily<T>(this Component thisComponent) where T : Component
        {
            var targetComponent = thisComponent.GetComponent<T>();
            if (targetComponent == null)
                targetComponent = thisComponent.GetComponentInParent<T>();
            if (targetComponent == null)
                targetComponent = thisComponent.GetComponentInChildren<T>();

            return targetComponent;
        }

        public static bool TryGetComponentInFamily<T>(this Component thisComponent, out T targetComponent) where T : Component
        {
            targetComponent = thisComponent.GetComponentInFamily<T>();
            return targetComponent != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component thisComponent, out T targetComponent) where T : Component
        {
            targetComponent = thisComponent.GetComponentInChildren<T>();
            return targetComponent != null;
        }

        public static T GetComponentInFamily<T>(this GameObject thisComponent) where T : Component
        {
            var targetComponent = thisComponent.GetComponent<T>();
            if(targetComponent == null)
                targetComponent = thisComponent.GetComponentInParent<T>();
            if(targetComponent == null)
                targetComponent = thisComponent.GetComponentInChildren<T>();

            return targetComponent;
        }

        public static List<T> GetComponentsInFamily<T>(this GameObject thisComponent) where T : Component
        {
            var targetComponents = new List<T>(thisComponent.GetComponents<T>());
            targetComponents.AddRange(thisComponent.GetComponentsInParent<T>());
            targetComponents.AddRange(thisComponent.GetComponentsInChildren<T>());
            return targetComponents;
        }

        public static GameObject Find(string goName)
        {
            var allGOs = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var go in allGOs)
                if (go.name == goName)
                    return go;

            return null;
        }

        public static Transform FindDescendant(this Transform root, string targetName)
        {
            Transform foundTarget = null;
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.gameObject.name == targetName)
                    return child;
            }

            if (foundTarget == null)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    var child = root.GetChild(i);
                    var _foundTarget = FindDescendant(child, targetName);
                    if (_foundTarget != null)
                        return _foundTarget;
                }
            }
                
            return foundTarget;
        }

        public static bool TryFindDescendant(this Transform root, string targetName, out Transform foundTarget)
        {
            foundTarget = FindDescendant(root, targetName);
            return foundTarget != null;
        }

        public static bool TryInstantiate<T>(this Object context, T original, out T instantiatedObject, Transform parent = null) where T : Object
        {
            if (original != null) 
            {
                instantiatedObject = Object.Instantiate(original,parent); ;
                return true;
            }
            else
            {
                instantiatedObject=null;
                return false;
            }
        }

        public static void DestroyChildren(this Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                 Object.Destroy(parent.GetChild(i).gameObject);
        }

        public static void DestroyImmediateChildren(this Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }

    }
}