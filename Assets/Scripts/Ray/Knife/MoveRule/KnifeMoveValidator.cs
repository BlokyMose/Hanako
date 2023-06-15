using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;

namespace Hanako.Knife
{
    public abstract class KnifeMoveValidator : ScriptableObject
    {
        [SerializeField]
        protected string validatorName;

        [SerializeField]
        protected bool reverseResult = false;

        public abstract bool Validate(ColRow colRow, KnifeLevelManager.PieceCache thisPiece, List<KnifeLevelManager.PieceCache> allPieces, KnifeLevel levelProperties);
    }
}
