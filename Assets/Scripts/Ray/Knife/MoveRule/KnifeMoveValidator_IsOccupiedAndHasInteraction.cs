using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeValidator_IsOccupiedAndHasInteraction", menuName = "SO/Knife/Move Validator/Is Occupied and Has Interaction")]

    public class KnifeMoveValidator_IsOccupiedAndHasInteraction : KnifeMoveValidator
    {
        [SerializeField]
        KnifeInteraction targetInteraction;

        public override bool Validate(KnifeLevel.ColRow colRow, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var foundPiece = GetPieceOf(colRow, allPieces);
            if (foundPiece != null && foundPiece.Piece.HasInteraction(targetInteraction))
            {
                return !reverseResult;
            }
            return true;
        }

    }
}
