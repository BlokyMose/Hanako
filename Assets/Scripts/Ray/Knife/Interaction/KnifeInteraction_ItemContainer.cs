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

        public override void Interact(PieceCache interactedPiece, TileCache interactedTile, PieceCache interactorPiece, TileCache interactorTile, KnifeLevelManager levelManager)
        {
            if (interactorPiece is LivingPieceCache)
            {
                var otherLivingPiece = interactorPiece as LivingPieceCache;

                foreach (var ability in abilities)
                    ability.Interacted(otherLivingPiece, interactedTile, levelManager);
                otherLivingPiece.LivingPiece.MoveToTile(interactedTile.Tile, false);

                otherLivingPiece.Piece.StartCoroutine(Delay(otherLivingPiece.LivingPiece.MoveDuration));
                IEnumerator Delay(float delay)
                {
                    yield return new WaitForSeconds(delay);
                    levelManager.RemovePiece(interactedPiece.Piece);
                    otherLivingPiece.LivingPiece.SetActState(PieceActingState.PostActing);
                    Destroy(interactedPiece.GO);
                }
            }
        }

        public override void ShowPreview(PieceCache interactedPiece, TileCache interactedTile, PieceCache interactorPiece, TileCache interactorTile, KnifeLevelManager levelManager)
        {
            foreach (var ability in abilities)
            {
                ability.Preview(interactorPiece as LivingPieceCache, interactedTile, levelManager);
            }
        }
    }
}
