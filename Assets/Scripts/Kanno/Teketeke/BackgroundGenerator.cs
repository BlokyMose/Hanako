using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class BackgroundGenerator : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private GameObject backGroundPrefab, groundPrefab;

        [SerializeField] private float backGroundToSpawn = 10, groundToSpawn = 10;

        private List<GameObject> backGroundPool = new List<GameObject>();
        private List<GameObject> groundPool = new List<GameObject>();

        [SerializeField] private float backGround_Y_Pos = 0f, ground_Y_Pos = 0f;

        [SerializeField] private float backGround_X_Distance = 18f, ground_X_Distance = 17.98f;

        private float nextBackGroundXPos, nextGroundXPos;

        [SerializeField] private float generateLevelWaitTime = 11f;

        private float waitTime;
        bool isStarted = false;
        public void StartGame() => isStarted = true;
        public void EndGame() => isStarted = false;

        void Start()
        {
            Genetate();
            waitTime = Time.time + generateLevelWaitTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isStarted)
                return;

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

            Vector3 groundPosition = Vector3.zero;

            GameObject newGround;

            for (int i = 0; i < groundToSpawn; i++)
            {
                groundPosition = new Vector3(nextGroundXPos, ground_Y_Pos, 0f);

                newGround = Instantiate(groundPrefab, groundPosition, Quaternion.identity);

                newGround.transform.SetParent(transform);

                groundPool.Add(newGround);

                nextGroundXPos += ground_X_Distance;

            }
        }


        void CheckForBackGround()
        {
            //îwåiÇ∆ínñ ÇèoÇ∑ä‘äu
            if(Time.time > waitTime)
            {
                SetNewGround();
                SetNewBackGround();

                waitTime = Time.time + generateLevelWaitTime;
            }
        }

        //êVÇµÇ¢ínñ ÇèoÇ∑
        void SetNewGround()
        {
            Vector3 groundPosition = Vector3.zero;

            foreach (GameObject obj in groundPool)
            {
                if (!obj.activeInHierarchy)
                {
                    groundPosition = new Vector3(nextGroundXPos, ground_Y_Pos, 0);
                    obj.transform.position = groundPosition;
                    obj.SetActive(true);

                    nextGroundXPos += ground_X_Distance;
                }
            }
        }

        //êVÇµÇ¢îwåiÇèoÇ∑
        void SetNewBackGround()
        {
            Vector3 backGroundPosition = Vector3.zero;

            foreach(GameObject obj in backGroundPool)
            {
                if (!obj.activeInHierarchy)
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
