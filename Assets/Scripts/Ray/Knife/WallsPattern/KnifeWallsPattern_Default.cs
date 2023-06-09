using Hanako.Knife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="KnifeWalls_Default_", menuName = "SO/Knife/Walls Pattern/Default")]

    public class KnifeWallsPattern_Default : KnifeWallsPattern
    {
        [SerializeField]
        GameObject wall;

        public override GameObject GetLeftWall(int index, KnifeLevel levelProperties)
        {
            return wall;
        }

        public override GameObject GetRightWall(int index, KnifeLevel levelProperties)
        {
            return wall;
        }
    }
}
