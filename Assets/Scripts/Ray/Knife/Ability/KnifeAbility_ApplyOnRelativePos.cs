using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_ApplyOnRelativePos", menuName = "SO/Knife/Ability/Apply On Relative Pos")]


    public class KnifeAbility_ApplyOnRelativePos : KnifeAbility
    {
        [SerializeField]
        List<ColRow> relativePos = new();

        [SerializeField]
        List<KnifeAbility> abilities = new();

        public override void Interacted(LivingPieceCache otherPiece, TileCache myTile, KnifeLevelManager levelManager)
        {
            foreach (var pos in relativePos)
            {
                if (levelManager.TryGetTile(ColRow.AddBetween(myTile.ColRow, pos), out var foundTile))
                {
                    if (foundTile.Tile.TryGetPiece(out var tilePiece) && tilePiece != otherPiece.Piece)
                    {
                        if (tilePiece is KnifePiece_Living)
                        {
                            var tileLivingPiece = tilePiece as KnifePiece_Living;
                            foreach (var ability in abilities)
                            {
                                ability.Interacted(levelManager.GetLivingPiece(tileLivingPiece), foundTile, levelManager);
                            }
                        }
                    }
                }
            }
        }

    }
}
