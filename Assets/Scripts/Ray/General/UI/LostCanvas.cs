using FlyingWormConsole3.LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako
{
    public class LostCanvas : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField]
        Image againBut;

        [SerializeField]
        Image hubBut;

        [SerializeField]
        AudioSourceRandom audioSource;

        [SerializeField]
        string sfxHoverName = "sfxHover";

        [SerializeField]
        string sfxClickName = "sfxClick";

        Animator againButAnimator, hubButAnimator;
        int int_mode;

        private void Awake()
        {
            int_mode = Animator.StringToHash(nameof(int_mode));
            againButAnimator = againBut.GetComponent<Animator>();
            againBut.AddEventTriggers(
                onEnter: OnEnterAgainBut,
                onExit: OnExitAgainBut,
                onDown: OnDownAgainBut,
                onClick: OnClickAgainBut
                );

            hubButAnimator = hubBut.GetComponent<Animator>();
            hubBut.AddEventTriggers(
                onEnter: OnEnterHubBut,
                onExit: OnExitHubBut,
                onDown: OnDownHubBut,
                onClick: OnClickHubBut
                );

            #region [Methods: Again But]


            void OnEnterAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Hover);
                audioSource.PlayOneClipFromPack(sfxHoverName);
            }

            void OnExitAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                audioSource.PlayOneClipFromPack(sfxHoverName);
            }

            void OnDownAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);
                audioSource.PlayOneClipFromPack(sfxClickName);
            }

            void OnClickAgainBut()
            {
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                var sceneLoading = FindObjectOfType<SceneLoadingManager>();
                var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
                if (sceneLoading != null && allGamesInfo != null)
                    sceneLoading.LoadScene(allGamesInfo.AllGamesInfo.CurrentLevel);
            }

            #endregion

            #region [Methods: Hub But]


            void OnEnterHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Hover);
                audioSource.PlayOneClipFromPack(sfxHoverName);
            }

            void OnExitHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                audioSource.PlayOneClipFromPack(sfxHoverName);
            }

            void OnDownHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Pressed);
                audioSource.PlayOneClipFromPack(sfxClickName);
            }

            void OnClickHubBut()
            {
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                var sceneLoading = FindObjectOfType<SceneLoadingManager>();
                var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
                if (sceneLoading != null && allGamesInfo != null)
                    sceneLoading.LoadScene(allGamesInfo.AllGamesInfo.HubLevelInfo);
            }

            #endregion
        }

        private void Start()
        {
            StartCoroutine(DelayButAnimation(1f));
            IEnumerator DelayButAnimation(float delay)
            {
                yield return new WaitForSeconds(delay);
                againButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
                hubButAnimator.SetInteger(int_mode, (int)SolidButtonState.Idle);
            }
        }
    }
}
