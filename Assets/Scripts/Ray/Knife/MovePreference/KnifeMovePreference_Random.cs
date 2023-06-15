using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMovePref_Random", menuName = "SO/Knife/Move Preference/Random")]

    public class KnifeMovePreference_Random : KnifeMovePreference
    {

        public override void Evaluate(List<KnifePiece_Enemy.PrefferedTile> tiles, int influence, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties)
        {
            foreach (var tile in tiles)
                tile.AddInfluence(influence);
        }

    }
}
