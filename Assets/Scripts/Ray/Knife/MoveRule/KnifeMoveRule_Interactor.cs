using Hanako.Knife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako
{

    [CreateAssetMenu(fileName = "KnifeMoveRule_Interactor", menuName = "SO/Knife/Move Rule/Interactor")]
    public class KnifeMoveRule_Interactor : KnifeMoveRule
    {
        public override List<ColRow> GetValidMoves(PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var goUp = new ColRow(thisPiece.ColRow.col, thisPiece.ColRow.row + 1);
            var goDown = new ColRow(thisPiece.ColRow.col, thisPiece.ColRow.row - 1);
            var goRight = new ColRow(thisPiece.ColRow.col + 1, thisPiece.ColRow.row);
            var goLeft = new ColRow(thisPiece.ColRow.col - 1, thisPiece.ColRow.row);
            var validMoves = new List<ColRow>() { goUp, goDown, goRight, goLeft };

            for (int i = validMoves.Count - 1; i >= 0; i--)
            {
                if (!IsInsideMap(validMoves[i], levelProperties.LevelSize) ||
                    (IsOccupied(validMoves[i], allPieces, out var occupantPiece) &&
                     !occupantPiece.Piece.IsInteractable
                    ))
                {
                    validMoves.RemoveAt(i);
                }
            }

            return validMoves;
        }
    }
}
