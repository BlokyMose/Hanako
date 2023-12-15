using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMovePref_Random", menuName = "SO/Knife/Move Preference/Random")]

    public class KnifeMovePreference_Random : KnifeMovePreference
    {

        public override void Evaluate(
            List<KnifePiece_Enemy.PrefferedTile> prefferedTiles,
            List<TileCache> validTiles,
            int influence,
            PieceCache thisPiece,
            KnifeLevelManager levelManager)
        {
            foreach (var tile in prefferedTiles)
                tile.AddInfluence(influence);
        }

    }
}
