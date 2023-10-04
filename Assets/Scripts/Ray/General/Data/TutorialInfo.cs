using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="Tut_", menuName ="SO/Tutorial")]

    public class TutorialInfo : ScriptableObject
    {
        [SerializeField]
        List<GameObject> tutPages = new();

        public List<GameObject> TutPages => tutPages; 
    }
}
