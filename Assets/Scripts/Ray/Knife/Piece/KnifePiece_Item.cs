using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_Item : KnifePiece_NonLiving
    {
        [SerializeField]
        KnifeAbility item;

        public override void Interacted(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        {
            item.Interacted(otherPiece, myTile);
            base.Interacted(otherPiece, myTile);
        }
    }
}
