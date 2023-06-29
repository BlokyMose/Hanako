using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
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
        KnifePieceInfoPanel interactionPanelPrefab;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup allPanelsParent;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup interactionsParent;

        [Header("Customizations")]
        [SerializeField]
        List<DefaultInfo> defaultInfos = new();

        public KnifePieceInfoPanel ProfilePanel { get => profilePanel; }

        Transform profileCameraPos;
        Vector3 profileCameraOffset;
        List<HorizontalOrVerticalLayoutGroup> layoutGroups = new();

        void Awake()
        {
            layoutGroups = new() { interactionsParent, allPanelsParent };
        }

        void Start()
        {
            SetDefaultInfo();
        }

        public void SetInformation(KnifePiece knifePiece, bool headLogoFlipX = false)
        {
            var info = knifePiece.Information;
            if (info != null)
            {
                var headPosOffset = new Vector3((headLogoFlipX ? -1 : 1) * knifePiece.HeadPosOffset.x, knifePiece.HeadPosOffset.y, knifePiece.HeadPosOffset.z);
                profilePanel.SetInformation(new(info.PieceName, info.Desc, info.Color));
                profileCameraPos = knifePiece.HeadPosForLogo;
                profileCameraOffset = headPosOffset;
                profilePanel.FlipXLogo(headLogoFlipX);
            }
            else
            {
            }

            foreach (var interactionProperties in knifePiece.Interactions)
            {
                foreach (var interaction in interactionProperties.Interactions)
                {
                    var interactionInfo = interaction.GetInformation();
                    if (interactionInfo.ShowMode == KnifeInteraction.Information.InformationShowMode.Panel)
                    {
                        var infoPanel = Instantiate(interactionPanelPrefab, interactionsParent.transform);
                        infoPanel.SetInformation(new(interactionInfo.Name, interactionInfo.Desc));
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

        public void SetDefaultInfo()
        {
            interactionsParent.transform.DestroyChildren();
            var useFirstIndex = Random.Range(0, 10) < 9;
            var defaultInfo = useFirstIndex ? defaultInfos[0] : defaultInfos.GetRandom();
            profilePanel.SetInformation(defaultInfo.Info);
            profileCameraPos = defaultInfo.LogoPos;
            profileCameraOffset = defaultInfo.Offset;
            RefreshCanvas();
        }

        public void RefreshCanvas()
        {
            Canvas.ForceUpdateCanvases();
            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = false;
            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = true;
        }


        void Update()
        {
            if (profileCameraPos != null)
                profileCamera.transform.position = profileCameraPos.position + profileCameraOffset; 
        }
    }
}
