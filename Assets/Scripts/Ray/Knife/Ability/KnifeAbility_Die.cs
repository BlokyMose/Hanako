using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_Die", menuName = "SO/Knife/Ability/Die")]

    public class KnifeAbility_Die : KnifeAbility
    {
        public override void Interacted(LivingPieceCache otherPiece, TileCache myTile, KnifeLevelManager levelManager)
        {
            // TODO: stream line die to only one code
            otherPiece.Die();
            otherPiece.LivingPiece.Die(otherPiece);
        }
    }
}
