using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public abstract class KnifeAbility : ScriptableObject
    {
        public abstract void Interacted(KnifeLevelManager.LivingPieceCache interactorPiece, KnifeLevelManager.TileCache interactedTile, KnifeLevelManager levelManager);
        public virtual void Preview(KnifeLevelManager.LivingPieceCache interactorPiece, KnifeLevelManager.TileCache interactedTile, KnifeLevelManager levelManager)
        {

        }
    }
}
