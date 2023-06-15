using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_MoveRuleName", menuName = "SO/Knife/Ability/Edit Move Rule")]

    public class KnifeAbility_EditMoveRule : KnifeAbility
    {
        [SerializeField]
        KnifeMoveRule moveRule;

        public override void Interacted(KnifeLevelManager.LivingPieceCache otherPiece, KnifeLevelManager.TileCache myTile)
        {
            otherPiece.LivingPiece.MoveRule = moveRule;
        }
    }
}
