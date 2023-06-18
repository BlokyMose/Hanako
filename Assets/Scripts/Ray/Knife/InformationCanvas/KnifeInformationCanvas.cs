using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using static Hanako.Knife.KnifeInformationPanel;

namespace Hanako.Knife
{
    public class KnifeInformationCanvas : MonoBehaviour
    {
        [SerializeField]
        Camera profileCamera;

        [SerializeField]
        KnifeInformationPanel profilePanel;

        [SerializeField]
        RectTransform interactionPanelsParent;

        [SerializeField]
        KnifeInformationPanel interactionPanelPrefab;

        public KnifeInformationPanel ProfilePanel { get => profilePanel; }

        Transform profileCameraPos;
        Vector3 profileCameraOffset;

        void Awake()
        {
            interactionPanelsParent.transform.DestroyChildren();
        }

        public void SetInformation(KnifePiece knifePiece, bool headLogoFlipX = false)
        {
            var info = knifePiece.Information;
            if (info != null)
            {
                var headPosOffset = new Vector3((headLogoFlipX ? -1 : 1) * knifePiece.HeadPosOffset.x, knifePiece.HeadPosOffset.y, knifePiece.HeadPosOffset.z);
                profilePanel.SetInformation(new(info.PieceName, info.Desc, info.Logo));
                profileCameraPos = knifePiece.HeadPosForLogo;
                profileCameraOffset = headPosOffset;
                profilePanel.FlipXLogo(headLogoFlipX);
            }

            foreach (var interactionProperties in knifePiece.Interactions)
            {
                foreach (var interaction in interactionProperties.Interactions)
                {
                    var infoPanel = Instantiate(interactionPanelPrefab, interactionPanelsParent.transform);
                    var interactionInfo = interaction.GetInformation();
                    infoPanel.SetInformation(new(interactionInfo.Name, interactionInfo.Desc, interactionInfo.Logo));
                }
            }
            Canvas.ForceUpdateCanvases();
        }

        public void Clear()
        {
            profilePanel.Clear();
            interactionPanelsParent.transform.DestroyChildren();
        }

        void Update()
        {
            if (profileCameraPos != null)
                profileCamera.transform.position = profileCameraPos.position + profileCameraOffset; 
        }
    }
}
