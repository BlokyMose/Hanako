using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifeInteraction : ScriptableObject
    {
        [SerializeField]
        string interactionName;

        public abstract void Interact(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager);

        public virtual bool CheckInteractabilityAgainst(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager)
        {
            return true;
        }

    }
}
