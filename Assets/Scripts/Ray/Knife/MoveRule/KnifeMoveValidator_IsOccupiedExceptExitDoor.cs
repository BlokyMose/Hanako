using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeValidator_IsOccupiedExceptExitDoor", menuName = "SO/Knife/Move Validator/Is Occupied Except Exit Door")]

    public class KnifeMoveValidator_IsOccupiedExceptExitDoor : KnifeMoveValidator
    {
        public override bool Validate(ColRow colRow, PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var foundPiece = GetPieceOf(colRow, allPieces);
            if (foundPiece != null)
            {
                if (foundPiece.Piece is KnifePiece_ExitDoor)
                {
                    return reverseResult;
                }
                else
                {
                    return !reverseResult;
                }
            }

            return true;
        }
    }
}
