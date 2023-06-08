using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeTiles_ByIndex_", menuName = "SO/Knife/Tiles Pattern/ByIndex")]

    public class KnifeTilesPattern_ByIndex : KnifeTilesPattern
    {
        [SerializeField]
        List<GameObject> tiles = new();

        public override GameObject GetTile(KnifeLevel.ColRow colRow, KnifeLevel levelProperties)
        {
            return tiles[(colRow.col + (levelProperties.LevelSize.col * colRow.row)) % tiles.Count];
        }
    }
}