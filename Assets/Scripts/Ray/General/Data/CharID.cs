using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="Char_", menuName ="SO/Char ID")]

    public class CharID : ScriptableObject
    {
        [SerializeField]
        string charName;

        [SerializeField]
        GameObject prefab;

        [SerializeField, PreviewField]
        Sprite icon;

        public string CharName { get => charName; }
        public GameObject Prefab { get => prefab; }
        public Sprite Icon { get => icon; }
    }
}
