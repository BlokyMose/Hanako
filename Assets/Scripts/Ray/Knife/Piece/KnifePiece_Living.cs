using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public abstract class KnifePiece_Living : KnifePiece
    {
        [SerializeField]
        protected KnifeMoveRule moveRule;
        public KnifeMoveRule MoveRule { get => moveRule; }

        public abstract void PleaseMove(Action onMoveDone);

    }
}
