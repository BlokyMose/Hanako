using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;
using static Hanako.Knife.KnifePiece_Living;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeInteraction_ItemContainer", menuName = "SO/Knife/Interaction/Item Container")]

    public class KnifeInteraction_ItemContainer : KnifeInteraction
    {
        [SerializeField]
        List<KnifeAbility> abilities = new();

        public override void Interact(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager)
        {
            if (otherPiece is LivingPieceCache)
            {
                var otherLivingPiece = otherPiece as LivingPieceCache;

                foreach (var ability in abilities)
                    ability.Interacted(otherLivingPiece, myTile);
                otherLivingPiece.LivingPiece.MoveToTile(myTile.Tile, false);

                otherLivingPiece.Piece.StartCoroutine(Delay(otherLivingPiece.LivingPiece.MoveDuration));
                IEnumerator Delay(float delay)
                {
                    yield return new WaitForSeconds(delay);
                    levelManager.RemovePiece(myPiece.Piece);
                    otherLivingPiece.LivingPiece.SetActState(PieceActingState.PostActing);
                    Destroy(myPiece.GO);
                }
            }
        }
    }
}
