using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtility;

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


        [Header("Components")]
        [SerializeField]
        Animator animator;
        
        [SerializeField]
        SpriteRendererEditor srEditor;

        int boo_isCrawling;

        void Awake()
        {
            if (animator==null) animator = gameObject.GetComponentInFamily<Animator>();
            if (srEditor==null) srEditor= gameObject.GetComponentInFamily<SpriteRendererEditor>();

            boo_isCrawling = Animator.StringToHash(nameof(boo_isCrawling));
            srEditor.ChangeAlpha(0f);
        }

        public void Crawl(HanakoDestination_Toilet fromToilet, HanakoDestination_Toilet toToilet, float duration)
        {
            StartCoroutine(crawling());
            IEnumerator crawling()
            {
                fromToilet.PlayAnimationUnpossessed();
                animator.SetBool(boo_isCrawling, true);
                srEditor.BeOpaque(changeAlphaDuration);

                transform.localEulerAngles = new Vector3(0, 0, 90);
                transform.position = fromToilet.transform.position;

                var movePos = new List<Vector2>() 
                {
                    new Vector3(fromToilet.transform.position.x, fromToilet.transform.position.y + moveHeight, 0),
                    new Vector3(toToilet.transform.position.x, toToilet.transform.position.y + moveHeight,0),
                    new Vector3(toToilet.transform.position.x, toToilet.transform.position.y + endYHeightOffset)
                };

                var durationPerPos = duration / movePos.Count;
                var index = 0;
                var isTransparent = false;
                var isAnimationtransition = false;
                foreach (var pos in movePos)
                {
                    var time = 0f;
                    var originPos = transform.position;
                    var originRot = transform.rotation;
                    
                    // Calculating target rotation
                    var direction = pos - (Vector2)transform.position;
                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    var targetQuaternion = Quaternion.Euler(0, 0, angle);
                    var durationPerPosRot = durationPerPos / 1.5f;

                    while (time < durationPerPos)
                    {
                        time += Time.deltaTime;
                        transform.position = Vector2.LerpUnclamped(originPos, pos, time / durationPerPos);
                        transform.rotation = Quaternion.Lerp(originRot, targetQuaternion, time / durationPerPosRot);

                        if (index == movePos.Count - 1)
                        {
                            if (!isAnimationtransition)
                            {
                                isAnimationtransition = true;
                                animator.SetBool(boo_isCrawling, false);

                            }

                            if (!isTransparent && 
                                time > durationPerPos - changeAlphaDuration)
                            {
                                toToilet.PlayAnimationPossessed();
                                isTransparent = true;
                                srEditor.BeTransparent(0.2f);
                            }
                        }


                        yield return null;
                    }
                    index++;
                }
            }
        }
    }
}
