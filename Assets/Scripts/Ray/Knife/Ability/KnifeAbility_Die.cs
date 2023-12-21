using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeAbility_Die", menuName = "SO/Knife/Ability/Die")]

    public class KnifeAbility_Die : KnifeAbility
    {
        public override void Interacted(LivingPieceCache interactorPiece, TileCache interactedTile, KnifeLevelManager levelManager)
        {
            // TODO: stream line die to only one code
            interactorPiece.Die();
            interactorPiece.LivingPiece.Die(interactorPiece);
        }
    }
}
