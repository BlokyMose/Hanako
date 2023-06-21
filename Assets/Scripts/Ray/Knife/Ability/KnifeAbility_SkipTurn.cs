using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_SkipMove", menuName = "SO/Knife/Ability/Skip Move")]

    public class KnifeAbility_SkipTurn : KnifeAbility
    {
        [SerializeField]
        int count = 1;

        public override void Interacted(LivingPieceCache otherPiece, TileCache myTile, KnifeLevelManager levelManager)
        {
            levelManager.RemoveTurnOf(otherPiece, count);
        }
    }
}
