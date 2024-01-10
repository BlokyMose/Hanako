using Hanako.Hub;
using Hanako.Knife;
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

        [SerializeField]
        float autoShowAfter = 60;

        [SerializeField]
        Transform initialPos;

        bool isShowing;
        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;
        AllGamesInfo allGamesInfo;
        PlayerInputHandler playerInputHandler;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            playBut.AddEventTrigger(OnClick);

            var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfoManager != null)
                allGamesInfo = allGamesInfoManager.AllGamesInfo;

            playerInputHandler = FindObjectOfType<PlayerInputHandler>();
            if (playerInputHandler != null)
            {
                var cooldown = autoShowAfter;
                playerInputHandler.OnClickInput += () => cooldown = autoShowAfter;
                playerInputHandler.OnMoveInput += (move) => cooldown = autoShowAfter;

                if (autoShowAfter > 0)
                    StartCoroutine(AutoShowingLogin());

                IEnumerator AutoShowingLogin()
                {
                    while (true)
                    {
                        cooldown -= Time.deltaTime;
                        if (cooldown < 0)
                        {
                            nameInputField.text = "";
                            Show();
                        }

                        if (isShowing)
                            cooldown = autoShowAfter;

                        yield return null;
                    }
                }
            }
        }

        void Start()
        {
            if (allGamesInfo != null && allGamesInfo.PlayerIDs.Count == 0)
                Show();
        }

        void OnClick()
        {
            if (allGamesInfo != null)
            {
                var id = string.IsNullOrEmpty(nameInputField.text) ? "anon" : nameInputField.text;
                var displayName = id;
                var foundPlayerID = allGamesInfo.GetPlayerID(id);
                allGamesInfo.Login(new(id, displayName));

                var pauseCanvas = FindObjectOfType<PauseCanvas>();
                if (pauseCanvas != null)
                {
                    pauseCanvas.UpdatePlayerInformation();
                    if (foundPlayerID == null)
                    {
                        pauseCanvas.ShowTutorialCanvas();
                        if (initialPos != null)
                        {
                            var playerBrain = FindObjectOfType<HubCharacterBrain_Player>();
                            if (playerBrain != null)
                                playerBrain.transform.position = initialPos.position;
                        }
                    }
                }


            }

            Hide();
        }

        public void Show()
        {
            isShowing = true;
            if (allGamesInfo.CurrentPlayerID != null)
                nameInputField.text = allGamesInfo.CurrentPlayerID.DisplayName;

            animator.SetBool(boo_show, true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (playerInputHandler != null)
                playerInputHandler.DisableMove();
        }

        public void Hide()
        {
            isShowing = false;
            animator.SetBool(boo_show, false);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (playerInputHandler != null)
                playerInputHandler.EnableMove();
        }
    }
}
