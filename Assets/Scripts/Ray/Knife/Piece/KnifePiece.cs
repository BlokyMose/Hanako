using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifePiece : MonoBehaviour
    {
        protected KnifeLevelManager levelManager;
        protected bool isInteractable = true;
        public bool IsInteractable { get => isInteractable; }

        public virtual void Init(KnifeLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

        public virtual bool CheckInteractabilityAgainst(LivingPieceCache otherPiece, TileCache myTile)
        {
            return isInteractable;
        }

        public virtual bool CheckValidityAgainst(LivingPieceCache otherPiece, TileCache myTile)
        {
            return isInteractable;
        }

        public virtual void Interacted(LivingPieceCache otherPiece, TileCache myTile)
        {
            otherPiece.LivingPiece.MoveToTile(myTile.Tile);
        }
    }
}
