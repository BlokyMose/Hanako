using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_Student : KnifePiece_Enemy
    {
        public override bool CheckValidityAgainst(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        {
            return CheckInteractabilityAgainst(otherPiece, myTile);
        }

        public override bool CheckInteractabilityAgainst(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        {
            if (otherPiece.LivingPiece is KnifePiece_Enemy)
            {
                return false;
            }
            if (otherPiece.LivingPiece is KnifePiece_Player)
            {
                return true;
            }

            return true;

        }

        public override void Interacted(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        {
            otherPiece.LivingPiece.MoveToTile(myTile.Tile);
            if (otherPiece.LivingPiece is KnifePiece_Player)
            {
                (otherPiece.LivingPiece as KnifePiece_Player).Attack();
            }
            Die(otherPiece);
        }
    }
}
