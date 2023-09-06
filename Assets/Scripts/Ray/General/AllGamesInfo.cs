using Encore.Utility;
using Hanako.Hub;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="AllGamesInfo", menuName ="SO/All Games Info")]

    public class AllGamesInfo : ScriptableObject
    {
        [SerializeField]
        List<GameInfo> gameInfos = new();

        [SerializeField]
        List<LevelInfo> levelInfos = new();

        [SerializeField]
        float playTime;

        public List<GameInfo> GameInfos { get => gameInfos; }
        public List<LevelInfo> LevelInfos { get => levelInfos; }
        public float PlayTime { get => playTime; }

        public int CurrentSoulCount
        {
            get
            {
                var currentSoulCount = 0;
                foreach (var level in levelInfos)
                    currentSoulCount += level.CurrentSoulCount;
                return currentSoulCount;
            }
        }

        public int MaxSoulCount
        {
            get
            {
                var maxSoulCount = 0;
                foreach (var level in levelInfos)
                    maxSoulCount += level.MaxSoulCount;
                return maxSoulCount;
            }
        }

        public void AddPlayTime(float increment) => playTime += increment;

        public void ResetAllRuntimeData()
        {
            playTime = 0;
            foreach (var level in levelInfos)
                level.ResetRuntimeData();
        }

        [Button]
        void UpdateInfosByLevelDoors()
        {
            var levelDoors = new List<HubLevelDoor>(FindObjectsByType<HubLevelDoor>(FindObjectsSortMode.None));
            foreach (var levelDoor in levelDoors)
            {
                levelInfos.AddIfHasnt(levelDoor.LevelInfo);
                gameInfos.AddIfHasnt(levelDoor.LevelInfo.GameInfo);
            }
        }
    }
}
