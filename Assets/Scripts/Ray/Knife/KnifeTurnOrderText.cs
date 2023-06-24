using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifeTurnOrderText : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI text;

        [SerializeField]
        CanvasGroup canvasGroup;

        public void Init(Vector2 offset)
        {
            transform.localPosition = offset;
        }

        public void SetText(string text)
        {
            this.text.text = text;
            transform.eulerAngles = Vector3.zero;
        }

        public string GetText()
        {
            return this.text.text;
        }

        public void ClearText()
        {
            this.text.text = "";
        }

        public void Show()
        {
            canvasGroup.alpha = 0.75f;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
        }
    }
}
