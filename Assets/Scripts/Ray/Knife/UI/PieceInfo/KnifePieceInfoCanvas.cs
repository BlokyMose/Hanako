using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using static Hanako.Knife.KnifeInteraction.Information;
using static Hanako.Knife.KnifePieceInfoPanel;

namespace Hanako.Knife
{
    public class KnifePieceInfoCanvas : MonoBehaviour
    {
        [System.Serializable]
        public class DefaultInfo
        {
            [SerializeField]
            Information info;

            [SerializeField]
            Transform logoTarget;

            [SerializeField]
            Vector3 offset = new Vector3(0, 0, -20);

            public Information Info { get => info; }
            public Transform LogoPos { get => logoTarget; }
            public Vector3 Offset { get => offset; }
        }

        [SerializeField]
        Camera profileCamera;

        [SerializeField]
        KnifePieceInfoPanel profilePanel;

        [SerializeField]
        List<KnifePieceInfoPanel> interactionPanels = new();

        [SerializeField]
        HorizontalOrVerticalLayoutGroup allPanelsParent;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup interactionsParent;

        [Header("Customizations")]
        [SerializeField]
        List<DefaultInfo> defaultInfos = new();

        [SerializeField]
        DefaultInfo moveableTileInfo = new();

        [SerializeField]
        DefaultInfo noTileSelectedInfo = new();

        [SerializeField]
        KnifeInteraction.Information moveInteractionInfo = new();

        [SerializeField]
        KnifeInteraction.Information invalidMoveInteractionInfo = new();

        public KnifePieceInfoPanel ProfilePanel { get => profilePanel; }

        [Header("Animation")]
        [SerializeField]
        Animator animator;

        [SerializeField]
        int transitionVariantCount = 3;

        int tri_transition, int_variant;
        Transform profileCameraPos;
        Vector3 profileCameraOffset;
        List<HorizontalOrVerticalLayoutGroup> layoutGroups = new();

        void Awake()
        {
            layoutGroups = new() { interactionsParent, allPanelsParent };
            tri_transition = Animator.StringToHash(nameof(tri_transition));
            int_variant = Animator.StringToHash(nameof(int_variant));
        }

        void Start()
        {
            SetInformationOnNoTileSelected();
        }

        public void SetInformation(KnifePiece knifePiece, bool isMoveable, bool headLogoFlipX = false)
        {
            PlayAnimationTransition();
            DeactivateAllInteractionInfoPanels();
            if (!isMoveable)
                SetInteractionInfoOnInvalidMove();

            var info = knifePiece.Information;
            if (info != null)
            {
                var headPosOffset = new Vector3((headLogoFlipX ? -1 : 1) * knifePiece.HeadPosOffset.x, knifePiece.HeadPosOffset.y, knifePiece.HeadPosOffset.z);
                profilePanel.SetInformation(new(info));
                profileCameraPos = knifePiece.HeadPosForLogo;
                profileCameraOffset = headPosOffset;
                profilePanel.FlipXLogo(headLogoFlipX);
            }

            var interactionPanelIndex = 0;
            foreach (var interactionProperties in knifePiece.Interactions)
            {
                foreach (var interaction in interactionProperties.Interactions)
                {
                    var interactionInfo = interaction.GetInformation();
                    if (interactionPanelIndex < interactionPanels.Count && (
                        interactionInfo.ShowMode == InformationShowMode.AlwaysShow ||
                        (interactionInfo.ShowMode == InformationShowMode.ShowIfValid && isMoveable))
                        )
                    {
                        interactionPanels[interactionPanelIndex].gameObject.SetActive(true);
                        interactionPanels[interactionPanelIndex].SetInformation(new(interactionInfo));
                        interactionPanelIndex++;
                    }
                }
            }

            StartCoroutine(Delay(0.05f));
            IEnumerator Delay(float delay)
            {
                RefreshCanvas();
                yield return new WaitForSeconds(delay);
                RefreshCanvas();
            }
        }

        private void DeactivateAllInteractionInfoPanels()
        {
            foreach (var interactionPanel in interactionPanels)
                interactionPanel.gameObject.SetActive(false);
        }

        public void SetInformationOnEmptyUnmoveableTile()
        {
            PlayAnimationTransition();
            SetInteractionInfoOnInvalidMove();

            var useFirstIndex = Random.Range(0, 10) < 9;
            var defaultInfo = useFirstIndex ? defaultInfos[0] : defaultInfos.GetRandom();
            profilePanel.SetInformation(defaultInfo.Info);
            profileCameraPos = defaultInfo.LogoPos;
            profileCameraOffset = defaultInfo.Offset;
            RefreshCanvas();
        }
        
        public void SetInformationOnEmptyMoveableTile()
        {
            PlayAnimationTransition();
            SetInteractionInfoOnMoveable();

            profilePanel.SetInformation(moveableTileInfo.Info);
            profileCameraPos = moveableTileInfo.LogoPos;
            profileCameraOffset = moveableTileInfo.Offset;

            RefreshCanvas();
        }

        public void SetInformationOnNoTileSelected()
        {
            PlayAnimationTransition();
            DeactivateAllInteractionInfoPanels();

            profilePanel.SetInformation(noTileSelectedInfo.Info);
            profileCameraPos = noTileSelectedInfo.LogoPos;
            profileCameraOffset = noTileSelectedInfo.Offset;
            RefreshCanvas();
        }

        public void SetInteractionInfoOnMoveable()
        {
            DeactivateAllInteractionInfoPanels();

            if (moveInteractionInfo.ShowMode == InformationShowMode.AlwaysShow)
            {
                interactionPanels[0].gameObject.SetActive(true);
                interactionPanels[0].SetInformation(new(moveInteractionInfo));
            }
        }

        public void SetInteractionInfoOnInvalidMove()
        {
            DeactivateAllInteractionInfoPanels();

            if (invalidMoveInteractionInfo.ShowMode == InformationShowMode.AlwaysShow)
            {
                interactionPanels[0].gameObject.SetActive(true);
                interactionPanels[0].SetInformation(new(invalidMoveInteractionInfo));
            }
        }

        public void RefreshCanvas()
        {
            Canvas.ForceUpdateCanvases();
            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = false;
            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = true;
        }

        void PlayAnimationTransition()
        {
            animator.SetInteger(int_variant, Random.Range(0, transitionVariantCount));
            animator.SetTrigger(tri_transition);
        }

        void Update()
        {
            if (profileCameraPos != null)
                profileCamera.transform.position = profileCameraPos.position + profileCameraOffset; 
        }
    }
}
