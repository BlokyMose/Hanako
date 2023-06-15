using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_ExitDoor : KnifePiece_NonLiving
    {
        [SerializeField]
        Transform doorPosition;

        public override void Interacted(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                levelManager.MoveLivingPieceToEscapeList(otherPiece.LivingPiece);
                otherPiece.LivingPiece.MoveToTile(myTile.Tile);
                yield return new WaitForSeconds(otherPiece.LivingPiece.MoveDuration);
                otherPiece.LivingPiece.Escape(doorPosition.position);
                yield return new WaitForSeconds(otherPiece.LivingPiece.MoveDuration);
                otherPiece.LivingPiece.SetActState(KnifePiece_Living.PieceActingState.PostActing);
            }
        }
    }
}
