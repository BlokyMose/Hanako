using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako.Hanako
{
    public class HanakoGameInfoCanvas : MonoBehaviour
    {
        [SerializeField]
        Animator killCountAnimator;

        [SerializeField]
        TextMeshProUGUI killCountText;

        [SerializeField]
        string killCountPlaceholder = "{killCount}";

        [SerializeField]
        string enemyCountPlaceholder = "{enemyCount}";

        [SerializeField]
        string killCountTextFormat = "{killCount}<size=75><alpha=#AA>/{enemyCount}";

        int tri_beat;

        void Awake()
        {
            tri_beat = Animator.StringToHash(nameof(tri_beat));
        }

        public void Init(int enemyCount, ref Action<int,int> onAddKillCount, int killCount = 0)
        {
            SetText(killCount, enemyCount);
            onAddKillCount += SetText;
        }


        public void SetText(int killCount, int enemyCount)
        {
            var text = killCountTextFormat.Replace(killCountPlaceholder.ToString(), killCount.ToString());
            text = text.Replace(enemyCountPlaceholder.ToString(), enemyCount.ToString());
            killCountText.text = text;
            killCountAnimator.SetTrigger(tri_beat);
        }
    }
}
