using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName ="KnifeValidator_IsOccupied", menuName ="SO/Knife/Move Validator/Is Occupied & Interactable")]

    public class KnifeMoveValidator_IsOccupiedAndInteractable : KnifeMoveValidator
    {
        public override bool Validate(ColRow colRow, PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            foreach (var piece in allPieces)
            {
                if (piece.ColRow.IsEqual(colRow))
                {
                    if (piece.Piece.IsInteractable)
                    {
                        return !reverseResult;
                    }
                    else
                    {
                        return !reverseResult;
                    }
                }
            }

            return true;
        }
    }
}
