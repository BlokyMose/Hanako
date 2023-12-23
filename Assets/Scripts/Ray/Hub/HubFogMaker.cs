using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hub
{
    public class HubFogMaker : MonoBehaviour
    {
        [SerializeField]
        int fogCount = 10;

        [Header("Components")]
        [SerializeField]
        Vector2 instantiateArea = new(10,10);

        [SerializeField]
        ColliderProxy col;

        [SerializeField]
        HubFog fogPrefab;

        bool isPlaying = true;

        List<HubFog> fogs = new();
        Coroutine corPlaying;

        void Awake()
        {
            col.OnEnter += (collider) =>
            {
                if (collider.TryGetComponent<HubCharacterBrain_Player>(out var player))
                    Init();
            };

            col.OnExit += (collider) =>
            {
                if (collider.TryGetComponent<HubCharacterBrain_Player>(out var player))
                    Pause();
            };
        }

        void Init()
        {
            corPlaying = this.RestartCoroutine(Delay(0.33f), corPlaying);
            IEnumerator Delay(float delay)
            {
                for (int i = 0; i < fogCount; i++)
                {
                    var fog = Instantiate(fogPrefab, transform);
                    fog.transform.position = GetRandomPos();
                    fog.OnMoveEnd += () => Reinit(fog);
                    fogs.Add(fog);
                    yield return new WaitForSeconds(delay);
                }
            }
            void Reinit(HubFog fog)
            {
                if (!isPlaying) return;
                fog.transform.position = GetRandomPos();
                fog.Init();
            }
        }

        public void Pause()
        {
            isPlaying = false;
            this.StopCoroutineIfExists(corPlaying);
            foreach (var fog in fogs)
                Destroy(fog.gameObject,2f);
        }


        public void Play()
        {
            isPlaying = true;
            Init();
        }

        Vector2 GetRandomPos()
        {
            return new (transform.position.x + Random.Range(-instantiateArea.x / 2, instantiateArea.x / 2),
                        transform.position.y + Random.Range(-instantiateArea.y / 2, instantiateArea.y / 2));
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.8f, 0.2f, 0.5f, 0.3f);
            Gizmos.DrawCube(transform.position,instantiateArea);
        }
    }
}
