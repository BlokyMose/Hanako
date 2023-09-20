using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako
{
    public class ScoreDetailText : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI text;

        public void Init(string text, Color textColor)
        {
            this.text.text = text;
            this.text.color = textColor;
        }
    }
}
