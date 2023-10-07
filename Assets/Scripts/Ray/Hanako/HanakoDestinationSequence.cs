using Hanako.Hanako;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.HanakoDestinationSequence;

namespace Hanako
{
    [CreateAssetMenu(fileName ="DestinationSeq_", menuName ="SO/Hanako/Destination Sequence")]
    [InlineEditor]
    public class HanakoDestinationSequence : ScriptableObject
    {
        [Serializable]
        public class Destination
        {
            [SerializeField]
            GameObject prefab;

            [SerializeField]
            Vector2 position;

            public GameObject Prefab { get => prefab; }
            public Vector2 Position { get => position; }

#if UNITY_EDITOR
            public void SetPrefab(GameObject newPrefab)=> prefab = newPrefab;
            public void SetPosition(Vector2 newPosition) => position = newPosition;
#endif
        }

        [SerializeField]
        string sequenceName;

        [SerializeField]
        List<Destination> sequence = new();

        public string SequenceName { get => sequenceName; }
        public List<Destination> Sequence { get => sequence; }

        public void SortDestinations()
        {
            var sortedDestinations = new List<Destination>();
            foreach (var destination in sequence)
            {
                bool isInserted = false;
                foreach (var sortedDestination in sortedDestinations)
                {
                    if (destination.Position.x < sortedDestination.Position.x)
                    {
                        sortedDestinations.Insert(0, destination);
                        isInserted = true;
                        break;
                    }
                }

                if (!isInserted)
                    sortedDestinations.Add(destination);

                sequence = sortedDestinations;
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }
    }
}
