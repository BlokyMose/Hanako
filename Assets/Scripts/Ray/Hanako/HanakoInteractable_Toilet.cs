using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility;
using static Hanako.Hanako.HanakoDestination;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(HanakoDestination))]
    public class HanakoInteractable_Toilet : HanakoInteractable
    {
        [Header("Toilet")]
        [SerializeField]
        Transform postAttackPos;

        [SerializeField]
        Transform hanakoFront;

        [Header("VFX")]
        [SerializeField]
        GameObject vfxSuccessfulAttack;

        [SerializeField]
        Transform vfxSuccessfulAttackParent;

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom hanakoAudioSource;

        [SerializeField]
        string sfxScreamName = "sfxScream";

        [SerializeField]
        string sfxKillName = "sfxKill";

        [SerializeField]
        string sfxKillBigName = "sfxKillBig";

        [SerializeField]
        string sfxLostName = "sfxLost";

        HanakoDestination destination;
        public Transform PostAttackPos => postAttackPos; 
        public Transform PostInteractPos => destination.PostInteractPos; 
        public Transform InteractablePos => destination.InteractablePos;
        public OccupationMode Occupation => destination.Occupation;
        public bool CanBePossessed => Occupation == OccupationMode.Unoccupied;


        protected HashSet<HanakoEnemy> enemiesDetecting = new();
        protected bool isHighligthingEnemies = false;
        protected int tri_open, tri_attack, tri_attackFailed, boo_isPossessed, boo_hanakoPeeks;
        protected bool canAttack = false;
        protected bool canInstantiateVFXSuccessfulAttack = false;
        protected int killedEnemiesInLastAttack = 0;

        public event Action OnPossess, OnDispossess;
        event Action OnLostGame;
        event Action<int, float> OnVFXSuccessfulAttack; // killedEnemies, delay
        event Action<int> OnEnemiesKilled;

        protected override Sprite GetActionIcon => Occupation == OccupationMode.Player ? icons.AttackIcon : icons.ArrownDownIcon;
        protected override HanakoIcons.ActionIconMode GetActionIconAnimation => Occupation == OccupationMode.Player ? icons.AttackAnimation : icons.ArrowDownAnimation;

        protected override void Awake()
        {
            base.Awake();

            tri_open = Animator.StringToHash(nameof(tri_open));
            tri_attack = Animator.StringToHash(nameof(tri_attack));
            tri_attackFailed = Animator.StringToHash(nameof(tri_attackFailed));
            boo_isPossessed = Animator.StringToHash(nameof(boo_isPossessed));
            boo_hanakoPeeks = Animator.StringToHash(nameof(boo_hanakoPeeks));

            destination = GetComponent<HanakoDestination>();

            colorSetter = GetComponent<SpriteRendererColorSetter>();
            colorSetter.RemoveSR(actionIconSR);
            if (destinationUIPrefab != null)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
            }
        }

        public void Init(
            Action onLostGame,
            Action<int, float> onVFXSuccessfulAttack,
            Action<int> onEnemiesKilled)
        {
            this.OnLostGame += onLostGame;
            this.OnVFXSuccessfulAttack += onVFXSuccessfulAttack;
            this.OnEnemiesKilled += onEnemiesKilled;
        }

        public override void Hover()
        {
            if (Occupation == OccupationMode.Enemy) return;
            isHovering = true;

            uiAudioSource.PlayOneClipFromPack(sfxHoverName);
            ChangeColor(colors.HoverColor);
            ShowActionIcon();

            if (Occupation == OccupationMode.Player)
            {
                ChangeColor(colors.PlayerColor);
                isHighligthingEnemies = true;
                foreach (var enemy in enemiesDetecting)
                    enemy.Highlight(HanakoEnemy.HighlightMode.Detecting);

                foreach (var enemy in enemiesInDetectArea)
                    enemy.Highlight(HanakoEnemy.HighlightMode.Attackable);

                detectAreaAnimator.SetInteger(int_mode, (int)DetectAreaAnimation.Show);
            }
        }

        public override void Unhover()
        {
            base.Unhover();
            if (Occupation == OccupationMode.Player)
            {
                isHighligthingEnemies = false;
                foreach (var enemy in enemiesDetecting)
                    enemy.Highlight(HanakoEnemy.HighlightMode.None);

                foreach (var enemy in enemiesInDetectArea)
                    enemy.Highlight(HanakoEnemy.HighlightMode.None);

                detectAreaAnimator.SetInteger(int_mode, (int)DetectAreaAnimation.Hide);
            }
        }

        public void AddDetectedBy(HanakoEnemy enemy)
        {
            enemiesDetecting.Add(enemy);
            if (isHighligthingEnemies && !enemiesInDetectArea.Contains(enemy))
                enemy.Highlight(HanakoEnemy.HighlightMode.Detecting);
        }

        public void RemoveDetectedBy(HanakoEnemy enemy)
        {
            enemiesDetecting.Remove(enemy);
            if (isHighligthingEnemies && !enemiesInDetectArea.Contains(enemy))
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
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
                    OnEnemiesKilled?.Invoke(killedEnemiesInLastAttack);
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
                    if (isHovering)
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

        public void Possess(float animationDuration, bool playAnimation = false)
        {
            OnPossess?.Invoke();
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
                if (isHovering)
                {
                    Hover();
                    ShowActionIcon();
                }
            }
        }

        public void Dispossess(bool playAnimation = false)
        {
            OnDispossess?.Invoke();
            ResetColor();
            destinationUI.HidePlayerHere();
            HideActionIcon();

            if (playAnimation)
            {
                PlayAnimationUnpossessed();
                PlayAnimationHanakoHides();
            }
        }

        // Called by animation "toilet_hanakoAttack"
        public void InstantiateVFXIfSuccessfulAttack()
        {
            if (!canInstantiateVFXSuccessfulAttack) return;
            canInstantiateVFXSuccessfulAttack = false;
            var vfxGO = Instantiate(vfxSuccessfulAttack, vfxSuccessfulAttackParent);
            OnVFXSuccessfulAttack?.Invoke(killedEnemiesInLastAttack, 0.1f); //delay
            Destroy(vfxGO, 2f);
        }

        #region [Methods: Animation]

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

        #endregion
    }
}
