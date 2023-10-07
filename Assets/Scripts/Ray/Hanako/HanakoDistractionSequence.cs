using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [InlineEditor]
    [CreateAssetMenu(fileName ="DistractionSeq_", menuName ="SO/Hanako/Distraction Sequence")]
    public class HanakoDistractionSequence : ScriptableObject
    {
        [Serializable]
        public class Distraction
        {
            [SerializeField]
            GameObject prefab;

            [SerializeField]
            Vector2 position;

            public GameObject Prefab { get => prefab; }
            public Vector2 Position { get => position; }

#if UNITY_EDITOR
            public void SetPrefab(GameObject newPrefab) => prefab = newPrefab;
            public void SetPosition(Vector2 newPosition) => position = newPosition;
#endif
        }

        [SerializeField]
        string sequenceName;

        [SerializeField]
        List<Distraction> sequence = new();

        public string SequenceName { get => sequenceName; }
        public List<Distraction> Sequence { get => sequence; }

        public void SortDestinations()
        {
            var sortedDistraction = new List<Distraction>();
            foreach (var distraction in sequence)
            {
                bool isInserted = false;
                foreach (var sortedDestination in sortedDistraction)
                {
                    if (distraction.Position.x < sortedDestination.Position.x)
                    {
                        sortedDistraction.Insert(0, distraction);
                        isInserted = true;
                        break;
                    }
                }

                if (!isInserted)
                    sortedDistraction.Add(distraction);

                sequence = sortedDistraction;
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }
    }
}
