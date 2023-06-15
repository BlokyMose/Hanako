using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMoveRule_Base", menuName = "SO/Knife/Move Rule/Base")]
    public class KnifeMoveRule_Base : KnifeMoveRule
    {
        [SerializeField, Tooltip("Counted from the current colRow of the piece")]
        List<ColRow> validMoves = new();

        [SerializeField]
        List<KnifeMoveValidator> validators = new();

        public override List<ColRow> GetValidMoves(PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var validMoves = new List<ColRow>();
            foreach (var colRow in this.validMoves)
                validMoves.Add(ColRow.AddBetween(thisPiece.ColRow, colRow));

            for (int i = validMoves.Count - 1; i >= 0; i--)
                if (!IsInsideMap(validMoves[i], levelProperties.LevelSize))
                    validMoves.RemoveAt(i);

            foreach (var validator in validators)
                for (int i = validMoves.Count - 1; i >= 0; i--)
                    if (!validator.Validate(validMoves[i], thisPiece, allPieces, levelProperties))
                        validMoves.RemoveAt(i);

            return validMoves;
        }
    }
}
