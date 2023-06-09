using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public abstract class KnifePiece : MonoBehaviour
    {
        protected KnifeLevelManager levelManager;

        public virtual void Init(KnifeLevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

    }
}
