using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_Immoveable : KnifePiece_NonLiving
    {
        public override void Init(KnifeLevelManager levelManager)
        {
            base.Init(levelManager);
            isInteractable = false;
        }
    }
}
