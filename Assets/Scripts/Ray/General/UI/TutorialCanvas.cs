using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako
{
    [RequireComponent(typeof(Animator),typeof(CanvasGroup),typeof(Canvas))]
    public class TutorialCanvas : MonoBehaviour
    {
        enum TutButAnimation { Hidden = -1, Idle, Hover, Click }
        enum TutPageAnimation { LeftToMid = -2, MidToLeft = -1, Idle = 0, MidToRight = 1, RightToMid = 2 }

        [SerializeField]
        float pageTransitionDuration = 0.6f;

        [SerializeField]
        float showDuration = 0.6f;

        [SerializeField]
        List<GameObject> tutPagePrefabs = new();

        [Header("Components")]
        [SerializeField]
        Transform tutPagesParent;

        [SerializeField]
        Image closeBut;

        [SerializeField]
        Image nextBut;

        [SerializeField]
        Image backBut;

        [SerializeField]
        Image checkBut;

        Animator animator, nextButAnimator, backButAnimator, checkButAnimator, closeButAnimator;
        Canvas canvas;
        CanvasGroup canvasGroup;
        List<GameObject> tutPages = new();
        int tutPagesIndex = 0;
        int int_mode, boo_show;
        bool isShowing = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            canvasGroup = GetComponent<CanvasGroup>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            boo_show = Animator.StringToHash(nameof(boo_show));

            SetupButton();

            void SetupButton()
            {
                nextButAnimator = nextBut.GetComponent<Animator>();
                nextBut.AddEventTriggers(
                    onClick: () =>  { if (tutPagesIndex < tutPagePrefabs.Count - 1) NextPage(); },
                    onEnter: () =>  { if (tutPagesIndex < tutPagePrefabs.Count - 1) OnEnter(nextButAnimator); },
                    onExit: () =>   { if (tutPagesIndex < tutPagePrefabs.Count - 1) OnExit(nextButAnimator); },
                    onDown: () =>   { if (tutPagesIndex < tutPagePrefabs.Count - 1) OnDown(nextButAnimator); },
                    onUp: () =>     { if (tutPagesIndex < tutPagePrefabs.Count - 1) OnExit(nextButAnimator); }
                    );

                backButAnimator = backBut.GetComponent<Animator>();
                backBut.AddEventTriggers(
                    onClick: () =>  { if (tutPagesIndex > 0) BackPage(); },
                    onEnter: () =>  { if (tutPagesIndex > 0) OnEnter(backButAnimator); },
                    onExit: () =>   { if (tutPagesIndex > 0) OnExit(backButAnimator); },
                    onDown: () =>   { if (tutPagesIndex > 0) OnDown(backButAnimator); },
                    onUp: () =>     { if (tutPagesIndex > 0) OnExit(backButAnimator); }
                    );

                closeButAnimator = closeBut.GetComponent<Animator>();
                closeBut.AddEventTriggers(
                    onClick: () => { Hide(); },
                    onEnter: () => { OnEnter(closeButAnimator); },
                    onExit: () => { OnExit(closeButAnimator); },
                    onDown: () => { OnDown(closeButAnimator); },
                    onUp: () => { OnExit(closeButAnimator); }
                    );

                checkButAnimator = checkBut.GetComponent<Animator>();
                checkBut.AddEventTriggers(
                    onClick: () =>  { if (tutPagesIndex >= tutPagePrefabs.Count - 1) Hide(); },
                    onEnter: () =>  { if (tutPagesIndex >= tutPagePrefabs.Count - 1) OnEnter(checkButAnimator); },
                    onExit: () =>   { if (tutPagesIndex >= tutPagePrefabs.Count - 1) OnExit(checkButAnimator); },
                    onDown: () =>   { if (tutPagesIndex >= tutPagePrefabs.Count - 1) OnDown(checkButAnimator); },
                    onUp: () =>     { if (tutPagesIndex >= tutPagePrefabs.Count - 1) OnExit(checkButAnimator); }
                    );

                

                void OnEnter(Animator animator) 
                { 
                    if (isShowing) animator.SetInteger(int_mode, (int)TutButAnimation.Hover); 
                }

                void OnExit(Animator animator)
                {
                    if (isShowing) animator.SetInteger(int_mode, (int)TutButAnimation.Idle);
                }

                void OnDown(Animator animator)
                {
                    if (isShowing) animator.SetInteger(int_mode, (int)TutButAnimation.Click);
                }
            }

        }

        void Start()
        {
            if (tutPagePrefabs.Count>0)
                Init(tutPagePrefabs, tutPagePrefabs.Count-1, 0);
        }

        public void Init(TutorialInfo tutorialInfo, TutorialPreview tutorialPreview)
        {
            Init(tutorialInfo.TutPages, tutorialPreview.EndIndex, tutorialPreview.InitialShowIndex);
        }

        public void Init(List<GameObject> pages, int lastPageIndex, int initialShowPageIndex)
        {
            if (isShowing) return;

            lastPageIndex = lastPageIndex > pages.Count - 1 ? pages.Count - 1 : lastPageIndex;
            initialShowPageIndex = initialShowPageIndex > pages.Count - 1 ? pages.Count - 1 : initialShowPageIndex;
            this.tutPagePrefabs = pages.GetRange(0, lastPageIndex + 1);
            SetupPages();
            ShowPage(initialShowPageIndex);

            isShowing = true;
            gameObject.SetActive(true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            animator.SetBool(boo_show, true);

            StartCoroutine(ShowButtons(showDuration));
            IEnumerator ShowButtons(float delay)
            {
                yield return new WaitForSeconds(delay);

                ShowBut(closeButAnimator);
                HideBut(backButAnimator);

                if (this.tutPagePrefabs.Count > 1)
                {
                    ShowBut(nextButAnimator);
                    HideBut(checkButAnimator);
                }
                else
                {
                    ShowBut(checkButAnimator);
                    HideBut(nextButAnimator);
                }
            }

            void SetupPages()
            {
                tutPagesParent.DestroyChildren();
                for (int i = 0; i <= lastPageIndex; i++)
                {
                    var tutPage = pages[i];    
                    var pageGO = Instantiate(tutPage, tutPagesParent);
                    tutPages.Add(pageGO);
                }
                var tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
                tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.RightToMid);
            }
        }

        public void Hide()
        {
            if (!isShowing) return;

            isShowing = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            animator.SetBool(boo_show, false);

            HideBut(nextButAnimator);
            HideBut(backButAnimator);
            HideBut(checkButAnimator);
            HideBut(closeButAnimator);

            Destroy(gameObject, showDuration);
        }

        void ShowPage(int index)
        {
            if (index <= 0 || index >= tutPages.Count - 1 || index == tutPagesIndex)
                return;

            var tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
            tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.MidToLeft);

            tutPagesIndex = index;

            tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
            tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.RightToMid);

            if (tutPagesIndex >= tutPages.Count - 1)
            {
                ShowBut(checkButAnimator);
                HideBut(nextButAnimator);
            }
            else
            {
                HideBut(checkButAnimator);
                ShowBut(nextButAnimator);
            }

            if (tutPagesIndex > 0)
                ShowBut(backButAnimator);
            else
                HideBut(backButAnimator);

        }

        void NextPage()
        {
            if (tutPagesIndex >= tutPages.Count - 1)
                return;

            var tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
            tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.MidToLeft);

            tutPagesIndex++;

            tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
            tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.RightToMid);

            if (tutPagesIndex >= tutPages.Count - 1)
            {
                ShowBut(checkButAnimator);
                HideBut(nextButAnimator);
            }

            if (tutPagesIndex > 0)
                ShowBut(backButAnimator);
        }

        void BackPage()
        {
            if (tutPagesIndex <= 0)
                return;

            var tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
            tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.MidToRight);

            tutPagesIndex--;

            tutPageAnimator = tutPages[tutPagesIndex].GetComponent<Animator>();
            tutPageAnimator.SetInteger(int_mode, (int)TutPageAnimation.LeftToMid);

            if (tutPagesIndex < tutPages.Count - 1)
            {
                HideBut(checkButAnimator);
                ShowBut(nextButAnimator);
            }

            if (tutPagesIndex <= 0)
                HideBut(backButAnimator);
        }

        void ShowBut(Animator animator)
        {
            animator.SetInteger(int_mode, (int)TutButAnimation.Idle);

        }

        void HideBut(Animator animator)
        {
            animator.SetInteger(int_mode, (int)TutButAnimation.Hidden);
        }
    }
}
