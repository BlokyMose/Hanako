using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtility;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    public class HanakoEnemyList : MonoBehaviour
    {
        [SerializeField]
        int previewPanelCount = 3;

        [SerializeField]
        float minScale = 0.66f;

        [SerializeField]
        float maxScale = 1f;

        [SerializeField]
        float scaleSpeed = 0.5f;

        [Header("Components")]
        [SerializeField]
        HanakoEnemyPanel panelPrefab;

        [SerializeField]
        Transform panelParent;

        List<HanakoEnemyPanel> panels = new();

        private void Awake()
        {
            panelParent.DestroyChildren();
        }

        /// <summary>
        /// Preview the first enemies 
        /// </summary>
        public void Init(HanakoEnemySequence sequence, int previewPanelCount)
        {
            if (sequence.Sequence.Count == 0) return;
            this.previewPanelCount = previewPanelCount;

            panelParent.DestroyChildren();

            var actualPanelCount = previewPanelCount;
            if (sequence.Sequence.Count < actualPanelCount)
                actualPanelCount = sequence.Sequence.Count;

            var incrementScale = (maxScale - minScale)/actualPanelCount;

            for (int i = 0; i < actualPanelCount; i++)
            {
                var enemy = sequence.Sequence[i];
                AddPanel(enemy.ID, enemy.DestinationSequence, minScale + incrementScale * (actualPanelCount - i));
            }

            panels[0].PlayScaleAnmation(maxScale, scaleSpeed);
        }

        public void RemoveFirstPanel()
        {
            if (panels.Count == 0) return;
            panels[0].HideAndDestroy();
            panels.RemoveAt(0);
        }

        public void StartLoadingBarOfFirstPanel(float duration, Color color)
        {
            panels[0].FillLoadingBar(duration, color);
        }

        public void AddPanel(HanakoEnemyID id, List<DestinationProperties> destinations, float? initScale = null)
        {
            var panel = Instantiate(panelPrefab, panelParent);
            panel.Init(id, destinations, (float)(initScale != null ? initScale : this.minScale)) ;
            panels.Add(panel);
            panel.transform.SetAsFirstSibling();
        }

        public void IncrementPanelsScale()
        {
            var actualPreviewPanelCount = previewPanelCount;
            if (panels.Count < actualPreviewPanelCount)
                actualPreviewPanelCount = panels.Count;

            var incrementScale = (maxScale - minScale) / actualPreviewPanelCount;
            for (int i = 0; i < actualPreviewPanelCount; i++)
                panels[i].PlayScaleAnmation(minScale + incrementScale * (actualPreviewPanelCount-i), scaleSpeed);
        }
    }
}
