using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_Enemy : KnifePiece_Living
    {
        [SerializeField]
        KnifeMovePreference movePreference;

        public override void PleaseMove(Action onMoveDone)
        {
        }
    }
}
