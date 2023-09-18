using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination, IHanakoInteractableByCursor
    {
        [SerializeField]
        ColliderProxy detectArea;

        [SerializeField]
        Transform hanakoFront;

        [Header("VFX")]
        [SerializeField]
        GameObject vfxSuccessfulAttack;

        [SerializeField]
        Transform vfxSuccessfulAttackParent;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom uiAudioSource;

        [SerializeField]
        AudioSourceRandom doorAudioSource;

        [SerializeField]
        AudioSourceRandom hanakoAudioSource;

        [SerializeField]
        string sfxHoverName = "sfxHover";

        [SerializeField]
        string sfxClickName = "sfxClick";
        
        [SerializeField]
        string sfxDoorOpenName = "sfxDoorOpen";
        
        [SerializeField]
        string sfxDoorCloseName = "sfxDoorClose";

        [SerializeField]
        string sfxFlushName = "sfxFlush";

        [SerializeField]
        string sfxScreamName = "sfxScream";

        [SerializeField]
        string sfxKillName = "sfxKill";

        [SerializeField]
        string sfxKillBigName = "sfxKillBig";

        [SerializeField]
        string sfxLostName = "sfxLost";

        event Action OnLostGame;
        event Action<int, float> OnVFXSuccessfulAttack; // killedEnemies, delay

        int tri_open, tri_attack, tri_attackFailed, boo_isPossessed, boo_hanakoPeeks;
        bool canAttack = false;
        HashSet<HanakoEnemy> enemiesInDetectArea = new();
        bool isHighligthingEnemies = false;
        bool canInstantiateVFXSuccessfulAttack = false;
        int killedEnemiesInLastAttack = 0;


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

        public void Init(HanakoColors colors, HanakoIcons icons, Func<HanakoLevelManager.HanakoGameState> getGameState, int indexOfAllDestinations, int indexOfSameID, Action onLostGame, Action<int, float> onVFXSuccessfulAttack)
        {
            base.Init(colors, icons, getGameState, indexOfAllDestinations,indexOfSameID);
            this.OnLostGame += onLostGame;
            this.OnVFXSuccessfulAttack += onVFXSuccessfulAttack;
        }

        protected override void WhenOccupationStart(HanakoEnemy enemy)
        {
            base.WhenOccupationStart(enemy);
            animator.SetTrigger(tri_open);
            doorAudioSource.PlayOneClipFromPack(sfxFlushName);
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
                base.ShowActionIcon(); // Arrow down icon & animation
            }
            else if (occupationMode == OccupationMode.Player)
            {
                actionIconSR.sprite = icons.AttackIcon;
                actionIconSR.color = colors.AttackableColor;
                actionIconAnimator.SetInteger(int_mode, (int)icons.AttackAnimation);
                actionIconAnimator.SetTrigger(tri_transition);
            }
        }

        // Should be called by animation
        public void OnAnimationToiletIsOpened()
        {
            if (occupationMode == OccupationMode.Enemy) // enemy enters
            {
                if (currentOccupant != null)
                {
                    currentOccupant.transform.parent = postInteractPos;

                    // in case this function not called when two enemies enter & exit at the same time
                    if (lastOccupant != null && lastOccupant.transform.parent == postInteractPos)
                        lastOccupant.transform.parent = null;
                    doorAudioSource.PlayOneClipFromPack(sfxDoorOpenName);
                }
            }
            else // enemy exits
            {
                if (lastOccupant != null)
                    lastOccupant.transform.parent = null;
                doorAudioSource.PlayOneClipFromPack(sfxDoorCloseName);
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
            uiAudioSource.PlayOneClipFromPack(sfxClickName);

            StartCoroutine(Delay(animationDuration));
            IEnumerator Delay(float delay)
            {
                HideActionIcon();
                canAttack = false;
                yield return new WaitForSeconds(delay);
                canAttack = true;
                if (isHovered)
                {
                    Hover();
                    ShowActionIcon();
                }
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
            hanakoAudioSource.PlayOneClipFromPack(sfxScreamName);

            if (IsOuterEnemyDetecting())
            {
                LostGame();
            }
            else
            {
                if (enemiesInDetectArea.Count > 0)
                {
                    foreach (var enemy in enemiesInDetectArea)
                        enemy.ReceiveAttack(this, enemyReceiveAttackDelay, enemyReceiveAttackDelay / 2f);
                    StartCoroutine(PlaySFX(enemyReceiveAttackDelay, enemiesInDetectArea.Count));
                    killedEnemiesInLastAttack = enemiesInDetectArea.Count;
                    canInstantiateVFXSuccessfulAttack = true;
                    enemiesInDetectArea.Clear();

                    IEnumerator PlaySFX(float delay, int killCount)
                    {
                        yield return new WaitForSeconds(delay);
                        if (killCount == 1)
                            hanakoAudioSource.PlayOneClipFromPack(sfxKillName);
                        else
                            hanakoAudioSource.PlayOneClipFromPack(sfxKillBigName);
                    }
                }

                animator.SetTrigger(tri_attack);

                StartCoroutine(ResetCanAttack(cooldown));
                IEnumerator ResetCanAttack(float delay)
                {
                    HideActionIcon();
                    yield return new WaitForSeconds(delay);
                    canAttack = true;
                    if (isHovered)
                    {
                        ShowActionIcon();
                        Hover();
                    }
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
                hanakoAudioSource.PlayOneClipFromPack(sfxLostName);
                OnLostGame?.Invoke();
            }
        }

        public void AddEnemyInDetectArea(HanakoEnemy enemy)
        {
            if (!enemy.IsAlive || !enemy.IsKillable) return;
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

        public override void Hover()
        {
            base.Hover();

            uiAudioSource.PlayOneClipFromPack(sfxHoverName);

            if (occupationMode != OccupationMode.Player) return;

            isHighligthingEnemies = true;
            foreach (var enemy in enemiesDetecting)
                enemy.Highlight(HanakoEnemy.HighlightMode.Detecting);

            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(HanakoEnemy.HighlightMode.Attackable);


        }

        public override void Unhover()
        {
            base.Unhover();
            uiAudioSource.PlayOneClipFromPack(sfxHoverName);
            if (occupationMode != OccupationMode.Player) return;

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

        /// <summary>Should be called by the animation</summary>
        public void InstantiateVFXIfSuccessfulAttack()
        {
            if (!canInstantiateVFXSuccessfulAttack) return;
            canInstantiateVFXSuccessfulAttack = false;
            var vfxGO = Instantiate(vfxSuccessfulAttack, vfxSuccessfulAttackParent);
            OnVFXSuccessfulAttack?.Invoke(killedEnemiesInLastAttack, 0.1f); //delay
            Destroy(vfxGO, 2f);
        }
    }
}
