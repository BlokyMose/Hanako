using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako
{
    public class AStar_Node : MonoBehaviour
    {
        public TextMeshProUGUI costText;
        public TextMeshProUGUI distanceFromStartText;
        public TextMeshProUGUI distanceToEndText;
        public SpriteRenderer sr;
        public bool isObstacle = false;
        public bool isStart = false;
        public bool isEnd = false;
        public int x;
        public int y;

        private void OnValidate()
        {
            if (sr != null)
            {
                if (isStart)
                    sr.color = Color.green;
                else if (isEnd)
                    sr.color = Color.yellow;
                else if (!isObstacle)
                    sr.color = Color.white;
                else
                    sr.color = Color.gray;
            }
            x = (int) transform.position.x;
            y = (int) transform.position.y;
        }

        public void Set(Color color, int cost, int fromStart, int toEnd)
        {
            sr.color = color;
            costText.text = cost.ToString();
            distanceFromStartText.text = fromStart.ToString();
            distanceToEndText.text = toEnd.ToString();
        }
    }
}
