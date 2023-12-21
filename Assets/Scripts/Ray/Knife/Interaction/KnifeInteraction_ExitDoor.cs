using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeInteraction_ExitDoor", menuName = "SO/Knife/Interaction/ExitDoor")]

    public class KnifeInteraction_ExitDoor : KnifeInteraction
    {
        [SerializeField]
        Vector2 doorPosition;

        [SerializeField]
        string animatorModeParam = "int_mode";

        public override void Interact(PieceCache interactedPiece, TileCache interactedTile, PieceCache interactorPiece, TileCache interactorTile, KnifeLevelManager levelManager)
        {
            if (interactorPiece is LivingPieceCache)
            {
                var interactorLivingPiece = interactorPiece as LivingPieceCache;

                interactedPiece.Piece.StartCoroutine(Delay());
                IEnumerator Delay()
                {
                    levelManager.MoveLivingPieceToEscapeList(interactorLivingPiece.LivingPiece);
                    interactorLivingPiece.LivingPiece.MoveToTile(interactedTile.Tile);
                    if(interactedPiece.Piece.TryGetComponentInChildren<Animator>(out var animator))
                        animator.SetInteger(animatorModeParam, 0);
                    yield return new WaitForSeconds(interactorLivingPiece.LivingPiece.MoveDuration);
                    interactorLivingPiece.LivingPiece.Escape((Vector2)interactedPiece.Piece.transform.position * interactedPiece.Piece.transform.localScale + doorPosition);
                    yield return new WaitForSeconds(interactorLivingPiece.LivingPiece.MoveDuration);
                    if (animator != null)
                        animator.SetInteger(animatorModeParam, 1);
                    interactorLivingPiece.LivingPiece.SetActState(KnifePiece_Living.PieceActingState.PostActing);
                }
            }
        }
    }
}
