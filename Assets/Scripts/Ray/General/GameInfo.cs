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
        GameObject loadingAnimation;

        public string GameName { get => gameName; }
        public Sprite TitleLogo { get => titleLogo; }
        public string SceneName { get => sceneName; set => sceneName = value; }
        public GameObject LoadingAnimation { get => loadingAnimation; }
        public Sprite TitleIcon { get => titleIcon; }
    }
}
