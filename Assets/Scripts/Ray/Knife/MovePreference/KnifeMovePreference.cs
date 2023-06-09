using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifeMovePreference : ScriptableObject
    {
        [SerializeField]
        string preferenceName;

        public abstract ColRow GetPrefferedMove(
            List<ColRow> validMoves, 
            PieceCache thisPiece,
            List<PieceCache> allPieces,
            KnifeLevel levelProperties
            );
    }
}
