using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName ="KnifeTiles_Default_", menuName ="SO/Knife/Tiles Pattern/Default")]

    public class KnifeTilesPattern_Default : KnifeTilesPattern
    {
        [SerializeField]
        GameObject tile;

        public override GameObject GetTile(ColRow colRow, KnifeLevel levelProperties)
        {
            return tile;
        }
    }
}
