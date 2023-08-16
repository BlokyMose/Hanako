using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [CreateAssetMenu(fileName ="HanakoLvl_", menuName ="SO/Hanako/Level")]

    public class HanakoLevel : ScriptableObject
    {
        [SerializeField]
        string levelName;

        [SerializeField]
        HanakoEnemySequence enemySequence;

        [SerializeField]
        HanakoDestinationSequence destinationSequence;

        [SerializeField]
        HanakoDistractionSequence distractionSequence;

        public string LevelName { get => levelName; }
        public HanakoEnemySequence EnemySequence { get => enemySequence; }
        public HanakoDestinationSequence DestinationSequence { get => destinationSequence; }
        public HanakoDistractionSequence DistractionSequence { get => distractionSequence; }
    }
}
