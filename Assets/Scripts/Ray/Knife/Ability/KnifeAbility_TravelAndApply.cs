using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_TravelAndApply", menuName = "SO/Knife/Ability/Travel And Apply")]

    public class KnifeAbility_TravelAndApply : KnifeAbility
    {
        [SerializeField]
        List<ColRow> directions = new();

        [SerializeField]
        List<KnifeAbility> abilities = new();

        public override void Interacted(LivingPieceCache interactorPiece, TileCache interactedTile, KnifeLevelManager levelManager)
        {
            foreach (var direction in directions)
            {
                var _direction = direction;
                while (true)
                {
                    if (levelManager.TryGetTile(ColRow.AddBetween(interactedTile.ColRow, _direction), out var foundTile))
                    {
                        if (foundTile.Tile.TryGetPiece(out var tilePiece) && tilePiece != interactorPiece.Piece)
                        {
                            if (tilePiece is KnifePiece_Living)
                            {
                                var tileLivingPiece = tilePiece as KnifePiece_Living;
                                foreach (var ability in abilities)
                                {
                                    ability.Interacted(levelManager.GetLivingPiece(tileLivingPiece), foundTile, levelManager);
                                }
                            }
                            break;
                        }

                        _direction.Add(direction);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
