using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class ObsPpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject makibisiPrefab, rockPrefab, fallenPrefab;

        [SerializeField]
        private float makibisiYPos = -3.75f, rockYPos = -3.5f, fallenTreeYPos = -3.5f;

        [SerializeField]
        private float minSpawnWaitTime = 2f, maxSpawnWaitTime = 3.5f;

        private float spawnWaitTime;

        private int obstacleTypesCount = 3;
        private int obstacleToSpawne;

        private Camera mainCamera;

        private Vector3 obstacleSpawnePos = Vector3.zero;

        private GameObject newObstacle;

        [SerializeField]
        private List<GameObject> makibisiPool, rockPool, fallentreePool;

        [SerializeField]
        private int initialObstacleToSpawn = 5;

        private void Awake()
        {
            mainCamera = Camera.main;

            GenerateObstacles();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ObstacleSoawning();
        }

        void GenerateObstacles()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnObstacles(i);
            }
        }

        void SpawnObstacles(int obstacleType)
        {
            switch (obstacleType)
            {
                case 0:

                    for (int i = 0; i < initialObstacleToSpawn; i++)
                    {
                        newObstacle = Instantiate(makibisiPrefab);

                        newObstacle.transform.SetParent(transform);

                        makibisiPool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;

                case 1:
                    for (int i = 0; i < initialObstacleToSpawn; i++)
                    {
                        newObstacle = Instantiate(rockPrefab);

                        newObstacle.transform.SetParent(transform);

                        rockPool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;

                case 2:
                    for (int i = 0; i < initialObstacleToSpawn; i++)
                    {
                        newObstacle = Instantiate(fallenPrefab);

                        newObstacle.transform.SetParent(transform);

                        fallentreePool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;
            }
        }

        void ObstacleSoawning()
        {
            if (Time.time > spawnWaitTime)
            {
                SpawneObstacleInGame();

                spawnWaitTime = Time.time + Random.Range(minSpawnWaitTime, maxSpawnWaitTime);
            }
        }

        void SpawneObstacleInGame()
        {
            obstacleToSpawne = Random.Range(0, obstacleTypesCount);

            obstacleSpawnePos.x = mainCamera.transform.position.x + 20f;

            switch (obstacleToSpawne)
            {
                case 0:

                    for (int i = 0; i < makibisiPool.Count; i++)
                    {
                        if (!makibisiPool[i].activeInHierarchy)
                        {
                            makibisiPool[i].SetActive(true);

                            obstacleSpawnePos.y = makibisiYPos;

                            newObstacle = makibisiPool[i];

                            break;
                        }

                    }

                    break;

                case 1:

                    for (int i = 0; i < rockPool.Count; i++)
                    {
                        if (!rockPool[i].activeInHierarchy)
                        {
                            rockPool[i].SetActive(true);

                            obstacleSpawnePos.y = rockYPos;

                            newObstacle = rockPool[i];

                            break;
                        }

                    }

                    break;

                case 2:

                    for (int i = 0; i < fallentreePool.Count; i++)
                    {
                        if (!fallentreePool[i].activeInHierarchy)
                        {
                            fallentreePool[i].SetActive(true);

                            obstacleSpawnePos.y = fallenTreeYPos;

                            newObstacle = fallentreePool[i];

                            break;
                        }

                    }

                    break;
            }

            newObstacle.transform.position = obstacleSpawnePos;
        }
    }
}
