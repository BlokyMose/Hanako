using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Knife
{
    public class KnifePiece_Player : KnifePiece_Living
    {
        [Header("PLayer: SFX")]
        [SerializeField]
        AudioSourceRandom audioSource;

        [SerializeField]
        string sfxAttackName = "sfxAttack";

        public override void Attack(bool returnToCurrentStateAfterAttack = false)
        {
            base.Attack(returnToCurrentStateAfterAttack);
            audioSource.PlayOneClipFromPack(sfxAttackName);
        }
    }
}
