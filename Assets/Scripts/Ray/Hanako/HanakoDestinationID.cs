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
        Sprite logo;

        public string DisplayName { get => displayName; }
        public Sprite Logo { get => logo; }
    }
}
