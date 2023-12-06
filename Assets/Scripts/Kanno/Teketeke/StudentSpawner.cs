using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class StudentSpawner : MonoBehaviour
    {
        [SerializeField] private new List<GameObject> prefabs;

       // [SerializeField] private float deskYPos1 = 0.6f;
        [SerializeField] private float deskYPos2 = -0.6f;
        //[SerializeField] private float deskYPos3 = -2.3f;


        [SerializeField] private float minSpawnWaitTime = 2f, maxSpawnWaitTime = 3.5f;

        private float spawnWaitTime;

        private int obstacleTypesCount = 1;
        private int obstacleToSpawn;

        private Camera mainCamera;

        private Vector3 obstacleSpawnPos = Vector3.zero;

        private GameObject newObstacle;

        [SerializeField] private List<GameObject> deskPool;

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
            ObstacleSpawning();
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
                        newObstacle = Instantiate(prefabs[Random.Range(0, prefabs.Count)]);

                        newObstacle.transform.SetParent(transform);

                        deskPool.Add(newObstacle);
                        newObstacle.SetActive(false);
                    }

                    break;

                




            }
        }

        void ObstacleSpawning()
        {
            if (Time.time > spawnWaitTime)
            {
                SpawneObstacleInGame();

                spawnWaitTime = Time.time + Random.Range(minSpawnWaitTime, maxSpawnWaitTime);
            }
        }

        void SpawneObstacleInGame()
        {
            obstacleToSpawn = Random.Range(0, obstacleTypesCount);

            obstacleSpawnPos.x = mainCamera.transform.position.x + 20f;

            switch (obstacleToSpawn)
            {
                case 0:

                    for (int i = 0; i < deskPool.Count; i++)
                    {
                        if (!deskPool[i].activeInHierarchy)
                        {
                            deskPool[i].SetActive(true);

                            obstacleSpawnPos.y = deskYPos2;

                            newObstacle = deskPool[i];

                            break;
                        }

                    }

                    break;

                
            }

            newObstacle.transform.position = obstacleSpawnPos;
        }
    }
}
