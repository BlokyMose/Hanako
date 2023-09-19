using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Hanako
{
    public class AudioLevelManager : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixerMaster;

        [SerializeField]
        AudioSource audioSourceAmbience;

        [SerializeField]
        AudioSource audioSourceSFX;

        [SerializeField]
        AudioSource audioSourceBGM;

        public AudioMixer AudioMixerMaster { get => audioMixerMaster; }
        public AudioSource AudioSourceAmbience { get => audioSourceAmbience; }
        public AudioSource AudioSourceSFX { get => audioSourceSFX; }
        public AudioSource AudioSourceBGM { get => audioSourceBGM; }
    }
}
