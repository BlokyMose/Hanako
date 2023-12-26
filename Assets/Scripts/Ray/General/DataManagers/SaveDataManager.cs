using DialogueSyntax;
using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static DialogueSyntax.DSyntaxUtility;

namespace Hanako
{
    public class SaveDataManager : MonoBehaviour
    {
        public enum Mode { None, AutoLoad, AutoSave }

        public Mode mode;

        [Header("Settings")]
        public AllGamesInfo allGamesInfo;
        public DSyntaxSettings DSyntaxSettings;
        public string fileName = "hanako_save_data";
        public string extensionName = "savedata";

        [Header("Debug")]
        public TextMeshProUGUI debugText;

        public const string PLAYER = nameof(PLAYER);
        public const string PLAY_TIME = nameof(PLAY_TIME);

        private void Awake()
        {
            if (mode == Mode.AutoLoad)
                TryLoadAllGamesInfo();
            else if (mode == Mode.AutoSave)
                SaveAllGamesInfo();
        }

        public bool TryLoad(out string saveDataText)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, $"{fileName}.{extensionName}");
            var isSucceedReadingSaveData = false;
            saveDataText = "";

            if (File.Exists(filePath))
            {
                try
                {
                    saveDataText = File.ReadAllText(filePath);
                    isSucceedReadingSaveData = true;
                    if (debugText != null)
                        debugText.text = saveDataText;

                    Debug.Log("Success reading save data");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading save data: "+e.Message); 
                }
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
            }

            return isSucceedReadingSaveData;
        }

        public void Save(string saveDataText)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, $"{fileName}.{extensionName}");

            try
            {
                File.WriteAllText(filePath, saveDataText);
                Debug.Log("Success writing save data");
            }
            catch (Exception e)
            {
                Debug.LogError("Error writing save data: " + e.Message);
            }
        }

        [Button]
        public void SaveAllGamesInfo()
        {
            var saveDataText = "";
            var playTimeData = WriteCommand(DSyntaxSettings, PLAY_TIME, allGamesInfo.PlayTime.ToString());
            saveDataText += playTimeData + "\n";

            foreach (var player in allGamesInfo.PlayerIDs)
            {
                var playerData = WriteCommand(DSyntaxSettings, PLAYER, new List<string>() { player.ID, player.DisplayName });
                var levelsData = "";
                foreach (var level in allGamesInfo.LevelInfos)
                {
                    var leaderboardItem = level.Leaderboard.Find(x => x.PlayerID == player.ID);
                    if (leaderboardItem != null)
                        levelsData += WriteCommand(DSyntaxSettings, GetLevelSaveID(level), leaderboardItem.Score.ToString());
                }
                playerData += levelsData;
                saveDataText += playerData + "\n";
            }

            Save(saveDataText);

            static string GetLevelSaveID(LevelInfo level)=>level.GameInfo.GameID+"_"+level.LevelName;
        }

        [Button]
        public void LoadAllGamesInfo()
        {
            if (!TryLoad(out var saveDataText))
                return;

            allGamesInfo.ResetAllRuntimeData();

            var allCommands = ReadCommands(DSyntaxSettings, saveDataText);
            var playTimeData = allCommands[0].GetParameter(DSyntaxSettings, 0);
            if (!float.TryParse(playTimeData, out var playTime))
                Debug.LogWarning("ERROR: parsing playTime");

            var playerIDs = new List<PlayerID>();
            var playerDataGroups = ReadCommandsByGroups(DSyntaxSettings, saveDataText, PLAYER);
            foreach (var playerDataGroup in playerDataGroups)
            {
                var playerID = playerDataGroup[0].GetParameter(DSyntaxSettings, 0);
                var playerDisplayName = playerDataGroup[0].GetParameter(DSyntaxSettings, 1);
                playerIDs.Add(new (playerID, playerDisplayName));

                for (int i = 1; i < playerDataGroup.Count; i++)
                {
                    var levelData = playerDataGroup[i];
                    GetGameIDAndLevelName(levelData, out var gameID, out var levelName);
                    var score = levelData.GetParameter(DSyntaxSettings, 0);

                    var foundLevelInfo = allGamesInfo.LevelInfos.Find(lvl => lvl.GameInfo.GameID == gameID && lvl.LevelName == levelName);
                    if (foundLevelInfo != null && int.TryParse(score, out int scoreINT))
                        foundLevelInfo.AddLeaderboardItem(new(scoreINT, playerID));
                }
            }

            allGamesInfo.LoadData(playTime, playerIDs);


            static void GetGameIDAndLevelName(Command command, out string gameID, out string levelName)
            {
                var data = command.name.Split('_');
                gameID = data.GetAt(0,"");
                levelName = data.GetAt(1,"");
            }
        }

        [Button]
        public bool TryLoadAllGamesInfo()
        {
            var isLoaded = false;
            try
            {
                LoadAllGamesInfo();
                isLoaded = true;
            }
            catch (Exception e)
            {
                Debug.Log(nameof(e) + " : " + e.Message);
            }

            return isLoaded;
        }
    }
}
