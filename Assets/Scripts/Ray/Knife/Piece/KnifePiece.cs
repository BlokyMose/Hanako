using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifePiece : MonoBehaviour
    {
        [Serializable]
        public class InteractionProperties
        {
            [SerializeField]
            GameObject componentType;

            [SerializeField]
            bool isInteractable = true;

            [SerializeField]
            List<KnifeInteraction> interactions = new();

            public GameObject ComponentType { get => componentType; }
            public bool IsInteractable { get => isInteractable; }
            public List<KnifeInteraction> Interactions { get => interactions; }
        }

        [SerializeField]
        KnifePieceInformation information;

        [SerializeField]
        protected bool isInteractableDefault = true;

        [SerializeField]
        protected List<InteractionProperties> interactionProperties = new();
        public List<InteractionProperties> Interactions => interactionProperties;

        [Header("UI")]
        [SerializeField, LabelText("Head Pos")]
        Transform headPosForLogo;

        [SerializeField, LabelText("Offset")]
        Vector3 headPosOffset = new(0, 0.25f, -3f);

        protected KnifeLevelManager levelManager;

        protected bool isInteractable = true;
        public bool IsInteractable { get => isInteractable; }
        public KnifePieceInformation Information { get => information; }
        public Transform HeadPosForLogo { get => headPosForLogo != null ? headPosForLogo : transform; }
        public Vector3 HeadPosOffset { get => headPosOffset; }

        public bool HasInteraction(KnifeInteraction targetInteraction)
        {
            foreach (var interaction in interactionProperties)
            {
                if (interaction.Interactions.Contains(targetInteraction))
                    return true;
            }
            return false;
        }

        protected virtual void Awake()
        {
            if (interactionProperties.Count == 0)
                Debug.LogWarning($"{gameObject.name} has no interaction properties; The game will be stucked because SetActingState is not called via Interacted()");
        }

        public virtual void Init(KnifeLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

        public virtual bool CheckValidityAgainst(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile)
        {
            return CheckInteractabilityAgainst(myPiece, myTile, otherPiece, otherTile);
        }

        public virtual bool CheckInteractabilityAgainst(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile)
        {
            if (isInteractable)
            {
                foreach (var ip in interactionProperties)
                {
                    if (ip.ComponentType == null ||
                        (ip.ComponentType != null && ip.ComponentType.TryGetComponent(otherPiece.Piece.GetType(), out var piece)))
                    {
                        if (ip.IsInteractable)
                        {
                            foreach (var interaction in ip.Interactions)
                            {
                                if (!interaction.CheckInteractabilityAgainst(myPiece, myTile, otherPiece, otherTile, levelManager))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return isInteractableDefault;
            }
            else
            {
                return false;
            }
        }

        public virtual void Interacted(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile)
        {
            foreach (var ip in interactionProperties)
            {
                if (ip.ComponentType == null ||
                    (ip.ComponentType != null && ip.ComponentType.TryGetComponent(otherPiece.Piece.GetType(), out var piece)))
                {
                    foreach (var _interaction in ip.Interactions)
                    {
                        _interaction.Interact(myPiece, myTile, otherPiece, otherTile, levelManager);
                    }
                    break;
                }
            }
        }
    }
}
