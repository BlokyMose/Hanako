using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public abstract class KnifeAbility : ScriptableObject
    {
        public abstract void Interacted(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile);
    }
}
