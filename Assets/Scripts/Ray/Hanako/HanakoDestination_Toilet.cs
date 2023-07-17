using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        [Header("Toilet")]
        [SerializeField]
        Transform enemyLocationInToilet;

        [SerializeField]
        float durationToEnemyParent = 0.1f;

        public override bool IsOccupied { get => currentOccupant != null || isPossessed; }


        int tri_open;
        Vector2 occupantLastPos;
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
            occupantLastPos = enemy.transform.position;
            StartCoroutine(MoveOccupant(enemy, enemyLocationInToilet.position, durationToEnemyParent));
        }

        protected override void WhenInteractEnd(HanakoEnemy enemy)
        {
            base.WhenInteractEnd(enemy);
            animator.SetTrigger(tri_open);
            StartCoroutine(MoveOccupant(enemy, occupantLastPos, durationToEnemyParent));
        }

        public void Hover()
        {
            foreach (var sr in srs)
            {
                sr.color = Color.red;
            }
        }

        public void Unhover()
        {
            foreach (var sr in srs)
            {
                sr.color = Color.white;
            }
        }

        public void OnAnimationToiletIsOpened()
        {
            if (IsOccupied) // enemy enters
            {
                currentOccupant.transform.parent = enemyLocationInToilet;
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
