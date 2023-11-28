using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="GameInfo_", menuName ="SO/Game Info")]

    public class GameInfo : ScriptableObject
    {
        [SerializeField]
        string gameID;

        [SerializeField]
        string gameDisplayName;

        [SerializeField]
        string sceneName;

        [SerializeField, PreviewField]
        Sprite titleLogo;
        
        [SerializeField, PreviewField]
        Sprite titleIcon;

        [SerializeField]
        TutorialInfo tutorialInfo;

        [SerializeField]
        GameObject loadingAnimation;

        public string GameID { get => gameID; }
        public string GameDisplayName { get => gameDisplayName; }
        public string SceneName { get => sceneName; set => sceneName = value; }
        public Sprite TitleLogo { get => titleLogo; }
        public Sprite TitleIcon { get => titleIcon; }
        public TutorialInfo TutorialInfo { get => tutorialInfo;  }
        public GameObject LoadingAnimation { get => loadingAnimation; }
    }
}
