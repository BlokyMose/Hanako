using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Hanako.HanakoDestination;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        [SerializeField]
        ColliderProxy detectArea;

        int tri_open, tri_attack, boo_isPossessed, boo_hanakoPeeks;
        HashSet<HanakoEnemy> enemiesInDetectArea = new();

        protected override void Awake()
        {
            base.Awake();
            tri_open = Animator.StringToHash(nameof(tri_open));
            tri_attack = Animator.StringToHash(nameof(tri_attack));
            boo_isPossessed = Animator.StringToHash(nameof(boo_isPossessed));
            boo_hanakoPeeks = Animator.StringToHash(nameof(boo_hanakoPeeks));

            if (detectArea != null)
            {
                detectArea.OnEnter += OnEnter;
                detectArea.OnExit += OnExit;

                void OnEnter(Collider2D col)
                {
                    if (col.TryGetComponent<HanakoEnemy>(out var enemy))
                    {
                        DebugLog.Color(DebugLog.ColorName.Green, "Enter " , enemy.gameObject.name);
                        enemiesInDetectArea.Add(enemy);
                    }
                }

                void OnExit(Collider2D col)
                {
                    if (col.TryGetComponent<HanakoEnemy>(out var enemy))
                    {
                        Debug.Log("Exit : " + enemy.gameObject.name);
                        enemiesInDetectArea.Remove(enemy);
                    }
                }

            }
        }

        protected override void WhenOccupationStart(HanakoEnemy enemy)
        {
            base.WhenOccupationStart(enemy);
            animator.SetTrigger(tri_open);
        }

        protected override void WhenOccupationEnd(HanakoEnemy enemy)
        {
            base.WhenOccupationEnd(enemy);
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
            ChangeSRsColor(colors.PlayerColor);
            isHovered = false;
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

        public void Attack()
        {
            animator.SetTrigger(tri_attack);

            if (detectingEnemies.Count > 0)
            {
                Debug.Log(nameof(detectingEnemies.Count) + " : " + detectingEnemies.Count);
                foreach (var enemy in enemiesInDetectArea)
                    enemy.ReceiveAttack(this);

                enemiesInDetectArea.Clear();
            }
            else
            {

                Debug.Log("No enemies detected");
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
