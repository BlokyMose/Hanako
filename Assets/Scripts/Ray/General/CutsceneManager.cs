using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Hanako
{
    public class CutsceneManager : MonoBehaviour
    {
        [SerializeField]
        PlayableDirector director;

        public void PlayAt(int time)
        {
            director.time = time;
        }
    }
}
