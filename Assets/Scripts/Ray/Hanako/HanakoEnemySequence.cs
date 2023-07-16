using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [CreateAssetMenu(fileName ="EnemySeq_", menuName ="SO/Hanako/Enemy Sequence")]
    [InlineEditor]
    public class HanakoEnemySequence : ScriptableObject
    {
        [Serializable]
        public class Enemy
        {
            [SerializeField]
            GameObject prefab;

            [SerializeField]
            List<HanakoDestinationID> destinationSequence = new();

            [SerializeField]
            float delay = 2f;

            public GameObject Prefab { get => prefab; }
            public List<HanakoDestinationID> DestinationSequence { get => destinationSequence; }
            public float Delay { get => delay; }
        }

        [SerializeField]
        string sequenceName;

        [SerializeField]
        List<Enemy> sequence = new();

        public string SequenceName { get => sequenceName; }
        public List<Enemy> Sequence { get => sequence; }

    }
}
