using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hub
{
    [CreateAssetMenu(fileName ="HubColors", menuName ="SO/Hub/Hub Colors")]

    public class HubColors : ScriptableObject
    {
        [SerializeField]
        Color incompleteLevel = Color.red;

        [SerializeField]
        Color completedLevel;

        public Color IncompleteLevel { get => incompleteLevel; }
        public Color CompletedLevel { get => completedLevel; }
    }
}
