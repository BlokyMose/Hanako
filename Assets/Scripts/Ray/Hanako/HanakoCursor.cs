using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Hanako.HanakoDestination;

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
            if (collision.TryGetComponent<HanakoDestination_Toilet>(out var toilet))
            {
                Hover(toilet);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<HanakoDestination_Toilet>(out var toilet))
            {
                if (toilet == hoveredToilet)
                    Unhover(toilet);
            }
        }

        public override void ClickState(bool isClicking)
        {
            base.ClickState(isClicking);
            if (!isClicking || hoveredToilet == null) return;

            if (possessedToilet == hoveredToilet)
                AttackFromPossessedToilet();

            else if (hoveredToilet.Occupation == OccupationMode.Unoccupied && possessCooldown < 0f)
                PossessToilet(hoveredToilet);
        }

        private void AttackFromPossessedToilet()
        {
            possessCooldown = levelManager.AttackCooldown / 2f;
            possessedToilet.Attack(levelManager.AttackCooldown, levelManager.EnemyReceiveAttackDelay);
        }

        void PossessToilet(HanakoDestination_Toilet targetToilet)
        {
            possessCooldown = levelManager.HanakoMoveDuration;
            OnPossess?.Invoke(possessedToilet, targetToilet);
            possessedToilet.Dispossess();
            possessedToilet = targetToilet;
            possessedToilet.Possess(levelManager.HanakoMoveDuration);
            possessedToilet.HighlightDetectingEnemies();
        }

        void Hover(HanakoDestination_Toilet toilet)
        {
            if (hoveredToilet != null && hoveredToilet != toilet)
                Unhover(hoveredToilet);

            hoveredToilet = toilet;
            hoveredToilet.Hover();
            if (possessedToilet == hoveredToilet)
                possessedToilet.HighlightDetectingEnemies();
        }

        void Unhover(HanakoDestination_Toilet toilet)
        {
            if (possessedToilet == toilet)
                possessedToilet.ResetHighlightEnemies();
            hoveredToilet = null;
            toilet.Unhover();
        }
    }
}
