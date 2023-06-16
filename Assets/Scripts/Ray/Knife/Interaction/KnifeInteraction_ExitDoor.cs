using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeInteraction_ExitDoor", menuName = "SO/Knife/Interaction/ExitDoor")]

    public class KnifeInteraction_ExitDoor : KnifeInteraction
    {
        [SerializeField]
        Vector2 doorPosition;

        public override void Interact(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager)
        {
            if (otherPiece is LivingPieceCache)
            {
                var otherLivingPiece = otherPiece as LivingPieceCache;

                myPiece.Piece.StartCoroutine(Delay());
                IEnumerator Delay()
                {
                    levelManager.MoveLivingPieceToEscapeList(otherLivingPiece.LivingPiece);
                    otherLivingPiece.LivingPiece.MoveToTile(myTile.Tile);
                    yield return new WaitForSeconds(otherLivingPiece.LivingPiece.MoveDuration);
                    otherLivingPiece.LivingPiece.Escape((Vector2)myPiece.Piece.transform.position * myPiece.Piece.transform.localScale + doorPosition);
                    yield return new WaitForSeconds(otherLivingPiece.LivingPiece.MoveDuration);
                    otherLivingPiece.LivingPiece.SetActState(KnifePiece_Living.PieceActingState.PostActing);
                }
            }
        }
    }
}
