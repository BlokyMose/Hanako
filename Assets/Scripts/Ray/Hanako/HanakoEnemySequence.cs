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
        public class DestinationProperties
        {
            [SerializeField, HorizontalGroup(0.75f), LabelWidth(0.1f)]
            HanakoDestinationID destinationID;

            [SerializeField, HorizontalGroup, LabelWidth(0.1f)]
            int index;

            public HanakoDestinationID ID { get => destinationID; }
            public int Index { get => index; }
        }

        [Serializable]
        public class Enemy
        {
            [SerializeField]
            HanakoEnemyID id;

            [SerializeField]
            List<DestinationProperties> destinationSequence = new();

            [SerializeField]
            float delay = 2f;

            public HanakoEnemyID ID { get => id; }
            public List<DestinationProperties> DestinationSequence { get => destinationSequence; }
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
