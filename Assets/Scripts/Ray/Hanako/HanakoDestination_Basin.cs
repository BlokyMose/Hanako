using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.VFX;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    public class HanakoDestination_Basin : HanakoDestination
    {
        [Header("VFX")]
        [SerializeField]
        VisualEffect soapBubblePrefab;

        [SerializeField]
        string isPlayingName = "isPlaying";

        [SerializeField]
        float durationToDestroy = 3f;

        [SerializeField]
        int boo_isActive;

        protected override void Awake()
        {
            base.Awake();

            boo_isActive = Animator.StringToHash(nameof(boo_isActive));

            OnMoveOccupantEnd += (occupant, targetPos) => {
                if (targetPos == (Vector2) postInteractPos.position)
                {
                    StartCoroutine(Animating());
                    IEnumerator Animating()
                    {
                        occupant.PlayAnimation(CharacterMotion.WashHands);
                        var soapBubbleVFX = Instantiate(soapBubblePrefab);
                        soapBubbleVFX.transform.position = occupant.HandR.position;
                        occupant.LookAt(occupant.HandR);
                        animator.SetBool(boo_isActive, true);

                        var timeRemaining = interactDuration;
                        while (timeRemaining > 0)
                        {
                            soapBubbleVFX.transform.position = occupant.HandR.position;
                            timeRemaining -= Time.deltaTime;
                            yield return null;
                        }

                        Destroy(soapBubbleVFX, durationToDestroy);
                        soapBubbleVFX.SetBool(isPlayingName, false);
                        occupant.ResetLook();
                        animator.SetBool(boo_isActive, false);
                    }   
                }
            };
        }

        protected override void WhenOccupationStart(HanakoEnemy enemy)
        {
            base.WhenOccupationStart(enemy);
            enemy.transform.parent = postInteractPos;
        }

        protected override void WhenOccupationEnd(HanakoEnemy enemy)
        {
            base.WhenOccupationEnd(enemy);
            enemy.transform.parent = null;
        }
    }
}
