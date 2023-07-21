using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtility;
using static UnityEditor.PlayerSettings;

namespace Hanako.Hanako
{
    public class HanakoCrawl : MonoBehaviour
    {
        [SerializeField]
        float moveHeight = 6f;

        [SerializeField]
        float endYHeightOffset = 2f;

        [SerializeField]
        float changeAlphaDuration = 0.1f;

        [SerializeField]
        bool isAutoInvisible = true;


        [Header("Components")]
        [SerializeField]
        VisualEffect pebbleVFX;

        [SerializeField]
        Animator animator;
        
        [SerializeField]
        SpriteRendererEditor srEditor;

        [SerializeField]
        Transform faceTarget;

        int boo_isCrawling;
        Coroutine corCrawling;

        void Awake()
        {
            if (animator==null) animator = gameObject.GetComponentInFamily<Animator>();
            if (srEditor==null) srEditor= gameObject.GetComponentInFamily<SpriteRendererEditor>();

            boo_isCrawling = Animator.StringToHash(nameof(boo_isCrawling));
            if (isAutoInvisible)
                srEditor.ChangeAlpha(0f);

            pebbleVFX.SetBool("isPlaying", false);
        }

        public void Crawl(HanakoDestination_Toilet fromToilet, HanakoDestination_Toilet toToilet, float duration)
        {
            corCrawling = this.RestartCoroutine(Crawling(), corCrawling);
            IEnumerator Crawling()
            {
                fromToilet.PlayAnimationUnpossessed();
                fromToilet.PlayAnimationHanakoHides();
                animator.SetBool(boo_isCrawling, true);
                srEditor.BeOpaque(changeAlphaDuration);

                Vector3 from = fromToilet.transform.position;
                Vector3 position = toToilet.transform.position;
                var movePos = new List<Vector2>() 
                {
                    new Vector3(from.x, from.y + moveHeight/2, 0),
                    new Vector3(from.x, from.y + moveHeight, 0),
                    new Vector3(position.x, position.y + moveHeight,0),
                    new Vector3(position.x, position.y + endYHeightOffset)
                };                
                var moveDuration = new List<float>() 
                {
                    duration*0.25f,
                    duration*0.15f,
                    duration*0.30f,
                    duration*0.30f,
                };

                transform.SetPositionAndRotation(from, Quaternion.Euler(0, 0, MathUtility.GetAngle(transform.position, movePos[0])));
                faceTarget.transform.position =  movePos[1];

                var index = 0;
                var isTransparent = false;
                var isAnimationtransition = false;
                var isPebbleVFXActive = false;
                foreach (var currentMovePos in movePos)
                {
                    var time = 0f;
                    var originPos = transform.position;
                    var originPos_Target = faceTarget.transform.position;
                    var originRot = transform.rotation;
                    var targetRotation = Quaternion.Euler(0, 0, MathUtility.GetAngle(transform.position, currentMovePos));
                    var duration_Rotation = moveDuration[index] / 1.5f;

                    if (!isPebbleVFXActive && index == 1)
                    {
                        isPebbleVFXActive = true;
                        pebbleVFX.SetBool("isPlaying", true);
                    }


                    while (time < moveDuration[index])
                    {
                        time += Time.deltaTime;
                        transform.position = Vector2.Lerp(originPos, currentMovePos, time /  moveDuration[index]);
                        if (index < movePos.Count - 1)
                        {
                            faceTarget.transform.position = Vector2.Lerp(originPos_Target, movePos[index+1], time / moveDuration[index] );
                        }
                        else
                        {
                            faceTarget.transform.position = movePos[index];
                        }

                        transform.rotation = Quaternion.Lerp(originRot, targetRotation, time / duration_Rotation);

                        if (index == movePos.Count - 1)
                        {
                            if (!isAnimationtransition)
                            {
                                isAnimationtransition = true;
                                animator.SetBool(boo_isCrawling, false);
                                pebbleVFX.SetBool("isPlaying", false);
                            }

                            if (!isTransparent && 
                                time > moveDuration[index] - changeAlphaDuration)
                            {
                                toToilet.PlayAnimationPossessed();
                                isTransparent = true;
                                srEditor.BeTransparent(changeAlphaDuration);
                            }
                        }

                        yield return null;
                    }

                    index++;
                }

                toToilet.PlayAnimationHanakoPeeks();
            }
        }
    }
}
