using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako
{
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class LoginCanvas : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField nameInputField;

        [SerializeField]
        Image playBut;

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;
        AllGamesInfo allGamesInfo;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            playBut.AddEventTrigger(OnClick);

            var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfoManager != null)
                allGamesInfo = allGamesInfoManager.AllGamesInfo;
        }

        void OnClick()
        {
            if (allGamesInfo != null)
            {
                var id = string.IsNullOrEmpty(nameInputField.text) ? "anon" : nameInputField.text;
                var displayName = id;
                allGamesInfo.Login(new(id, displayName));
            }

            Hide();
        }

        public void Show()
        {
            if (allGamesInfo.CurrentPlayerID != null)
                nameInputField.text = allGamesInfo.CurrentPlayerID.DisplayName;

            animator.SetBool(boo_show, true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            animator.SetBool(boo_show, false);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
