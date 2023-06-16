using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifePiece_Living;

namespace Hanako.Knife
{
    public class KnifePiece_Item : KnifePiece_NonLiving
    {
        //[SerializeField]
        //KnifeAbility item;

        //public override void Interacted(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        //{
        //    item.Interacted(otherPiece, myTile);
        //    otherPiece.LivingPiece.MoveToTile(myTile.Tile, false);

        //    StartCoroutine(Delay(otherPiece.LivingPiece.MoveDuration));
        //    IEnumerator Delay(float delay)
        //    {
        //        yield return new WaitForSeconds(delay);
        //        levelManager.RemovePiece(this);
        //        otherPiece.LivingPiece.SetActState(PieceActingState.PostActing);
        //        Destroy(gameObject);
        //    }
        //}
    }
}
