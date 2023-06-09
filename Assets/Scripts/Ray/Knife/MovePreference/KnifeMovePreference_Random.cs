using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMovePref_Random", menuName = "SO/Knife/Move Preference/Random")]

    public class KnifeMovePreference_Random : KnifeMovePreference
    {
        public override KnifeLevel.ColRow GetPrefferedMove(List<KnifeLevel.ColRow> validMoves, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties)
        {
            return validMoves.GetRandomStruct();
        }
    }
}
