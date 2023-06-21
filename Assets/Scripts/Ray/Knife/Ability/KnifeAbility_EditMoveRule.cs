using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_MoveRuleName", menuName = "SO/Knife/Ability/Edit Move Rule")]

    public class KnifeAbility_EditMoveRule : KnifeAbility
    {
        [SerializeField]
        KnifeMoveRule moveRule;

        public override void Interacted(LivingPieceCache otherPiece, TileCache myTile, KnifeLevelManager levelManager)
        {
            otherPiece.LivingPiece.MoveRule = moveRule;
        }
    }
}
