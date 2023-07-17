using Hanako.Knife;
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
                if (hoveredToilet != null && !hoveredToilet.IsOccupied)
                {
                    if (possessedToilet != null)
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
            toilet.Unhover();
        }
    }
}
