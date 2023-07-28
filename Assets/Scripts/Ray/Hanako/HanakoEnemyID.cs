using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [CreateAssetMenu(fileName = "HanakoEnemyID_", menuName = "SO/Hanako/Enemy ID")]

    public class HanakoEnemyID : ScriptableObject
    {
        [SerializeField]
        string enemyName;

        [SerializeField]
        Sprite logo;

        [SerializeField]
        GameObject prefab;

        public string EnemyName { get => enemyName; }
        public Sprite Logo { get => logo;}
        public GameObject Prefab { get => prefab; }
    }
}
