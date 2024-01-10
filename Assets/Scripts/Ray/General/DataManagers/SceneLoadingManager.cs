using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hanako
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneLoadingManager : MonoBehaviour
    {
        [SerializeField]
        SceneLoadingData sceneLoadingData;

        [SerializeField]
        GameObject bg;

        [SerializeField]
        GameObject defaultLoadingAnimation;

        [SerializeField]
        float toBlackDuration = 0.33f;

        CanvasGroup canvasGroup;
        int boo_show;
        bool isLoading;

        public SceneLoadingData SceneLoadingData { get => sceneLoadingData;  }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Hide();
        }

        [Button]
        public void LoadScene(LevelInfo levelInfo)
        {
            if (isLoading) return;
            isLoading = true;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfo != null)
                StartCoroutine(allGamesInfo.AllGamesInfo.FadeMasterVolume(0f, toBlackDuration));

            StartCoroutine(Delay(toBlackDuration));
            IEnumerator Delay(float delay)
            {
                sceneLoadingData.SetData(levelInfo);
                Instantiate(levelInfo.GameInfo.LoadingAnimation, transform);
                yield return new WaitForSeconds(delay);
                SceneManager.LoadScene(levelInfo.SceneName);
            }
        }

        public void Hide()
        {
            var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfo != null)
            {
                if (sceneLoadingData.LevelInfoToLoad != null)
                    allGamesInfo.AllGamesInfo.SetCurrentLevel(sceneLoadingData.LevelInfoToLoad);
                StartCoroutine(allGamesInfo.AllGamesInfo.FadeMasterVolume(1f, toBlackDuration));
            }

            var loadingAnimationGO = Instantiate(sceneLoadingData.LevelInfoToLoad != null
                ? sceneLoadingData.LevelInfoToLoad.GameInfo.LoadingAnimation
                : defaultLoadingAnimation,
                transform);
            var loadingAnimator = loadingAnimationGO.GetComponent<Animator>();
            loadingAnimator.SetBool(boo_show, false);
            Destroy(loadingAnimationGO, toBlackDuration + 1f);
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            bg.SetActive(false);
        }

        public void ResetData() => sceneLoadingData.ResetData();
    }
}
