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

        protected override void WhenInteractStart(HanakoEnemy enemy)
        {
            base.WhenInteractStart(enemy);
            if (enemy.TryGetComponentInFamily<SpriteRendererEditor>(out var srEditor))
                srEditor.BeTransparent(fadeOutDuration);
            StartCoroutine(MoveOccupant(currentOccupant, extraRunPosition.position, fadeOutDuration));
        }
    }
}
