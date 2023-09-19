using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako
{
    public class AllGamesCanvas : MonoBehaviour
    {
        [SerializeField]
        AllGamesInfo allGamesInfo;

        [Header("Components")]
        [SerializeField]
        TextMeshProUGUI soulCountText;

        [SerializeField]
        TextMeshProUGUI playTimeText;

        void Awake()
        {
            soulCountText.text = allGamesInfo.CurrentSoulCount.ToString();
        }

        void Update()
        {
            playTimeText.text = MathUtility.SecondsToTimeString(allGamesInfo.PlayTime);
        }
    }
}
