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
        [SerializeField]
        Camera profileCamera;

        [SerializeField]
        KnifePieceInfoPanel profilePanel;

        [SerializeField]
        KnifePieceInfoPanel interactionPanelPrefab;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup allPanelsParent;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup interactionPanelsParent;

        public KnifePieceInfoPanel ProfilePanel { get => profilePanel; }

        Transform profileCameraPos;
        Vector3 profileCameraOffset;

        void Awake()
        {
            Clear();
        }

        public void SetInformation(KnifePiece knifePiece, bool headLogoFlipX = false)
        {
            var info = knifePiece.Information;
            if (info != null)
            {
                profilePanel.gameObject.SetActive(true);

                var headPosOffset = new Vector3((headLogoFlipX ? -1 : 1) * knifePiece.HeadPosOffset.x, knifePiece.HeadPosOffset.y, knifePiece.HeadPosOffset.z);
                profilePanel.SetInformation(new(info.PieceName, info.Desc, info.Logo));
                profileCameraPos = knifePiece.HeadPosForLogo;
                profileCameraOffset = headPosOffset;
                profilePanel.FlipXLogo(headLogoFlipX);
            }
            else
            {
                profilePanel.gameObject.SetActive(false);
            }

            foreach (var interactionProperties in knifePiece.Interactions)
            {
                foreach (var interaction in interactionProperties.Interactions)
                {
                    var interactionInfo = interaction.GetInformation();
                    if (interactionInfo.ShowMode == KnifeInteraction.Information.InformationShowMode.Panel)
                    {
                        var infoPanel = Instantiate(interactionPanelPrefab, interactionPanelsParent.transform);
                        infoPanel.SetInformation(new(interactionInfo.Name, interactionInfo.Desc, interactionInfo.Logo));
                    }
                }
            }

            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return null;
                yield return null;
                RefreshCanvas();
            }

        }

        public void Clear()
        {
            profilePanel.Clear();
            profilePanel.gameObject.SetActive(false);
            interactionPanelsParent.transform.DestroyChildren();
            //RefreshCanvas();

        }

        public void RefreshCanvas()
        {
            Canvas.ForceUpdateCanvases();
            interactionPanelsParent.enabled = false;
            interactionPanelsParent.enabled = true;
            allPanelsParent.enabled = false;
            allPanelsParent.enabled = true;
        }


        void Update()
        {
            if (profileCameraPos != null)
                profileCamera.transform.position = profileCameraPos.position + profileCameraOffset; 
        }
    }
}
