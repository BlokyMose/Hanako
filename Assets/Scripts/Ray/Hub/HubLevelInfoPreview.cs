using FlyingWormConsole3.LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using static Hanako.Hub.HubLevelCanvas;

namespace Hanako.Hub
{
    public class HubLevelInfoPreview : MonoBehaviour
    {
        [SerializeField]
        string levelNamePrefix = "Lvl: ";

        [Header("Components")]
        [SerializeField]
        TextMeshProUGUI levelName;

        [SerializeField]
        Image titleIcon;

        [SerializeField]
        Transform soulIconParent;

        [SerializeField]
        Animator soulIconPrefab;

        Animator animator;
        int int_mode, boo_show;

        public void Init(LevelInfo levelInfo)
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            boo_show = Animator.StringToHash(nameof(boo_show));
            animator.SetBool(boo_show, true);

            titleIcon.sprite = levelInfo.GameInfo.TitleIcon;
            levelName.text = levelNamePrefix + levelInfo.LevelName;

            soulIconParent.DestroyChildren();
            for (int i = 0; i < levelInfo.MaxSoulCount; i++)
            {
                var soulIconAnimator = Instantiate(soulIconPrefab, soulIconParent);
                if (i <= levelInfo.CurrentSoulCount - 1)
                    soulIconAnimator.SetInteger(int_mode, (int)SoulIconState.Alive);
            }
        }

        public void Exit()
        {
            animator.SetBool(boo_show, false);
            Destroy(gameObject, 1f);
        }
    }
}
