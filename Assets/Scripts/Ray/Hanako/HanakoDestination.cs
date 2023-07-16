using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoDestination : MonoBehaviour
    {
        [SerializeField]
        private HanakoDestinationID id;

        [SerializeField]
        protected float interactDuration = 1f;

        [Header("UI")]
        [SerializeField]
        protected GameObject destinationUIPrefab;
        
        [SerializeField]
        protected Transform destinationUIParent;

        [Header("Components")]
        [SerializeField]
        protected List<SpriteRenderer> srs = new();

        [SerializeField]
        protected HanakoLevelManager levelManager;

        [SerializeField]
        protected Transform position;

        protected HanakoDestinationUI destinationUI;
        protected bool isOccupied = false;
        protected float durationLeft;
        public event Action<float> OnDurationDepleted;
        public event Action OnDurationEnd;

        public HanakoDestinationID Id { get => id; }
        public Transform Position => position == null ? transform : position;

        public bool IsOccupied { get => isOccupied; }

        protected virtual void Awake()
        {
            destinationUI = Instantiate(destinationUIPrefab, destinationUIParent).GetComponent<HanakoDestinationUI>();
            destinationUI.transform.localPosition = Vector3.zero;
            destinationUI.Init(ref OnDurationDepleted, ref OnDurationEnd);

            if (srs.Count == 0)
            {
                srs.AddRange(gameObject.GetComponentsInFamily<SpriteRenderer>());
            }
        }

        protected virtual void OnDestroy()
        {
            destinationUI.Exit(ref OnDurationDepleted, ref OnDurationEnd);
        }

        public void Init(HanakoLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

        public virtual IEnumerator Interact(HanakoEnemy enemy)
        {
            isOccupied = true;
            durationLeft = interactDuration;
            WhenInteractStart(enemy);
            while (durationLeft > 0f)
            {
                durationLeft -= Time.deltaTime;
                OnDurationDepleted?.Invoke(durationLeft / interactDuration);
                yield return null;
            }
            OnDurationEnd?.Invoke();
            isOccupied = false;
        }

        protected virtual void WhenInteractStart(HanakoEnemy enemy)
        {

        }
    }
}
