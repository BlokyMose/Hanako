using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace UnityUtility
{
    public static class DuplicateComponent
    {
        private const string DUPLICATE_PATH = "CONTEXT/Component/Duplicate";

        [MenuItem(DUPLICATE_PATH)]
        public static void Duplicate(MenuCommand command)
        {
            var sourceComponent = command.context as Component;
            var newComponent =  Undo.AddComponent(sourceComponent.gameObject, sourceComponent.GetType());

            var source = new SerializedObject(sourceComponent);
            var target = new SerializedObject(newComponent);
            var iterator = source.GetIterator();
            while (iterator.NextVisible(true))
                target.CopyFromSerializedProperty(iterator);

            target.ApplyModifiedProperties();
        }

        [MenuItem(DUPLICATE_PATH, validate =true)]
        public static bool DuplicateValidation(MenuCommand command)
        {
            return DoesComponentAllowMultiples(command.context as Component);
        }

        static bool DoesComponentAllowMultiples(Component component)
        {
            var componentType = component.GetType();
            var attributes = (DisallowMultipleComponent[]) componentType.GetCustomAttributes(typeof(DisallowMultipleComponent), true);
            return attributes.Length == 0;
        }
    }
}
