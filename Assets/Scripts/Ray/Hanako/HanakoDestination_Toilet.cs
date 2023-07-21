using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Hanako.HanakoDestination;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        int tri_open, boo_isPossessed, boo_hanakoPeeks;

        protected override void Awake()
        {
            base.Awake();
            tri_open = Animator.StringToHash(nameof(tri_open));
            boo_isPossessed = Animator.StringToHash(nameof(boo_isPossessed));
            boo_hanakoPeeks = Animator.StringToHash(nameof(boo_hanakoPeeks));
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
            if (occupationMode == OccupationMode.Enemy) // enemy enters
            {
                if (currentOccupant != null)
                {
                    currentOccupant.transform.parent = postInteractPos;
                }
                else
                {
                    Debug.LogWarning($"{gameObject.name}   ERROR !!! NULL currentOccu");
                }
            }
            else // enemy exits
            {
                if (lastOccupant!=null)
                    lastOccupant.transform.parent = null;
            }
        }

        public void Possess(bool playAnimation = false)
        {
            occupationMode = OccupationMode.Player;
            ChangeSRsColor(Color.green);
            destinationUI.ShowPlayerHere();
            if (playAnimation)
            {
                PlayAnimationPossessed();
                PlayAnimationHanakoPeeks();
            }
        }

        public void Dispossess(bool playAnimation = false)
        {
            occupationMode = OccupationMode.Unoccupied;
            ResetSRsColor();
            destinationUI.HidePlayerHere();
            if (playAnimation)
            {
                PlayAnimationUnpossessed();
                PlayAnimationHanakoHides();
            }
        }

        public void PlayAnimationPossessed()
        {
            animator.SetBool(boo_isPossessed, true);
        }

        public void PlayAnimationUnpossessed()
        {
            animator.SetBool(boo_isPossessed, false);
        }

        public void PlayAnimationHanakoPeeks()
        {
            animator.SetBool(boo_hanakoPeeks, true);
        }

        public void PlayAnimationHanakoHides()
        {
            animator.SetBool(boo_hanakoPeeks, false);
        }
    }
}
