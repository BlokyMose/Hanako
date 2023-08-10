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
        HanakoLevelManager levelManager;
        float possessCooldown = 0f;

        public HanakoDestination_Toilet PossessedToilet { get => possessedToilet; }

        public void Init(HanakoLevelManager levelManager, HanakoDestination_Toilet initialPossessedToilet, Action<HanakoDestination_Toilet, HanakoDestination_Toilet> onPossess)
        {
            this.possessedToilet = initialPossessedToilet;
            OnPossess += onPossess;
            this.levelManager = levelManager;

            StartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    this.possessCooldown -= Time.deltaTime;
                    yield return null;
                }
            }
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
            if (collision.TryGetComponentInFamily<HanakoDestination_Toilet>(out var toilet))
            {
                if (toilet == hoveredToilet)
                    Unhover(toilet);
            }
        }

        public override void ClickState(bool isClicking)
        {
            base.ClickState(isClicking);
            if (isClicking)
            {
                if (possessedToilet != null &&
                    hoveredToilet != null)
                {
                    if (hoveredToilet.Occupation == HanakoDestination.OccupationMode.Unoccupied)
                    {
                        if (possessCooldown > 0f) return;

                        possessCooldown = levelManager.HanakoMoveDuration;
                        OnPossess?.Invoke(possessedToilet, hoveredToilet);
                        possessedToilet.Dispossess();
                        possessedToilet = hoveredToilet;
                        possessedToilet.Possess(levelManager.HanakoMoveDuration);
                    }
                    else if (possessedToilet == hoveredToilet)
                    {
                        possessCooldown = levelManager.AttackCooldown/2f;
                        possessedToilet.Attack(levelManager.AttackCooldown, levelManager.EnemyReceiveAttackDelay);
                    }
                }
            }
        }

        void Hover(HanakoDestination_Toilet toilet)
        {
            if (hoveredToilet != null)
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
