using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeValidator_IsExitDoor", menuName = "SO/Knife/Move Validator/Is Exit Door")]

    public class KnifeMoveValidator_IsExitDoor : KnifeMoveValidator
    {
        public override bool Validate(KnifeLevel.ColRow colRow, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var foundPiece = GetPieceOf(colRow, allPieces);
            if (foundPiece != null && foundPiece.Piece is KnifePiece_ExitDoor)
            {
                return !reverseResult;
            }
            return true;
        }
    }
}
