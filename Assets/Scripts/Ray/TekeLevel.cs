using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="TekeLevel", menuName ="SO/TekeLevel")]

    public class TekeLevel : ScriptableObject
    {
        [SerializeField]
        string levelName;

        public string LevelName { get => levelName; }

    }
}
