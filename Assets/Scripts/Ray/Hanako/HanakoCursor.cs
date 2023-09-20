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
        [Header("Hanako")]
        [SerializeField]
        bool isInGame = false;

        IHanakoInteractableByCursor hoveredObject;
        HanakoDestination_Toilet possessedToilet;
        event Action<HanakoDestination_Toilet, HanakoDestination_Toilet> OnPossess;
        HanakoLevelManager levelManager;
        float possessCooldown = 0f;
        public HanakoDestination_Toilet PossessedToilet { get => possessedToilet; }

        public void Init(HanakoLevelManager levelManager, HanakoDestination_Toilet initialPossessedToilet, Action<HanakoDestination_Toilet, HanakoDestination_Toilet> onPossess)
        {
            OnPossess += onPossess;
            this.possessedToilet = initialPossessedToilet;
            this.levelManager = levelManager;
            isInGame = true;

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
            isInGame = false;
            StopAllCoroutines();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isInGame) return;

            if (collision.TryGetComponent<IHanakoInteractableByCursor>(out var interactableObject))
            {
                Hover(interactableObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isInGame) return;

            if (collision.TryGetComponent<IHanakoInteractableByCursor>(out var interctableObject))
            {
                if (interctableObject == hoveredObject)
                    Unhover(interctableObject);
            }
        }

        public override void ClickState(bool isClicking)
        {
            base.ClickState(isClicking);
            if (!isInGame) return;

            if (!isClicking || hoveredObject == null) return;

            if (hoveredObject is HanakoDestination_Toilet)
            {
                var hoveredToilet = hoveredObject as HanakoDestination_Toilet;
                if (possessedToilet == hoveredToilet)
                    AttackFromPossessedToilet();

                else if (hoveredToilet.Occupation == OccupationMode.Unoccupied && possessCooldown < 0f)
                    PossessToilet(hoveredToilet);
            }
            else if (hoveredObject is HanakoDistraction)
            {
                var hoveredDistraction = hoveredObject as HanakoDistraction;
                hoveredDistraction.Distract();
            }

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
        }

        void Hover(IHanakoInteractableByCursor interactableObject)
        {
            if (hoveredObject != null && hoveredObject != interactableObject)
                Unhover(hoveredObject);

            hoveredObject = interactableObject;
            hoveredObject.Hover();
        }

        void Unhover(IHanakoInteractableByCursor interactableObject)
        {
            hoveredObject = null;
            interactableObject.Unhover();
        }
    }
}
