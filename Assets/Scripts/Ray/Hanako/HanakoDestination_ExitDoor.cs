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

        protected override void WhenOccupationStart(HanakoEnemy enemy)
        {
            base.WhenOccupationStart(enemy);
            StartCoroutine(MoveOccupant(currentOccupant, extraRunPosition.position, fadeOutDuration));
            enemy.ReachedExitDoor(fadeOutDuration);
        }
    }
}
