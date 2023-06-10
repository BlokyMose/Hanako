using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hanako
{
    public class GameEnd1 : MonoBehaviour
    {
        private static Canvas gameEndCanvas1;
        // Start is called before the first frame update

        private void Awake()
        {
            gameEndCanvas1 = GetComponent<Canvas>();
        }
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public static void GameEndShowPanel1()
        {
            Time.timeScale = 0f;

            gameEndCanvas1.enabled = true;
        }
    }
}
