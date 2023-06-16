using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeValidator_IsOccupiedAndWithoutInteraction", menuName = "SO/Knife/Move Validator/Is Occupied And Without Interaction")]

    public class KnifeMoveValidator_IsOccupiedAndWithoutInteraction : KnifeMoveValidator
    {
        [SerializeField]
        KnifeInteraction targetInteraction;

        public override bool Validate(ColRow colRow, PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var foundPiece = GetPieceOf(colRow, allPieces);
            if (foundPiece != null)
            {
                if (foundPiece.Piece.HasInteraction(targetInteraction))
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
