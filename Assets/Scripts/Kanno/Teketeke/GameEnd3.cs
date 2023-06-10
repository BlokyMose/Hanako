using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class GameEnd3 : MonoBehaviour
    {
        private static Canvas gameEndCanvas3;
        // Start is called before the first frame update

        private void Awake()
        {
            gameEndCanvas3 = GetComponent<Canvas>();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void GameEndShowPanel3()
        {
            Time.timeScale = 0f;

            gameEndCanvas3.enabled = true;
        }
    }
}
