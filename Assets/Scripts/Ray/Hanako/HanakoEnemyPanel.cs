using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Hanako
{
    [RequireComponent(typeof(Animator))]
    public class HanakoEnemyPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        Image icon;

        [SerializeField]
        Transform destinationParent;

        [SerializeField]
        Image destinationIconPrefab;

        [SerializeField]
        Image loadingBar;

        Animator animator;
        int boo_show;
        Coroutine corScaling;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            loadingBar.fillAmount = 0f;
            destinationParent.DestroyChildren();
        }

        public void Init(HanakoEnemyID id, List<HanakoDestinationID> destinations, float initScale)
        {
            icon.sprite = id.Logo;
            foreach (var destination in destinations)
            {
                var destinationIcon = Instantiate(destinationIconPrefab, destinationParent);
                destinationIcon.sprite = destination.Logo;
                destinationIcon.color = destination.Color;
            }
            transform.localScale = new(initScale, initScale, initScale);
        }

        public void SetScale(float scale, float speed)
        {
            corScaling = this.RestartCoroutine(Scaling(),corScaling);
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

        public void Hide()
        {
            animator.SetBool(boo_show, false);
        }

        public void FillLoadingBar(float duration, Color color)
        {
            StartCoroutine(Update());
            IEnumerator Update()
            {
                var time = 0f;
                loadingBar.color = color;
                while (time < duration)
                {
                    loadingBar.fillAmount = time / duration;
                    time += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }
}
