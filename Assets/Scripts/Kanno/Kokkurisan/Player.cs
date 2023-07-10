using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Player : MonoBehaviour
    {
        // X, Y座標の移動可能範囲
        [System.Serializable]
        public class Bounds
        {
            public float xMin, xMax, yMin, yMax;
        }
        [SerializeField] Bounds bounds;

        [SerializeField, Range(0f, 1f)] private float followStrength;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            // マウス位置をスクリーン座標からワールド座標に変換する
            var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // X, Y座標の範囲を制限する
            targetPos.x = Mathf.Clamp(targetPos.x, bounds.xMin, bounds.xMax);
            targetPos.y = Mathf.Clamp(targetPos.y, bounds.yMin, bounds.yMax);

            // Z座標を修正する
            targetPos.z = 0f;

            // このスクリプトがアタッチされたゲームオブジェクトを、マウス位置に線形補間で追従させる
            transform.position = Vector3.Lerp(transform.position, targetPos, followStrength);
        }
    }
}
