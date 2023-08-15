using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    [InlineEditor]
    [CreateAssetMenu(fileName ="HanakoIcons", menuName ="SO/Hanako/Icons")]

    public class HanakoIcons : ScriptableObject
    {
        public enum ActionIconMode { Hide, Tilting, Nodding, Beating }

        [SerializeField, PreviewField]
        Sprite arrownDownIcon;

        [SerializeField]
        ActionIconMode arrowDownAnimation = ActionIconMode.Nodding;

        [SerializeField, PreviewField]
        Sprite warningIcon;

        [SerializeField]
        ActionIconMode warningAnimation = ActionIconMode.Tilting;
        
        [SerializeField, PreviewField]
        Sprite skullIcon;

        [SerializeField]
        ActionIconMode skullAnimation = ActionIconMode.Tilting;
        
        [SerializeField, PreviewField]
        Sprite attackIcon;

        [SerializeField]
        ActionIconMode attackAnimation = ActionIconMode.Tilting;

        [SerializeField, PreviewField]
        Sprite okCircleIcon;

        [SerializeField]
        ActionIconMode okCircleAnimation = ActionIconMode.Tilting;

        [SerializeField, PreviewField]
        Sprite distractionIcon;

        [SerializeField]
        ActionIconMode distractionAnimation = ActionIconMode.Beating;

        public Sprite ArrownDownIcon { get => arrownDownIcon; }
        public ActionIconMode ArrowDownAnimation { get => arrowDownAnimation; }
        public Sprite WarningIcon { get => warningIcon; }
        public ActionIconMode WarningAnimation { get => warningAnimation; }
        public Sprite SkullIcon { get => skullIcon; }
        public ActionIconMode SkullAnimation { get => skullAnimation; }
        public Sprite AttackIcon { get => attackIcon; }
        public ActionIconMode AttackAnimation { get => attackAnimation; }
        public Sprite OkCircleIcon { get => okCircleIcon; }
        public ActionIconMode OkCircleAnimation { get => okCircleAnimation; }
        public ActionIconMode HideAnimation => ActionIconMode.Hide;

        public Sprite DistractionIcon { get => distractionIcon; }
        public ActionIconMode DistractionAnimation { get => distractionAnimation; }
    }
}
