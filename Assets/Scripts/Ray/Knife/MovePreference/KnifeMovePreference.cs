using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;
using static Hanako.Knife.KnifePiece_Enemy;

namespace Hanako.Knife
{
    public abstract class KnifeMovePreference : ScriptableObject
    {
        [SerializeField]
        string preferenceName;

        public abstract void Evaluate(List<PrefferedTile> tiles, int influence, PieceCache thisPiece, List<PieceCache> allPieces, KnifeLevel levelProperties);
    }
}
