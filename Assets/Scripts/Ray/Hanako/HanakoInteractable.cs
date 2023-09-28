using FlyingWormConsole3.LiteNetLib.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Hanako.HanakoIcons;
using static Hanako.Hanako.HanakoEnemy;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(SpriteRendererColorSetter))]
    [RequireComponent(typeof(Animator))]
    public class HanakoInteractable : MonoBehaviour
    {
        [Header("Detect Area")]
        [SerializeField]
        protected HighlightMode detectAreaHighlightMode = HighlightMode.Attackable;
        
        [SerializeField]
        protected ColliderProxy detectArea;

        [Header("UI")]
        [SerializeField]
        protected GameObject destinationUIPrefab;

        [SerializeField, ShowIf("@" + nameof(destinationUIPrefab))]
        protected Transform destinationUIParent;

        [SerializeField]
        protected GameObject actionIconPrefab;

        [SerializeField, ShowIf("@" + nameof(actionIconPrefab))]
        protected Transform actionIconParent;

        [Header("SFX")]
        [SerializeField]
        protected AudioSourceRandom uiAudioSource;

        [SerializeField]
        protected string sfxHoverName = "sfxHover";

        [SerializeField]
        protected string sfxClickName = "sfxClick";

        [Header("Customization")]
        [SerializeField]
        protected HanakoColors colors;

        [SerializeField]
        protected HanakoIcons icons;

        protected Animator animator, detectAreaAnimator, actionIconAnimator;
        protected SpriteRendererColorSetter colorSetter;
        protected SpriteRenderer actionIconSR;
        protected int int_mode, tri_transition;
        protected bool isHovering;
        protected HashSet<HanakoEnemy> enemiesInDetectArea = new();
        protected HanakoDestinationUI destinationUI;

        protected virtual Sprite GetActionIcon => icons.ArrownDownIcon;
        protected virtual ActionIconMode GetActionIconAnimation => icons.ArrowDownAnimation;

        protected virtual void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));
            tri_transition = Animator.StringToHash(nameof(tri_transition));
            animator = GetComponent<Animator>();
            colorSetter = GetComponent<SpriteRendererColorSetter>();

            if (actionIconPrefab != null)
            {
                var actionIconGO = Instantiate(actionIconPrefab, actionIconParent);
                actionIconAnimator = actionIconGO.GetComponent<Animator>();
                actionIconSR = actionIconGO.GetComponentInChildren<SpriteRenderer>();
                colorSetter.RemoveSR(actionIconSR);
            }

            detectAreaAnimator = detectArea.GetComponent<Animator>();
            detectArea.OnEnter += OnEnter;
            detectArea.OnExit += OnExit;

            void OnEnter(Collider2D col)
            {
                if (col.gameObject.TryGetComponent<HanakoEnemy>(out var enemy))
                    AddEnemyInDetectArea(enemy);
            }

            void OnExit(Collider2D col)
            {
                if (col.gameObject.TryGetComponent<HanakoEnemy>(out var enemy))
                    RemoveEnemyInDetectArea(enemy);
            }
        }

        public virtual void Hover()
        {
            isHovering = true;

            uiAudioSource.PlayOneClipFromPack(sfxHoverName);
            ChangeColor(colors.HoverColor);
            HighlightEnemies();
            ShowActionIcon();
            detectAreaAnimator.SetInteger(int_mode, (int)DetectAreaAnimation.Show);
        }

        public virtual void Unhover()
        {
            isHovering = false;
            uiAudioSource.PlayOneClipFromPack(sfxHoverName);
            ResetColor();
            ResetHighlightEnemies();
            HideActionIcon();
            detectAreaAnimator.SetInteger(int_mode, (int)DetectAreaAnimation.Hide);
        }

        public void HighlightEnemies()
        {
            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(detectAreaHighlightMode);
        }

        public void ResetHighlightEnemies()
        {
            foreach (var enemy in enemiesInDetectArea)
                enemy.Highlight(HanakoEnemy.HighlightMode.None);
        }

        protected void ChangeColor(Color color)
        {
            colorSetter.ChangeColor(color);
        }

        protected void ResetColor()
        {
            colorSetter.ResetColor();
        }

        protected virtual void ShowActionIcon()
        {
            actionIconSR.sprite = GetActionIcon;
            actionIconSR.color = colors.PlayerColor;
            actionIconAnimator.SetInteger(int_mode, (int)GetActionIconAnimation);
            actionIconAnimator.SetTrigger(tri_transition);
        }

        protected void HideActionIcon()
        {
            actionIconAnimator.SetInteger(int_mode, (int)icons.HideAnimation);
            actionIconAnimator.SetTrigger(tri_transition);
        }

        protected virtual void AddEnemyInDetectArea(HanakoEnemy enemy, bool isHighligthing = true)
        {
            if (!enemy.IsAlive || !enemy.IsKillable) return;
            enemiesInDetectArea.Add(enemy);
            if (isHovering && isHighligthing)
                enemy.Highlight(detectAreaHighlightMode);
        }

        protected virtual void RemoveEnemyInDetectArea(HanakoEnemy enemy)
        {
            enemiesInDetectArea.Remove(enemy);
            if (isHovering)
                enemy.Highlight(HighlightMode.None);
        }
    }
}
