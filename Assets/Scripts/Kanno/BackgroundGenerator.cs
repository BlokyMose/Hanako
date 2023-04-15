using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class BackgroundGenerator : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private GameObject backGroundPrefab;

        [SerializeField] private float backGroundToSpawn = 10;

        private List<GameObject> backGroundPool = new List<GameObject>();

        [SerializeField] private float backGround_Y_Pos = 0f;

        [SerializeField] private float backGround_X_Distance = 18f;

        private float nextBackGroundXPos;

        [SerializeField] private float generateLevelWaitTime = 11f;

        private float waitTime;

        void Start()
        {
            Genetate();
            waitTime = Time.time + generateLevelWaitTime;
        }

        // Update is called once per frame
        void Update()
        {
            CheckForBackGround();
        }

        void Genetate()
        {
            Vector3 backGroundPosition = Vector3.zero;

            GameObject newBackGround;

            for (int i = 0; i < backGroundToSpawn; i++)
            {
                backGroundPosition = new Vector3(nextBackGroundXPos, backGround_Y_Pos, 0f);

                newBackGround = Instantiate(backGroundPrefab, backGroundPosition,Quaternion.identity);

                newBackGround.transform.SetParent(transform);

                backGroundPool.Add(newBackGround);

                nextBackGroundXPos += backGround_X_Distance;
            }
        }

        void CheckForBackGround()
        {
            if(Time.time > waitTime)
            {
                SetNewBackGround();

                waitTime = Time.time + generateLevelWaitTime;
            }
        }

        void SetNewBackGround()
        {
            Vector3 backGroundPosition = Vector3.zero;

            foreach(GameObject obj in backGroundPool)
            {
                if (obj.activeInHierarchy)
                {
                    backGroundPosition = new Vector3(nextBackGroundXPos,backGround_Y_Pos,0f);
                    obj.transform.position = backGroundPosition;
                    obj.SetActive(true);

                    nextBackGroundXPos += backGround_X_Distance;
                }
            }
        }
    }
}
