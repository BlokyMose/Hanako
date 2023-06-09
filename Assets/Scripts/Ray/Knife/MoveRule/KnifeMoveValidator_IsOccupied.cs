using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeValidator_IsOccupied", menuName = "SO/Knife/Move Validator/Is Occupied")]

    public class KnifeMoveValidator_IsOccupied : KnifeMoveValidator
    {
        public override bool Validate(ColRow colRow, PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var foundPiece = GetPieceOf(colRow, allPieces);
            if (foundPiece != null)
            {
                return !reverseResult;
            }

            return true;
        }
    }
}
