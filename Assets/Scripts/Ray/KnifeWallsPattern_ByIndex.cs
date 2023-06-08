using Hanako.Knife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="KnifeWalls_ByIndex_", menuName = "SO/Knife/Walls Pattern/ByIndex")]

    public class KnifeWallsPattern_ByIndex : KnifeWallsPattern
    {
        [SerializeField]
        List<GameObject> walls = new();

        public override GameObject GetLeftWall(int index, KnifeLevel levelProperties)
        {
            return walls[index%walls.Count];
        }

        public override GameObject GetRightWall(int index, KnifeLevel levelProperties)
        {
            return walls[index%walls.Count];
        }
    }
}
