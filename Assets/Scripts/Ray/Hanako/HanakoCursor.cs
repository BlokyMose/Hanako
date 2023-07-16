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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isClicking) return;

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

        void Hover(HanakoDestination_Toilet toilet)
        {
            hoveredToilet = toilet;
            toilet.Hover();
        }

        void Unhover(HanakoDestination_Toilet toilet)
        {
            toilet.Unhover();
        }
    }
}
