using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{

    public class ObjectSpawner : MonoBehaviour
    {
        public List<GameObject> objectPrefabs; // ��������I�u�W�F�N�g�̃v���n�u���X�g
        public float spawnInterval = 2.0f; // �����̊Ԋu�i�b�j
        public int numberOfObjectsToSpawn = 5; // ��������I�u�W�F�N�g�̐�
        public float xOffset = 2.0f; // �I�u�W�F�N�g�Ԃ̉������̃I�t�Z�b�g
        public float objectLifetime = 5.0f; // �I�u�W�F�N�g�̎����i�b�j

        private float timer = 0.0f;

        private void Update()
        {
            // �^�C�}�[���X�V
            timer += Time.deltaTime;

            // �w�肵���Ԋu���ƂɃI�u�W�F�N�g�𐶐�
            if (timer >= spawnInterval)
            {
                List<int> indexesToExclude = new List<int>();
                int randomIndexToExclude = Random.Range(0, numberOfObjectsToSpawn);
                indexesToExclude.Add(randomIndexToExclude);

                for (int i = 0; i < numberOfObjectsToSpawn; i++)
                {
                    if (i != randomIndexToExclude)
                    {
                        // �����_���Ȉʒu�ɃI�u�W�F�N�g�𐶐����A�ʒu��ݒ�
                        Vector3 spawnPosition = transform.position + new Vector3(i * xOffset, 0, 0);
                        GameObject newObject = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Count)], spawnPosition, Quaternion.identity);

                        // �I�u�W�F�N�g�̎�����ݒ�
                        Destroy(newObject, objectLifetime);
                    }
                }

                // �^�C�}�[�����Z�b�g
                timer = 0.0f;
            }
        }

    }
}

