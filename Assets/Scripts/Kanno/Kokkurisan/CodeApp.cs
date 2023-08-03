using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class CodeApp : MonoBehaviour
    {
        public float span = 3f;
        private float currentTime = 0f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime > span)
            {
                GameObject code1 = (GameObject)Resources.Load("Circle1");
                Instantiate(code1, new Vector3(0.0f, -4.0f, 0.0f), Quaternion.identity);

                GameObject code2 = (GameObject)Resources.Load("Circle2");
                Instantiate(code2, new Vector3(1.3f, -4.0f, 0.0f), Quaternion.identity);

                GameObject code3 = (GameObject)Resources.Load("Circle3");
                Instantiate(code3, new Vector3(2.6f, -4.0f, 0.0f), Quaternion.identity);

                GameObject code4 = (GameObject)Resources.Load("Circle4");
                Instantiate(code4, new Vector3(3.9f, -4.0f, 0.0f), Quaternion.identity);

                GameObject code5 = (GameObject)Resources.Load("Circle5");
                Instantiate(code5, new Vector3(-1.3f, -4.0f, 0.0f), Quaternion.identity);

                GameObject code6 = (GameObject)Resources.Load("Circle6");
                Instantiate(code6, new Vector3(-2.6f, -4.0f, 0.0f), Quaternion.identity);

                GameObject code7 = (GameObject)Resources.Load("Circle7");
                Instantiate(code7, new Vector3(-3.9f, -4.0f, 0.0f), Quaternion.identity);

                currentTime = 0f;
            }
        }
    }
}
