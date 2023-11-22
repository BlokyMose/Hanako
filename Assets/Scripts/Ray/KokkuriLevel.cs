using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="KokkuriLevel", menuName = "SO/KokkuriLevel")]
    public class KokkuriLevel : ScriptableObject
    {
        [SerializeField]
        string levelName;

        public string LevelName { get => levelName;  }

    }
}
