using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class GameEnd2 : MonoBehaviour
    {
        private static Canvas gameEndCanvas2;
        // Start is called before the first frame update

        private void Awake()
        {
            gameEndCanvas2 = GetComponent<Canvas>();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void GameEndShowPanel2()
        {
            Time.timeScale = 0f;

            gameEndCanvas2.enabled = true;
        }
    }
}
