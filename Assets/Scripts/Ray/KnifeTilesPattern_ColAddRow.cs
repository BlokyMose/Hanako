using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeTiles_ColAddRow_", menuName ="SO/Knife/Tiles Pattern/ColAddRow")]
    public class KnifeTilesPattern_ColAddRow : KnifeTilesPattern
    {
        [SerializeField]
        List<GameObject> tiles = new();

        public override GameObject GetTile(KnifeLevel.ColRow colRow, KnifeLevel levelProperties)
        {
            return tiles[(colRow.col + colRow.row) % tiles.Count];
        }
    }
}