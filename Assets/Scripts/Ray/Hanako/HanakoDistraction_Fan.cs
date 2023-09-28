using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoDistraction_Fan : HanakoDistraction
    {
        [Header("VFX")]
        [SerializeField]
        List<VisualEffect> distractVFXs = new();

        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom fanAudioSource;

        [SerializeField]
        string sfxFanSpinName = "sfxFanSpin";
        protected override void WhenDistractStart()
        {
            foreach (var vfx in distractVFXs)
                vfx.SetBool("isPlaying", true);
            fanAudioSource.PlayOneClipFromPack(sfxFanSpinName);
        }

        protected override void WhenDistractEnd()
        {
            foreach (var vfx in distractVFXs)
                vfx.SetBool("isPlaying", false);
            fanAudioSource.Stop();
        }
    }
}
