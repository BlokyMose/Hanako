using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    public class HanakoEnemyPanel : MonoBehaviour
    {
        [SerializeField]
        float warningTime = 1f;

        [SerializeField]
        float hideAnimationDuration = 0.65f;

        [Header("Components")]
        [SerializeField]
        Image icon;

        [SerializeField]
        Transform destinationParent;

        [SerializeField]
        Image destinationIconPrefab;

        [SerializeField]
        Image loadingBar;

        [SerializeField]
        Image loadingBarBG;

        Animator animator;
        int boo_show;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            loadingBar.fillAmount = 0f;
            destinationParent.DestroyChildren();
        }

        public void Init(HanakoEnemyID id, List<DestinationProperties> destinations, float initScale)
        {
            icon.sprite = id.Logo;
            foreach (var destination in destinations)
            {
                var destinationIcon = Instantiate(destinationIconPrefab, destinationParent);
                destinationIcon.sprite = destination.ID.GetLogo(destination.Index);
                destinationIcon.color = destination.ID.Color;
            }
            transform.localScale = new(initScale, initScale, initScale);
        }

        public void PlayScaleAnmation(float scale, float speed)
        {
            StartCoroutine(Scaling());
            IEnumerator Scaling()
            {
                if (transform.localScale.x < scale)
                {
                    while (transform.localScale.x < scale)
                    {
                        transform.localScale = new(
                            transform.localScale.x + speed * Time.deltaTime,
                            transform.localScale.y + speed * Time.deltaTime,
                            transform.localScale.z + speed * Time.deltaTime);
                        yield return null;
                    }
                }

                else if (transform.localScale.x > scale)
                {
                    while (transform.localScale.x > scale)
                    {
                        transform.localScale = new(
                            transform.localScale.x - speed * Time.deltaTime,
                            transform.localScale.y - speed * Time.deltaTime,
                            transform.localScale.z - speed * Time.deltaTime);
                        yield return null;
                    }
                }

                transform.localScale = new(scale, scale, scale);
            }
        }

        public void SetScale(float scale)
        {
            transform.localScale = new(scale, scale, scale);
        }

        public void HideAndDestroy()
        {
            animator.SetBool(boo_show, false);
            Destroy(gameObject, hideAnimationDuration);
        }

        public void FillLoadingBar(float duration, Color color, Color warningColor)
        {
            StartCoroutine(Update());
            IEnumerator Update()
            {
                var time = duration;
                var isWarning = false;
                loadingBar.color = color;
                while (time > 0)
                {
                    loadingBar.fillAmount = time / duration;
                    if (!isWarning && time < warningTime)
                    {
                        isWarning = true;
                        loadingBarBG.color = warningColor;
                    }

                    time -= Time.deltaTime;
                    yield return null;
                }
            }
        }
    }
}
