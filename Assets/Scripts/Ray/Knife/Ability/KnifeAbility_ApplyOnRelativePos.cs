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

        [SerializeField]
        ActionIconPack iconPackOnRelativePos;

        [SerializeField]
        GameObject GOWhenInteractedOnRelativePos;

        [SerializeField]
        float destroyAfter = 1f;

        public override void Interacted(LivingPieceCache interactorPiece, TileCache interactedTile, KnifeLevelManager levelManager)
        {
            foreach (var pos in relativePos)
            {
                if (levelManager.TryGetTile(ColRow.AddBetween(interactedTile.ColRow, pos), out var foundTile))
                {
                    if (foundTile.Tile.TryGetPiece(out var tilePiece) && 
                        tilePiece != interactorPiece.Piece)
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

                    if (GOWhenInteractedOnRelativePos != null)
                    {
                        if (!(tilePiece != null && tilePiece is KnifePiece_NonLiving))
                        {
                            var go = Instantiate(GOWhenInteractedOnRelativePos);
                            go.transform.position = foundTile.GO.transform.position;
                            Destroy(go, destroyAfter);
                        }
                    }
                }
            }
        }

        public override void Preview(LivingPieceCache interactorPiece, TileCache interactedTile, KnifeLevelManager levelManager)
        {
            var iconPack = new ActionIconPack(iconPackOnRelativePos.Icon, true, levelManager.Colors.TileActionColor, iconPackOnRelativePos.Animation);
            foreach (var pos in relativePos)
            {
                if (levelManager.TryGetTile(ColRow.AddBetween(interactedTile.ColRow, pos), out var foundTile))
                {
                    if (!foundTile.Tile.TryGetPiece(out var tilePiece) || 
                        tilePiece is KnifePiece_Living)
                        foundTile.Tile.ShowInteractionIcon(iconPack);
                }
            }
        }
    }
}
