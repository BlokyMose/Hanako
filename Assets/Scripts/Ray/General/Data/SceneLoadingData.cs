using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="SceneLoadingData", menuName ="SO/Scene Loading Data")]

    public class SceneLoadingData : ScriptableObject
    {
        [SerializeField]
        LevelInfo levelInfoToLoad;

        public LevelInfo LevelInfoToLoad { get => levelInfoToLoad; }

        public void SetData(LevelInfo levelInfo)
        {
            this.levelInfoToLoad = levelInfo;
        }
    }
}
