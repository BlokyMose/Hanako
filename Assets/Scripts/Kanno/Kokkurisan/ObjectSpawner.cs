using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{

    public class ObjectSpawner : MonoBehaviour
    {
        public List<GameObject> objectPrefabs; // 生成するオブジェクトのプレハブリスト
        public float spawnInterval = 2.0f; // 生成の間隔（秒）
        public int numberOfObjectsToSpawn = 5; // 生成するオブジェクトの数
        public float xOffset = 2.0f; // オブジェクト間の横方向のオフセット
        public float objectLifetime = 5.0f; // オブジェクトの寿命（秒）

        private float timer = 0.0f;

        private void Update()
        {
            // タイマーを更新
            timer += Time.deltaTime;

            // 指定した間隔ごとにオブジェクトを生成
            if (timer >= spawnInterval)
            {
                List<int> indexesToExclude = new List<int>();
                int randomIndexToExclude = Random.Range(0, numberOfObjectsToSpawn);
                indexesToExclude.Add(randomIndexToExclude);

                for (int i = 0; i < numberOfObjectsToSpawn; i++)
                {
                    if (i != randomIndexToExclude)
                    {
                        // ランダムな位置にオブジェクトを生成し、位置を設定
                        Vector3 spawnPosition = transform.position + new Vector3(i * xOffset, 0, 0);
                        GameObject newObject = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Count)], spawnPosition, Quaternion.identity);

                        // オブジェクトの寿命を設定
                        Destroy(newObject, objectLifetime);
                    }
                }

                // タイマーをリセット
                timer = 0.0f;
            }
        }

    }
}

