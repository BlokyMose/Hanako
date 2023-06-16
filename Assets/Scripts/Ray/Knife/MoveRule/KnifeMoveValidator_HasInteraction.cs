using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeValidator_HasInteraction", menuName = "SO/Knife/Move Validator/Has Interaction")]

    public class KnifeMoveValidator_HasInteraction : KnifeMoveValidator
    {
        [SerializeField]
        KnifeInteraction targetInteraction;

        public override bool Validate(ColRow colRow, PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var foundPiece = GetPieceOf(colRow, allPieces);
            if (foundPiece != null && foundPiece.Piece.HasInteraction(targetInteraction))
                return !reverseResult;

            return true;
        }
    }
}
