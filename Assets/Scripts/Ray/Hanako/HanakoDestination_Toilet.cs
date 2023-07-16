using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        protected override void WhenInteractStart(HanakoEnemy enemy)
        {
            base.WhenInteractStart(enemy);
        }

        public void Hover()
        {
            foreach (var sr in srs)
            {
                sr.color = Color.red;
            }
        }

        public void Unhover()
        {
            foreach (var sr in srs)
            {
                sr.color = Color.white;
            }

        }
    }
}
