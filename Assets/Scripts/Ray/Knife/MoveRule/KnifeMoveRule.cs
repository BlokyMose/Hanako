using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{

    public abstract class KnifeMoveRule : ScriptableObject
    {
        [SerializeField]
        string ruleName;

        public abstract List<ColRow> GetValidMoves(PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties);

        public bool IsInsideMap(ColRow colRow, ColRow levelSize)
        {
            return  colRow.col >= 0 &&
                    colRow.col < levelSize.col &&
                    colRow.row >= 0 &&
                    colRow.row < levelSize.row;
        }

        public bool IsOccupied(ColRow colRow, List<PieceCache> allPieces, out PieceCache occupantPiece)
        {
            foreach (var piece in allPieces)
            {
                if (colRow.IsEqual(piece.ColRow))
                {
                    occupantPiece = piece;
                    return true;
                }
            }

            occupantPiece = null;
            return false;
        }
    }
}
