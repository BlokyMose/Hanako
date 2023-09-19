using Hanako.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

namespace Hanako.Hub
{
    public class HubLevelManager : MonoBehaviour
    {
        [SerializeField]
        SceneLoadingManager sceneLoadingManager;

        [SerializeField]
        HubColors colors;

        [Header("UI")]
        [SerializeField]
        HubCursor playerCursor;

        [SerializeField]
        HubLevelCanvas levelCanvas;

        public HubColors Colors { get => colors; }

        void Awake()
        {
            var levelDoors = new List<HubLevelDoor>(FindObjectsByType<HubLevelDoor>(FindObjectsSortMode.None));
            foreach (var levelDoor in levelDoors)
                levelDoor.Init(ShowLevelCanvas,HideLevelCanvas, colors);

            levelCanvas.Init(LoadScene);
            levelCanvas.gameObject.SetActive(true);
        }

        public void ShowLevelCanvas(LevelInfo levelInfo)
        {
            levelCanvas.Show(levelInfo);
        }

        public void HideLevelCanvas()
        {
            levelCanvas.Hide();
        }

        public void ShowDialogueTriggerCanvas(DialogueData dialogueData)
        {

        }

        public void HideDialogueTriggerCanvas()
        {

        }

        public void LoadScene(LevelInfo levelInfo)
        {
            var allGamesInfo = FindObjectOfType<AllGamesInfoManager>();
            if (allGamesInfo != null)
                allGamesInfo.AllGamesInfo.SetCurrentLevel(levelInfo);
            sceneLoadingManager.LoadScene(levelInfo);
        }
    }
}
