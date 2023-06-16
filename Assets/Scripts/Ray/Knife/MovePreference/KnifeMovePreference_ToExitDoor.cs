using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMovePref_ToExitDoor", menuName = "SO/Knife/Move Preference/To Exit Door")]

    public class KnifeMovePreference_ToExitDoor : KnifeMovePreference
    {
        [SerializeField]
        KnifeInteraction targetInteraction;

        public override void Evaluate(List<KnifePiece_Enemy.PrefferedTile> tiles, int influence, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties)
        {
            var doorColRow = new ColRow(0, 0);
            foreach (var piece in allPieces)
            {
                if (piece.Piece.HasInteraction(targetInteraction))
                {
                    doorColRow = piece.ColRow;
                    var closestTile = tiles[0];
                    var closestDistance = ColRow.DistanceBetween(closestTile.Tile.ColRow, doorColRow);
                    for (int i = 1; i < tiles.Count; i++)
                    {
                        var distance = ColRow.DistanceBetween(tiles[i].Tile.ColRow, doorColRow);
                        if (distance < closestDistance)
                        {
                            closestTile = tiles[i];
                            closestDistance = distance;
                        }
                    }

                    foreach (var tile in tiles)
                    {
                        if (ColRow.DistanceBetween(tile.Tile.ColRow, doorColRow) == closestDistance)
                            tile.AddInfluence(influence);
                    }
                }
            }
        }

    }
}
