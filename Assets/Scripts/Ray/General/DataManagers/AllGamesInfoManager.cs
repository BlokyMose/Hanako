using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class AllGamesInfoManager : MonoBehaviour
    {
        [SerializeField]
        AllGamesInfo allGamesInfo;

        public AllGamesInfo AllGamesInfo { get => allGamesInfo; }

        void Update()
        {
            allGamesInfo.AddPlayTime(Time.deltaTime);
        }

        public void ResetRuntimeData()
        {
            allGamesInfo.ResetAllRuntimeData();
        }
    }
}
