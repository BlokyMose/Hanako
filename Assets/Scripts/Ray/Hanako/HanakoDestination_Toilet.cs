using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hanako
{
    public class HanakoDestination_Toilet : HanakoDestination
    {
        [Header("SFX")]
        [SerializeField]
        AudioSourceRandom doorAudioSource;

        [SerializeField]
        string sfxDoorOpenName = "sfxDoorOpen";
        
        [SerializeField]
        string sfxDoorCloseName = "sfxDoorClose";

        [SerializeField]
        string sfxFlushName = "sfxFlush";

        int tri_open;

        protected override void Awake()
        {
            base.Awake();

            tri_open = Animator.StringToHash(nameof(tri_open));

            if (TryGetComponent<HanakoInteractable_Toilet>(out var toilet))
            {
                toilet.OnPossess += () => { occupationMode = OccupationMode.Player; };
                toilet.OnDispossess += () => { occupationMode = OccupationMode.Unoccupied; };
            }
        }

        protected override void WhenOccupationStart(HanakoEnemy enemy)
        {
            base.WhenOccupationStart(enemy);
            animator.SetTrigger(tri_open);
            doorAudioSource.PlayOneClipFromPack(sfxFlushName);
        }

        protected override void WhenOccupationEnd(HanakoEnemy enemy)
        {
            base.WhenOccupationEnd(enemy);
            animator.SetTrigger(tri_open);
        }

        // Called by animation "toilet_openClose"
        public void OnAnimationToiletIsOpened()
        {
            if (occupationMode == OccupationMode.Enemy) // enemy enters
            {
                if (currentOccupant != null)
                {
                    currentOccupant.transform.parent = postInteractPos;

                    // in case this function not called when two enemies enter & exit at the same time
                    if (lastOccupant != null && lastOccupant.transform.parent == postInteractPos)
                        lastOccupant.transform.parent = null;
                    doorAudioSource.PlayOneClipFromPack(sfxDoorOpenName);
                }
            }
            else // enemy exits
            {
                if (lastOccupant != null)
                    lastOccupant.transform.parent = null;
                doorAudioSource.PlayOneClipFromPack(sfxDoorCloseName);
            }
        }
    }
}
