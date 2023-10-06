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

#if UNITY_EDITOR
            public void SetID(HanakoDestinationID newID) => destinationID = newID;
            public void SetIndex(int newIndex) => index = newIndex;
#endif
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

#if UNITY_EDITOR
            public void SetID(HanakoEnemyID newID) => id = newID;
            public void SetSequence(List<DestinationProperties> newDestinationSequence) => destinationSequence = newDestinationSequence;
            public void SetDelay(float newDelay) => delay = newDelay;
#endif
        }

        [SerializeField]
        string sequenceName;

        [SerializeField]
        List<Enemy> sequence = new();

        public string SequenceName { get => sequenceName; }
        public List<Enemy> Sequence { get => sequence; }

    }
}
