using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class ObstaclePpawner : MonoBehaviour
    {
        [SerializeField] private GameObject deskPrefab, chairPrefab, lecternPrefab;

        [SerializeField] private float deskYpos1 = 0.6f;
        [SerializeField] private float deskYpos2 = -0.6f;
        [SerializeField] private float deskYpos3 = -2.3f;

        [SerializeField] private float chairYpos1 = 0.6f;
        [SerializeField] private float chairpos2 = -0.6f;
        [SerializeField] private float chairYpos3 = -2.3f;

        [SerializeField] private float lecternYpos1 = 0.6f;
        [SerializeField] private float lecternYpos2 = -0.6f;
        [SerializeField] private float lecternYpos3 = -2.3f;

        [SerializeField] private float minSpawnWaitTime = 2f, maxSpawnWaitTime = 3.5f;

        private float spawnWaitTime;

        private int obstacleTypesCount = 3;
        private int obstacleToSpawn;

        private Camera mainCamera;

        private Vector3 obstacleSpawnPos = Vector3.zero;

        private GameObject newObstacle;

        [SerializeField] private List<GameObject> deskPool, chairPool, lecternPool;

        [SerializeField] private int initialObstacleToSpawn = 5;

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
                        newObstacle = Instantiate(deskPrefab);

                        newObstacle.transform.SetParent(transform);

                        deskPool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;

                 case 1:
                    for (int i = 0; i < initialObstacleToSpawn; i++)
                    {
                        newObstacle = Instantiate(chairPrefab);

                        newObstacle.transform.SetParent(transform);

                        chairPool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;

                case 2:
                    for (int i = 0; i < initialObstacleToSpawn; i++)
                    {
                        newObstacle = Instantiate(lecternPrefab);

                        newObstacle.transform.SetParent(transform);

                        lecternPool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;


            }
        }

        void ObstacleSpawning()
        {
            if(Time.time > spawnWaitTime)
            {
                spawnWaitTime = Time.time + Random.Range(minSpawnWaitTime,maxSpawnWaitTime);
            }
        }
    }
}
