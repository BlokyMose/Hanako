using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoDestination_ExitDoor : HanakoDestination
    {
        [Header("Exit Door")]
        [SerializeField]
        float fadeOutDuration = 0.33f;

        [SerializeField]
        Transform extraRunPosition;

        public override IEnumerator Occupy(HanakoEnemy enemy)
        {
            StartCoroutine(MoveOccupant(enemy, extraRunPosition.position, fadeOutDuration));
            enemy.ReachedExitDoor(fadeOutDuration);
            yield return null;
        }
    }
}
