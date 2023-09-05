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
        [SerializeField, Tooltip("When loading a new scene, use levelInfo to the game")]
        bool isUsingLevelInfo = true;

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

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            if (isUsingLevelInfo) sceneLoadingData.Activate(); 
            else sceneLoadingData.Deactivate();

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        void Start()
        {
            Hide();
        }

        [Button]
        public void LoadScene(LevelInfo levelInfo)
        {
            if (isLoading) return;
            isLoading = true;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            StartCoroutine(Delay(toBlackDuration));
            IEnumerator Delay(float delay)
            {
                sceneLoadingData.SetData(levelInfo);
                Instantiate(levelInfo.GameInfo.LoadingAnimation, transform);
                yield return new WaitForSeconds(delay);
                SceneManager.LoadScene(levelInfo.GameInfo.SceneName);
            }
        }

        public void Hide()
        {
            var loadingAnimationGO = Instantiate(sceneLoadingData.LevelInfo != null
                ? sceneLoadingData.LevelInfo.GameInfo.LoadingAnimation
                : defaultLoadingAnimation,
                transform);
            var loadingAnimator = loadingAnimationGO.GetComponent<Animator>();
            loadingAnimator.SetBool(boo_show, false);
            Destroy(loadingAnimationGO, toBlackDuration + 1f);
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            bg.SetActive(false);
        }
    }
}
