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

        [SerializeField]
        Sprite titleLogo;

        public string GameName { get => gameName; }
        public Sprite TitleLogo { get => titleLogo; }
        public string SceneName { get => sceneName; set => sceneName = value; }
    }
}
