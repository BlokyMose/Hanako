using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeInteraction_Murder", menuName = "SO/Knife/Interaction/Murder")]

    public class KnifeInteraction_Murder : KnifeInteraction
    {
        public override void Interact(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager)
        {
            var otherLivingPiece = levelManager.GetLivingPiece(otherPiece.Piece);
            var myLivingPiece = levelManager.GetLivingPiece(myPiece.Piece);

            if (otherLivingPiece != null && myLivingPiece != null)
            {
                otherLivingPiece.LivingPiece.MoveToTile(myTile.Tile);
                myLivingPiece.Piece.StartCoroutine(WaitToBeAttackThenDie());
                IEnumerator WaitToBeAttackThenDie()
                {
                    yield return new WaitForSeconds(otherLivingPiece.LivingPiece.MoveDuration * 0.25f);
                    otherLivingPiece.LivingPiece.Attack();
                    myLivingPiece.Die();
                    myLivingPiece.LivingPiece.Die(otherLivingPiece);
                }
            }
        }
    }
}
