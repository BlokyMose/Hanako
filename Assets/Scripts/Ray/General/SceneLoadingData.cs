using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="SceneLoadingData", menuName ="SO/Scene Loading Data")]

    public class SceneLoadingData : ScriptableObject
    {
        [SerializeField, Tooltip("Use levelInfo's data when playing")]
        bool isActive = true;

        [SerializeField]
        LevelInfo levelInfo;

        public bool IsActive { get => isActive; }
        public LevelInfo LevelInfo { get => levelInfo; }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }


        public void SetData(LevelInfo levelInfo)
        {
            this.levelInfo = levelInfo;
        }
    }
}
