using Encore.Utility;
using Hanako.Hub;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityUtility;

namespace Hanako
{
    [CreateAssetMenu(fileName ="AllGamesInfo", menuName ="SO/All Games Info")]

    public class AllGamesInfo : ScriptableObject
    {
        public enum AudioVolume { Mute, Low, Mid, High , Full }
        public static float GetAudioVolumeValue(AudioVolume audioVolume)
        {
            return audioVolume switch
            {
                AudioVolume.Mute => 0f,
                AudioVolume.Low => 0.25f,
                AudioVolume.Mid => 0.5f,
                AudioVolume.High => 0.75f,
                AudioVolume.Full => 1f,
                _ => 0f
            };
        }

        [SerializeField]
        LevelInfo hubLevelInfo;

        [SerializeField]
        List<GameInfo> gameInfos = new();

        [SerializeField]
        List<LevelInfo> levelInfos = new();

        [SerializeField]
        float playTime;

        [SerializeField]
        LevelInfo currentLevel;

        [Header("Audio Settings")]
        [SerializeField]
        AudioMixer audioMixer;

        [SerializeField]
        string masterVolumeName = "MASTER_VOLUME";

        [SerializeField]
        string sfxVolumeName = "SFX_VOLUME";

        [SerializeField]
        string bgmVolumeName = "BGM_VOLUME";

        [SerializeField]
        string ambienceVolumeName = "AMBIENCE_VOLUME";

        [SerializeField]
        AudioVolume sfxVolume = AudioVolume.Mid;

        [SerializeField]
        AudioVolume bgmVolume = AudioVolume.Mid;

        [SerializeField]
        AudioVolume ambienceVolume = AudioVolume.Mid;

        public LevelInfo HubLevelInfo { get => hubLevelInfo; }
        public List<GameInfo> GameInfos { get => gameInfos; }
        public List<LevelInfo> LevelInfos { get => levelInfos; }
        public float PlayTime { get => playTime; }
        public AudioVolume SFXVolume { get => sfxVolume; }
        public AudioVolume BGMVolume { get => bgmVolume; }
        public AudioVolume AmbienceVolume { get => ambienceVolume; }

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

        public LevelInfo CurrentLevel { get => currentLevel; }

        public void SetCurrentLevel(LevelInfo levelInfo) => this.currentLevel = levelInfo;

        public void AddPlayTime(float increment) => playTime += increment;

        [Button]
        public void ResetAllRuntimeData()
        {
            playTime = 0;
            foreach (var level in levelInfos)
                level.ResetRuntimeData();
        }

        public AudioVolume SpinSFXVolume()
        {
            sfxVolume = (AudioVolume)(((int)(sfxVolume + 1)) % Enum.GetNames(typeof(AudioVolume)).Length);
            audioMixer.SetFloatLog(sfxVolumeName, GetAudioVolumeValue(sfxVolume));
            return sfxVolume;
        }

        public AudioVolume SpinBGMVolume()
        {
            bgmVolume = (AudioVolume)(((int)(bgmVolume + 1)) % Enum.GetNames(typeof(AudioVolume)).Length);
            audioMixer.SetFloatLog(bgmVolumeName, GetAudioVolumeValue(bgmVolume));
            return bgmVolume;
        }

        public AudioVolume SpinAmbienceVolume()
        {
            ambienceVolume = (AudioVolume)(((int)(ambienceVolume + 1)) % Enum.GetNames(typeof(AudioVolume)).Length);
            audioMixer.SetFloatLog(ambienceVolumeName, GetAudioVolumeValue(ambienceVolume));
            return ambienceVolume;
        }

        public IEnumerator FadeMasterVolume(float targetVolume, float duration)
        {
            if (targetVolume > 1f) targetVolume = 1f;
            else if (targetVolume < 0f) targetVolume = 0f;

            var curve = AnimationCurve.Linear(0, audioMixer.GetFloatExp(masterVolumeName), duration, targetVolume);
            var time = 0f;
            while (time < duration)
            {
                audioMixer.SetFloatLog(masterVolumeName, curve.Evaluate(time));
                time += Time.deltaTime;
                yield return null;
            }
            audioMixer.SetFloatLog(masterVolumeName, targetVolume);
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
