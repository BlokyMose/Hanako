using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hanako
{
    public class GameManager : MonoBehaviour
    {

        public float countdownMinutes = 3;

        private float countdownSeconds;

        public TextMeshProUGUI timeText;

        // Start is called before the first frame update
        void Start()
        {        
            countdownSeconds = countdownMinutes * 60;
        }

        // Update is called once per frame
        void Update()
        {
            countdownSeconds -= Time.deltaTime;
            var span = new TimeSpan(0, 0, (int)countdownSeconds);
            timeText.text = span.ToString(@"mm\:ss");

            if (countdownSeconds <= 0)
            {
                Time.timeScale = 0;
            }
        }
    }
}
