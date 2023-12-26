using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako
{
    public class DebugAllGamesInfo : MonoBehaviour
    {
        public AllGamesInfo allGamesInfo;
        public TextMeshProUGUI text;

        public void DebugLog()
        {
            if (text != null)
                text.text = allGamesInfo.PlayTime.ToString();

            Debug.Log(nameof(allGamesInfo.PlayTime) + " : " + allGamesInfo.PlayTime);
        }
    }
}
