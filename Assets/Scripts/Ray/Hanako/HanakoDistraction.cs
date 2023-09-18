using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtility;
using static Hanako.Hanako.HanakoLevelManager;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    public class HanakoDistraction : MonoBehaviour, IHanakoInteractableByCursor
    {
        public enum AnimationState { Idle, Distract, Cooldown }

        [SerializeField]
        float duration = 2f;

        [SerializeField]
        float cooldown = 2f;

        [Header("Components")]
        [SerializeField]
        Animator animator;

        [SerializeField]
        ColliderProxy detectArea;

        [Header("UI")]
        [SerializeField]
        GameObject actionIconPrefab;

        [SerializeField, ShowIf("@" + nameof(actionIconPrefab))]
        Transform actionIconParent;

        [SerializeField]
        protected GameObject destinationUIPrefab;

        [SerializeField, ShowIf("@"+nameof(destinationUIPrefab))]
        protected Transform destinationUIParent;

        [Header("VFX")]
        [SerializeField]
        List<VisualEffect> distractVFXs = new();


        [Header("Customization")]
        [SerializeField]
        HanakoColors colors;

        [SerializeField]
        HanakoIcons icons;


        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom uiAudioSource;

        [SerializeField]
        AudioSourceRandom fanAudioSource;

        [SerializeField]
        string sfxHoverName = "sfxHover";

        [SerializeField]
        string sfxClickName = "sfxClick";

        [SerializeField]
        string sfxFanSpinName = "sfxFanSpin";

        HashSet<HanakoEnemy> enemiesInDetectArea = new();
        float currentCooldown = -1f;
        protected int int_mode, tri_transition;

        SpriteRendererColorSetter colorSetter;
        event Func<HanakoGameState> GetGameState;
        HanakoDestinationUI destinationUI;
        Animator actionIconAnimator;
        SpriteRenderer actionIconSR;
        Coroutine corDepletingCooldown;
        bool isHovering;

        public event Action<float> OnDistractDurationStart;
        public event Action<float> OnCooldownDurationStart;
        public event Action OnShowDestinationUI;
        public event Action OnHideDestinationUI;

        void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_transition = Animator.StringToHash(nameof(tri_transition));
            animator = animator == null ? GetComponent<Animator>() : animator;
            colorSetter = GetComponent<SpriteRendererColorSetter>();

            foreach (var vfx in distractVFXs)
                vfx.SetBool("isPlaying", false);

            if (destinationUIPrefab!=null)
            {
                destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
                destinationUI.transform.localPosition = Vector3.zero;
                destinationUI.Init(false, ref OnDistractDurationStart, ref OnCooldownDurationStart, ref OnShowDestinationUI, ref OnHideDestinationUI);
            }

            if (actionIconPrefab != null)
            {
                var actionIconGO = Instantiate(actionIconPrefab, actionIconParent);
                actionIconAnimator = actionIconGO.GetComponent<Animator>();
                actionIconSR = actionIconGO.GetComponentInChildren<SpriteRenderer>();
                colorSetter.RemoveSR(actionIconSR);
            }


            detectArea.OnEnter += OnEnter;
            detectArea.OnExit += OnExit;

            void OnEnter(Collider2D col)
            {
                if (col.gameObject.TryGetComponent<HanakoEnemy>(out var enemy))
                {
                    AddEnemyInDetectArea(enemy);
                }
            }

            void OnExit(Collider2D col)
            {
                if (col.gameObject.TryGetComponent<HanakoEnemy>(out var enemy))
                {
                    RemoveEnemyInDetectArea(enemy);
                }
            }
        }


        protected virtual void OnDestroy()
        {
            if (destinationUIPrefab!=null)
            {
                destinationUI.Exit(ref OnDistractDurationStart, ref OnCooldownDurationStart, ref OnShowDestinationUI, ref OnHideDestinationUI);
            }
        }

        public void Init(HanakoColors colors, HanakoIcons icons, Func<HanakoGameState> getGameState)
        {
            this.colors = colors;
            this.icons = icons;
            this.GetGameState = getGameState;
            corDepletingCooldown = this.RestartCoroutine(DepletingCooldown(), corDepletingCooldown);
        }

        public void Hover()
        {
            if (isHovering) return;
            isHovering = true;
            if (currentCooldown > 0f) return; // Hover() is called by DepletingCooldown once cooldown is done
            uiAudioSource.PlayOneClipFromPack(sfxHoverName);
            colorSetter.ChangeColor(colors.HoverColor);
            HighlightEnemies();
            ShowActionIcon();
        }

        public void Unhover()
        {
            if (!isHovering) return;
            isHovering = false;
            uiAudioSource.PlayOneClipFromPack(sfxHoverName);
            colorSetter.ResetColor();
            ResetHighlightEnemies();
            HideActionIcon();
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
            ResetHighlightEnemies();
            HideActionIcon();
            colorSetter.ResetColor();
            OnShowDestinationUI?.Invoke();
            OnDistractDurationStart?.Invoke(duration);
            currentCooldown = cooldown;
            animator.SetInteger(int_mode, (int)AnimationState.Distract);
            foreach (var vfx in distractVFXs)
                vfx.SetBool("isPlaying", true);

            StartCoroutine(ApplyDistractionToEnemies());
            IEnumerator ApplyDistractionToEnemies()
            {
                var distractedEnemies = new List<HanakoEnemy>();
                foreach (var enemy in enemiesInDetectArea)
                    if (enemy.IsAlive && enemy.IsKillable)
                    {
                        enemy.ReceiveDistraction(transform.position);
                        distractedEnemies.Add(enemy);
                    }

                fanAudioSource.PlayOneClipFromPack(sfxFanSpinName);
                yield return new WaitForSeconds(duration);
                fanAudioSource.Stop();

                foreach (var enemy in distractedEnemies)
                    enemy.MoveToCurrentDestination();

                corDepletingCooldown = this.RestartCoroutine(DepletingCooldown(), corDepletingCooldown);
                foreach (var vfx in distractVFXs)
                    vfx.SetBool("isPlaying", false);
            }
        }

        public void HighlightEnemies()
        {
            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(HanakoEnemy.HighlightMode.Distractable);
        }

        public void ResetHighlightEnemies()
        {
            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
        }

        protected virtual void ShowActionIcon()
        {
            actionIconSR.sprite = icons.DistractionIcon;
            actionIconSR.color = colors.AttackableColor;
            actionIconAnimator.SetInteger(int_mode, (int)icons.DistractionAnimation);
            actionIconAnimator.SetTrigger(tri_transition);
        }

        protected void HideActionIcon()
        {
            actionIconAnimator.SetInteger(int_mode, (int)icons.HideAnimation);
            actionIconAnimator.SetTrigger(tri_transition);
        }


        public void AddEnemyInDetectArea(HanakoEnemy enemy)
        {
            if (!enemy.IsAlive || !enemy.IsKillable) return;
            enemiesInDetectArea.Add(enemy);
            if (isHovering && currentCooldown < 0f)
                enemy.Highlight(HanakoEnemy.HighlightMode.Distractable);
        }

        public void RemoveEnemyInDetectArea(HanakoEnemy enemy)
        {
            enemiesInDetectArea.Remove(enemy);
            if (isHovering)
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
        }
    }
}
