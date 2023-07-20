using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        public override bool IsOccupied { get => currentOccupant != null || isPossessed; }

        int tri_open, boo_isPossessed;
        bool isPossessed = false;

        protected override void Awake()
        {
            base.Awake();
            tri_open = Animator.StringToHash(nameof(tri_open));
            boo_isPossessed = Animator.StringToHash(nameof(boo_isPossessed));
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

        public void Possess(bool playAnimation = false)
        {
            isPossessed = true;
            ChangeSRsColor(Color.green);
            destinationUI.ShowPlayerHere();
            if (playAnimation)
                PlayAnimationPossessed();
        }

        public void Dispossess(bool playAnimation = false)
        {
            isPossessed = false;
            ResetSRsColor();
            destinationUI.HidePlayerHere();
            if (playAnimation)
                PlayAnimationUnpossessed();
        }

        public void PlayAnimationPossessed()
        {
            animator.SetBool(boo_isPossessed, true);
        }
        public void PlayAnimationUnpossessed()
        {
            animator.SetBool(boo_isPossessed, false);
        }
    }
}
