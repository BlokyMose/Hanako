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

        [Header("Action Icon: Attack")]
        [SerializeField]
        Sprite actionIcon_Attack;

        [SerializeField]
        ActionIconMode actionIconMode_Attack = ActionIconMode.Tilting;

        int tri_open, tri_attack, tri_attackFailed, boo_isPossessed, boo_hanakoPeeks;
        bool canAttack = false;
        HashSet<HanakoEnemy> enemiesInDetectArea = new();
        bool isHighligthingEnemies = false;

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
                        AddEnemyInDetectArea(enemy);
                }

                void OnExit(Collider2D col)
                {
                    if (col.TryGetComponent<HanakoEnemy>(out var enemy))
                        RemoveEnemyInDetectArea(enemy);
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

        protected override void ShowActionIcon()
        {
            if (occupationMode == OccupationMode.Unoccupied)
            {
                base.ShowActionIcon();
            }
            else if (occupationMode == OccupationMode.Player)
            {
                actionIconSR.sprite = actionIcon_Attack;
                actionIconSR.color = levelManager.Colors.AttackableColor;
                actionIconAnimator.SetInteger(int_mode, (int)actionIconMode_Attack);
                actionIconAnimator.SetTrigger(tri_transition);
            }
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
            ChangeColor(colors.PlayerColor);
            destinationUI.ShowPlayerHere();
            if (playAnimation)
            {
                PlayAnimationPossessed();
                PlayAnimationHanakoPeeks();
            }


            StartCoroutine(Delay(animationDuration));
            IEnumerator Delay(float delay)
            {
                HideActionIcon();
                yield return new WaitForSeconds(delay);
                canAttack = true;
                if (isHovered)
                    ShowActionIcon();
            }
        }

        public void Dispossess(bool playAnimation = false)
        {
            occupationMode = OccupationMode.Unoccupied;
            ResetColor();
            destinationUI.HidePlayerHere();
            HideActionIcon();

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
                    HideActionIcon();
                    yield return new WaitForSeconds(delay);
                    canAttack = true;
                    if (isHovered)
                        ShowActionIcon();
                }
            }


            bool IsOuterEnemyDetecting()
            {
                if (enemiesDetecting.Count > 0)
                    foreach (var detectingEnemy in enemiesDetecting)
                        if (!enemiesInDetectArea.Contains(detectingEnemy))
                            return true;

                return false;
            }

            void LostGame()
            {
                animator.SetTrigger(tri_attackFailed);
                PlayAnimationHanakoHides();
                var isFacingRight = enemiesDetecting.First().transform.position.x > transform.position.x;
                hanakoFront.localEulerAngles = new Vector3(0, isFacingRight ? 0 : 180, 0);
                levelManager.LostGame();
            }
        }

        public void AddEnemyInDetectArea(HanakoEnemy enemy)
        {
            enemiesInDetectArea.Add(enemy);
            if (isHighligthingEnemies)
                enemy.Highlight(HanakoEnemy.HighlightMode.Attackable);
        }        
        
        public void RemoveEnemyInDetectArea(HanakoEnemy enemy)
        {
            enemiesInDetectArea.Remove(enemy);
            if (isHighligthingEnemies)
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
        }

        public override void AddDetectedBy(HanakoEnemy enemy)
        {
            base.AddDetectedBy(enemy);
            if (isHighligthingEnemies && !enemiesInDetectArea.Contains(enemy))
                enemy.Highlight(HanakoEnemy.HighlightMode.Detecting);
        }

        public override void RemoveDetectedBy(HanakoEnemy enemy)
        {
            base.RemoveDetectedBy(enemy);
            if (isHighligthingEnemies && !enemiesInDetectArea.Contains(enemy))
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
        }

        public void HighlightDetectingEnemies()
        {
            isHighligthingEnemies = true;
            foreach (var enemy in enemiesDetecting)
                enemy.Highlight(HanakoEnemy.HighlightMode.Detecting);

            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(HanakoEnemy.HighlightMode.Attackable);
                
        }        
        
        public void ResetHighlightEnemies()
        {
            isHighligthingEnemies = false;
            foreach (var enemy in enemiesDetecting)
                enemy.Highlight(HanakoEnemy.HighlightMode.None);

            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
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
