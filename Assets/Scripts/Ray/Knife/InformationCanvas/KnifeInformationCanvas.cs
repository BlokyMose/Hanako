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
        KnifeInformationPanel interactionPanelPrefab;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup allPanelsParent;

        [SerializeField]
        HorizontalOrVerticalLayoutGroup interactionPanelsParent;

        public KnifeInformationPanel ProfilePanel { get => profilePanel; }

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
                    var infoPanel = Instantiate(interactionPanelPrefab, interactionPanelsParent.transform);
                    var interactionInfo = interaction.GetInformation();
                    infoPanel.SetInformation(new(interactionInfo.Name, interactionInfo.Desc, interactionInfo.Logo));
                    //RefreshCanvas();
                }
            }

            RefreshCanvas();

            StartCoroutine(Delay(0.2f));
            IEnumerator Delay(float delay)
            {
                yield return null;
                RefreshCanvas();
            }

        }

        public void Clear()
        {
            profilePanel.Clear();
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
