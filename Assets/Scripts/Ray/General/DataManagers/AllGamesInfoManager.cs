using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class AllGamesInfoManager : MonoBehaviour
    {
        [SerializeField]
        bool isResetRuntimeDataAtAwake = false;

        [SerializeField]
        AllGamesInfo allGamesInfo;

        public AllGamesInfo AllGamesInfo { get => allGamesInfo; }

        void Awake()
        {
            if (isResetRuntimeDataAtAwake)
                allGamesInfo.ResetAllRuntimeData();
        }

        void Update()
        {
            allGamesInfo.AddPlayTime(Time.deltaTime);
        }
    }
}
