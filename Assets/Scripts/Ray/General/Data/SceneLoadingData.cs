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
        [SerializeField]
        LevelInfo lastLoadedLevel;

        public LevelInfo LevelInfoToLoad { get => levelInfoToLoad; }
        public LevelInfo LastLoadedLevel { get => lastLoadedLevel; }

        public void SetData(LevelInfo levelInfo)
        {
            // LevelInfoToLoad should always differ with LastLoadedLevel
            if (this.levelInfoToLoad == levelInfo) return;

            lastLoadedLevel = this.levelInfoToLoad;
            this.levelInfoToLoad = levelInfo;
        }

        public void ResetData()
        {
            this.levelInfoToLoad = null;
            this.lastLoadedLevel = null;
        }
    }
}
