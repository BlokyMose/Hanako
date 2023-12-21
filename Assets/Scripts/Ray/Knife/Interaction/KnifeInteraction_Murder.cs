using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeInteraction_Murder", menuName = "SO/Knife/Interaction/Murder")]

    public class KnifeInteraction_Murder : KnifeInteraction
    {
        public override void Interact(PieceCache interactedPiece, TileCache interactedTile, PieceCache interactorPiece, TileCache interactorTile, KnifeLevelManager levelManager)
        {
            var interactorLivingPiece = levelManager.GetLivingPiece(interactorPiece.Piece);
            var interactedLivingPiece = levelManager.GetLivingPiece(interactedPiece.Piece);

            if (interactorLivingPiece != null && interactedLivingPiece != null)
            {
                interactorLivingPiece.LivingPiece.MoveToTile(interactedTile.Tile);
                interactedLivingPiece.Piece.StartCoroutine(WaitToBeAttackThenDie());
                IEnumerator WaitToBeAttackThenDie()
                {
                    yield return new WaitForSeconds(interactorLivingPiece.LivingPiece.MoveDuration * 0.25f);
                    
                    // TODO: stream line die to only one code
                    interactorLivingPiece.LivingPiece.Attack();
                    interactedLivingPiece.Die();
                    interactedLivingPiece.LivingPiece.Die(interactorLivingPiece);
                }
            }
        }
    }
}
