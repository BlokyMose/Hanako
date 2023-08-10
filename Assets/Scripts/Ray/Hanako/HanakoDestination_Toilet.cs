using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility;
using static Hanako.Hanako.HanakoDestination;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        [SerializeField]
        ColliderProxy detectArea;

        [SerializeField]
        Transform hanakoFront;

        int tri_open, tri_attack, tri_attackFailed, boo_isPossessed, boo_hanakoPeeks;
        bool canAttack = false;
        HashSet<HanakoEnemy> enemiesInDetectArea = new();

        protected override void Awake()
        {
            base.Awake();
            tri_open = Animator.StringToHash(nameof(tri_open));
            tri_attack = Animator.StringToHash(nameof(tri_attack));
            tri_attackFailed = Animator.StringToHash(nameof(tri_attackFailed));
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
                        enemiesInDetectArea.Add(enemy);
                    }
                }

                void OnExit(Collider2D col)
                {
                    if (col.TryGetComponent<HanakoEnemy>(out var enemy))
                    {
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

        public void Possess(float animationDuration, bool playAnimation = false)
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


            StartCoroutine(Delay(animationDuration));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                canAttack = true;
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

        public void Attack(float cooldown, float enemyReceiveAttackDelay)
        {
            if (!canAttack) return;
            canAttack = false;

            if (IsOuterEnemyDetecting())
            {
                LostGame();
            }
            else
            {
                animator.SetTrigger(tri_attack);
                
                foreach (var enemy in enemiesInDetectArea)
                    enemy.ReceiveAttack(this, enemyReceiveAttackDelay, enemyReceiveAttackDelay / 2f);
                enemiesInDetectArea.Clear();
                
                StartCoroutine(ResetCanAttack(cooldown));
                IEnumerator ResetCanAttack(float delay)
                {
                    yield return new WaitForSeconds(delay);
                    canAttack = true;
                }
            }


            bool IsOuterEnemyDetecting()
            {
                if (detectingEnemies.Count > 0)
                    foreach (var detectingEnemy in detectingEnemies)
                        if (!enemiesInDetectArea.Contains(detectingEnemy))
                            return true;

                return false;
            }

            void LostGame()
            {
                animator.SetTrigger(tri_attackFailed);
                PlayAnimationHanakoHides();
                var isFacingRight = detectingEnemies.First().transform.position.x > transform.position.x;
                hanakoFront.localEulerAngles = new Vector3(0, isFacingRight ? 0 : 180, 0);
                levelManager.LostGame();
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
