using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;
using static Hanako.Knife.KnifePiece_Living;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeInteraction_Move", menuName = "SO/Knife/Interaction/Move")]

    public class KnifeInteraction_Move : KnifeInteraction
    {
        [SerializeField]
        float distanceToStartMoveAnimation = 1f;

        [SerializeField]
        int maximumMove = 1;

        public override void Interact(PieceCache interactedPiece, TileCache interactedTile, PieceCache interactorPiece, TileCache interactorTile, KnifeLevelManager levelManager)
        {
            if (interactorPiece is LivingPieceCache)
            {
                var otherLivingPiece = interactorPiece as LivingPieceCache;

                otherLivingPiece.LivingPiece.MoveToTile(interactedTile.Tile, autoSetAct: false);

                otherLivingPiece.LivingPiece.StartCoroutine(MovingMyselfWhenOtherPieceIsClose());
                IEnumerator MovingMyselfWhenOtherPieceIsClose()
                {
                    while (true)
                    {
                        if (Vector2.Distance(otherLivingPiece.LivingPiece.transform.position, interactedPiece.Piece.transform.position) < distanceToStartMoveAnimation)
                            break;
                        yield return null;
                    }

                    var moveColRow = ColRow.SubstractBetween(interactedTile.ColRow, interactorPiece.ColRow);
                    moveColRow.row = moveColRow.row > maximumMove ? maximumMove : moveColRow.row;
                    moveColRow.col = moveColRow.col > maximumMove ? maximumMove : moveColRow.col;
                    var targetColRow = ColRow.AddBetween(interactedTile.ColRow, moveColRow);
                    var targetTile = levelManager.GetTile(targetColRow);
                    MoveToTile(interactedPiece, targetTile.Tile, otherLivingPiece.LivingPiece.MoveDuration);

                    yield return new WaitForSeconds(otherLivingPiece.LivingPiece.MoveDuration);
                    otherLivingPiece.LivingPiece.SetActState(PieceActingState.PostActing);
                }
            }
        }

        public virtual void MoveToTile(PieceCache myPiece, KnifeTile destinationTile, float moveDuration)
        {
            myPiece.Piece.StartCoroutine(Moving());
            myPiece.Piece.StartCoroutine(SettingParent(moveDuration));

            IEnumerator SettingParent(float delay)
            {
                if (destinationTile.PieceParent.transform.position.y < myPiece.Piece.transform.position.y)
                {
                    destinationTile.SetAsParentOf(myPiece.Piece.gameObject);
                    yield break;
                }

                else if (destinationTile.PieceParent.transform.position.y > myPiece.Piece.transform.position.y)
                {
                    yield return new WaitForSeconds(delay);
                    destinationTile.SetAsParentOf(myPiece.Piece.gameObject);
                }
            }

            IEnumerator Moving()
            {
                var animator = myPiece.Piece.GetComponentInChildren<Animator>();
                int int_motion;
                int_motion = Animator.StringToHash(nameof(int_motion));
                if (animator != null) 
                    animator.SetInteger(int_motion, (int)CharacterMotion.Run);

                yield return new WaitForSeconds(0.1f);

                if (destinationTile.transform.position.x > myPiece.Piece.transform.position.x)
                {
                    myPiece.Piece.transform.localEulerAngles = new Vector3(myPiece.Piece.transform.localEulerAngles.x, 0f, myPiece.Piece.transform.localEulerAngles.z);
                }
                else if (destinationTile.transform.position.x < myPiece.Piece.transform.position.x)
                {
                    myPiece.Piece.transform.localEulerAngles = new Vector3(myPiece.Piece.transform.localEulerAngles.x, 180f, myPiece.Piece.transform.localEulerAngles.z);
                }

                var time = 0f;
                var originPos = myPiece.Piece.transform.position;
                var curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                while (true)
                {
                    var tScale = time / moveDuration;
                    tScale = curve.Evaluate(tScale);
                    myPiece.Piece.transform.position = Vector2.Lerp(originPos, destinationTile.PieceParent.position, tScale);

                    if (time > moveDuration - 0.1f)
                        if (animator != null) 
                            animator.SetInteger(int_motion, (int)CharacterMotion.Idle);

                    if (time >= moveDuration)
                    {
                        break;
                    }

                    time += Time.deltaTime;
                    yield return null;
                }

                myPiece.Piece.transform.localPosition = Vector2.zero;
                destinationTile = null;
            }
        }

        public override bool CheckInteractabilityAgainst(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager)
        {
            var moveColRow = ColRow.SubstractBetween(myPiece.ColRow, otherPiece.ColRow);
            moveColRow.row = moveColRow.row > maximumMove ? maximumMove : moveColRow.row;
            moveColRow.col = moveColRow.col > maximumMove ? maximumMove : moveColRow.col;

            var targetColRow = ColRow.AddBetween(myPiece.ColRow, moveColRow);

            if (levelManager.TryGetTile(targetColRow, out var foundTile))
            {
                if (foundTile.Tile.TryGetPiece(out var occupantPiece))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public override void ShowPreview(PieceCache interactedPiece, TileCache interactedTile, PieceCache interactorPiece, TileCache interactorTile, KnifeLevelManager levelManager)
        {
            var moveColRow = ColRow.SubstractBetween(interactedPiece.ColRow, interactorPiece.ColRow);
            moveColRow.row = moveColRow.row > maximumMove ? maximumMove : moveColRow.row;
            moveColRow.col = moveColRow.col > maximumMove ? maximumMove : moveColRow.col;

            var targetColRow = ColRow.AddBetween(interactedPiece.ColRow, moveColRow);
            if (levelManager.TryGetTile(targetColRow, out var foundTile))
            {
                if (foundTile.Tile.TryGetPiece(out var occupantPiece))
                    return;
                foundTile.Tile.ShowPieceHologram(interactedPiece.Piece.GOToPreview, interactedPiece.Piece.GOToPreviewOffset);
            }
           
        }
    }
}
