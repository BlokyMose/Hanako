using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMovePref_ToPlayer", menuName = "SO/Knife/Move Preference/To Player")]

    public class KnifeMovePreference_ToPlayer : KnifeMovePreference
    {
        [SerializeField]
        int maximumDistanceToPlayer = 1;

        public override void Evaluate(List<KnifePiece_Enemy.PrefferedTile> tiles, int influence, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var playerColRow = new ColRow(0, 0);
            foreach (var piece in allPieces)
            {
                if (piece.Piece is KnifePiece_Player)
                {
                    playerColRow = piece.ColRow;
                    foreach (var tile in tiles)
                    {
                        if (ColRow.DistanceBetween(tile.Tile.ColRow, playerColRow) <= maximumDistanceToPlayer)
                            tile.AddInfluence(influence);
                    }
                    break;
                }
            }
        }

    }
}
