using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    public class HanakoDistraction : HanakoInteractable
    {
        public enum AnimationState { Idle, Distract, Cooldown }

        [Header("Distraction")]
        [SerializeField]
        float duration = 2f;

        [SerializeField]
        float cooldown = 2f;

        float currentCooldown = -1f;

        public event Action<float> OnDistractDurationStart;
        public event Action<float> OnCooldownDurationStart;
        public event Action OnShowDestinationUI;
        public event Action OnHideDestinationUI;
        Coroutine corDepletingCooldown;

        public Animator GetDetectAreaAnimator() => detectAreaAnimator;
        protected override Sprite GetActionIcon => icons.DistractionIcon;
        protected override ActionIconMode GetActionIconAnimation => icons.DistractionAnimation;

        protected override void Awake()
        {
            base.Awake();

            if (destinationUIPrefab != null)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
                destinationUI.Init(false, ref OnDistractDurationStart, ref OnCooldownDurationStart, ref OnShowDestinationUI, ref OnHideDestinationUI);
            }
        }

        protected virtual void OnDestroy()
        {
            if (destinationUIPrefab!=null)
                destinationUI.Exit(ref OnDistractDurationStart, ref OnCooldownDurationStart, ref OnShowDestinationUI, ref OnHideDestinationUI);
        }

        public void Init(HanakoColors colors, HanakoIcons icons)
        {
            this.colors = colors;
            this.icons = icons;
            corDepletingCooldown = this.RestartCoroutine(DepletingCooldown(), corDepletingCooldown);
        }

        public override void Hover()
        {
            if (currentCooldown > 0f) return; // Hover() is called by DepletingCooldown once cooldown is done
            base.Hover();
            isHovering = true;
        }

        public override void Unhover()
        {
            base.Unhover();
        }

        IEnumerator DepletingCooldown()
        {
            OnCooldownDurationStart?.Invoke(cooldown);
            animator.SetInteger(int_mode, (int)AnimationState.Cooldown);
            while (currentCooldown > -0.1f)
            {
                currentCooldown -= Time.deltaTime;
                yield return null;
            }
            animator.SetInteger(int_mode, (int)AnimationState.Idle);

            if (isHovering)
                Hover();
            OnHideDestinationUI?.Invoke();
        }

        public void Distract()
        {
            if (currentCooldown > 0f) return;
            this.StopCoroutineIfExists(corDepletingCooldown);
            
            ResetColor();
            ResetHighlightEnemies();
            HideActionIcon();
            detectAreaAnimator.SetInteger(int_mode, (int)DetectAreaAnimation.Hide);

            StartCoroutine(ApplyDistractionToEnemies());
            IEnumerator ApplyDistractionToEnemies()
            {
                animator.SetInteger(int_mode, (int)AnimationState.Distract);
                OnShowDestinationUI?.Invoke();
                OnDistractDurationStart?.Invoke(duration);
                currentCooldown = cooldown;

                var distractedEnemies = new List<HanakoEnemy>();
                foreach (var enemy in enemiesInDetectArea)
                    if (enemy.IsAlive && enemy.IsKillable)
                    {
                        enemy.ReceiveDistraction(transform.position);
                        distractedEnemies.Add(enemy);
                    }
                
                WhenDistractStart();
                yield return new WaitForSeconds(duration);
                WhenDistractEnd();

                foreach (var enemy in distractedEnemies)
                    enemy.MoveToCurrentDestination();

                corDepletingCooldown = this.RestartCoroutine(DepletingCooldown(), corDepletingCooldown);
            }
        }

        protected virtual void WhenDistractStart()
        {
            
        }

        protected virtual void WhenDistractEnd()
        {
            
        }

        protected override void AddEnemyInDetectArea(HanakoEnemy enemy, bool isHighlighthing)
        {
            base.AddEnemyInDetectArea(enemy, currentCooldown < 0f ? isHighlighthing : false);
        }
    }
}
