using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [InlineEditor]
    [CreateAssetMenu(fileName ="HanakoIcons", menuName ="SO/Hanako/Icons")]

    public class HanakoIcons : ScriptableObject
    {
        [SerializeField, PreviewField]
        Sprite warningIcon;

        [SerializeField, PreviewField]
        Sprite skullIcon;

        [SerializeField, PreviewField]
        Sprite okCircleIcon;

        public Sprite WarningIcon { get => warningIcon; }
        public Sprite SkullIcon { get => skullIcon; }
        public Sprite OkCircleIcon { get => okCircleIcon; }
    }
}
