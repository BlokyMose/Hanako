using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace Hanako.Hub
{
    public class HubGhostMaker : MonoBehaviour
    {
        [SerializeField]
        int ghostCount = 10;

        [Header("Components")]
        [SerializeField]
        Vector2 instantiateArea = new(10,10);

        [SerializeField]
        HubGhost ghostPrefab;

        bool isPlaying = true;

        List<HubGhost> ghosts = new();
        Coroutine corPlaying;

        void Start()
        {
            Init();
        }

        void Init()
        {
            corPlaying = this.RestartCoroutine(Delay(0.33f), corPlaying);
            IEnumerator Delay(float delay)
            {
                for (int i = 0; i < ghostCount; i++)
                {
                    var ghost = Instantiate(ghostPrefab, transform);
                    ghost.transform.position = GetRandomPos();
                    ghost.OnMoveEnd += () => Reinit(ghost);
                    ghosts.Add(ghost);
                    yield return new WaitForSeconds(delay);
                }
            }
            void Reinit(HubGhost ghost)
            {
                if (!isPlaying) return;
                ghost.transform.position = GetRandomPos();
                ghost.Init();
            }
        }

        public void Pause()
        {
            isPlaying = false;
            this.StopCoroutineIfExists(corPlaying);
        }


        public void Play()
        {
            isPlaying = true;
            Init();
        }

        Vector2 GetRandomPos()
        {
            return new(transform.position.x + Random.Range(-instantiateArea.x / 2, instantiateArea.x / 2),
                        transform.position.y + Random.Range(-instantiateArea.y / 2, instantiateArea.y / 2));
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.1f, 0.5f, 0.9f, 0.3f);
            Gizmos.DrawCube(transform.position, instantiateArea);
        }
    }
}
