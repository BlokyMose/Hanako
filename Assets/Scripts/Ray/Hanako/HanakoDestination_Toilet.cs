using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        public override bool IsOccupied { get => currentOccupant != null || isPossessed; }

        int tri_open;
        bool isPossessed = false;

        protected override void Awake()
        {
            base.Awake();
            tri_open = Animator.StringToHash(nameof(tri_open));
        }

        protected override void WhenInteractStart(HanakoEnemy enemy)
        {
            base.WhenInteractStart(enemy);
            animator.SetTrigger(tri_open);
        }

        protected override void WhenInteractEnd(HanakoEnemy enemy)
        {
            base.WhenInteractEnd(enemy);
            animator.SetTrigger(tri_open);
        }

        public void OnAnimationToiletIsOpened()
        {
            if (IsOccupied) // enemy enters
            {
                currentOccupant.transform.parent = postInteractPos;
            }
            else // enemy exits
            {
                if (lastOccupant!=null)
                    lastOccupant.transform.parent = null;
            }
        }

        public void Possess()
        {
            isPossessed = true;
            destinationUI.ShowPlayerHere();
        }

        public void Dispossess()
        {
            isPossessed = false;
            destinationUI.HidePlayerHere();
        }
    }
}
