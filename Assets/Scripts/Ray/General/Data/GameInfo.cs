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
        string gameName;

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

        public string GameName { get => gameName; }
        public string SceneName { get => sceneName; set => sceneName = value; }
        public Sprite TitleLogo { get => titleLogo; }
        public Sprite TitleIcon { get => titleIcon; }
        public TutorialInfo TutorialInfo { get => tutorialInfo;  }
        public GameObject LoadingAnimation { get => loadingAnimation; }
    }
}
