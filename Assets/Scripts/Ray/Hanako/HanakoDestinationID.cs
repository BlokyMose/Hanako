using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [CreateAssetMenu(fileName = "DestinationID_", menuName = "SO/Hanako/Destination ID")]
    public class HanakoDestinationID : ScriptableObject
    {
        [SerializeField]
        string displayName;

        [SerializeField]
        Color color = Color.white;

        [SerializeField, PreviewField, ListDrawerSettings(Expanded =true)]
        List<Sprite> logosByIndex = new();

        public string DisplayName { get => displayName; }
        public List<Sprite> LogosByIndex { get => logosByIndex; }
        public Sprite GetLogo(int index) => logosByIndex[index];
        public Color Color { get => color;  }
    }
}
