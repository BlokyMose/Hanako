using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hanako.Dialogue
{
    [RequireComponent(typeof(Animator))] 
    public class DialoguePreviewCanvas : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI charNameText;

        int boo_show;

        Animator animator;
        void Awake()
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
        }

        public void Show(CharID charID)
        {
            charNameText.text = !string.IsNullOrEmpty(charID.DisplayName) ? charID.DisplayName : charID.CharName;
            animator.SetBool(boo_show, true);
        }

        public void Hide()
        {
            animator.SetBool(boo_show, false);
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
