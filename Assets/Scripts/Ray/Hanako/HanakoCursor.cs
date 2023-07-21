using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoCursor : PlayerCursor
    {
        HanakoDestination_Toilet hoveredToilet;
        HanakoDestination_Toilet possessedToilet;
        event Action<HanakoDestination_Toilet, HanakoDestination_Toilet> OnPossess;

        public void Init(HanakoDestination_Toilet possessedToilet, Action<HanakoDestination_Toilet, HanakoDestination_Toilet> onPossess)
        {
            this.possessedToilet = possessedToilet;
            OnPossess += onPossess;

        }

        public void Exit(Action<HanakoDestination_Toilet, HanakoDestination_Toilet> onPossess)
        {
            OnPossess -= onPossess;

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponentInFamily<HanakoDestination_Toilet>(out var toilet))
            {
                Hover(toilet);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponentInFamily<HanakoDestination_Toilet>(out var toilet) && hoveredToilet == toilet)
            {
                Unhover(toilet);
            }
        }

        public override void ClickState(bool isClicking)
        {
            base.ClickState(isClicking);
            if (isClicking)
            {
                if (hoveredToilet != null && hoveredToilet.Occupation == HanakoDestination.OccupationMode.Unoccupied)
                {
                    OnPossess?.Invoke(possessedToilet, hoveredToilet);
                    possessedToilet.Dispossess();
                    possessedToilet = hoveredToilet;
                    possessedToilet.Possess();
                }
            }
        }

        void Hover(HanakoDestination_Toilet toilet)
        {
            if (hoveredToilet!=null)
                Unhover(hoveredToilet);

            hoveredToilet = toilet;
            hoveredToilet.Hover();
        }

        void Unhover(HanakoDestination_Toilet toilet)
        {
            hoveredToilet = null;
            toilet.Unhover();
        }
    }
}
