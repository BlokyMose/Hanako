using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Player : MonoBehaviour
    {
        public float followSpeed = 5.0f; // 追従の速度を調整可能にする変数
        public float stopDistance = 0.1f; // マウスの位置に停止する距離

        private Rigidbody2D rb;
        private Camera mainCamera;
        private float minX, maxX, minY, maxY;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;

            // カメラの範囲を取得し、オブジェクトが画面外に出ないように制限するための範囲を計算
            float objectWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2;
            float objectHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2;

            Vector2 cameraMin = mainCamera.ViewportToWorldPoint(Vector2.zero);
            Vector2 cameraMax = mainCamera.ViewportToWorldPoint(Vector2.one);

            minX = cameraMin.x + objectWidth;
            maxX = cameraMax.x - objectWidth;
            minY = cameraMin.y + objectHeight;
            maxY = cameraMax.y - objectHeight;
        }

        private void Update()
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // オブジェクトのZ座標を調整

            // オブジェクトをマウスの位置に向かって移動させる
            Vector2 moveDirection = (mousePosition - transform.position).normalized;

            // マウスの位置に十分に近づいたら停止
            if (Vector2.Distance(transform.position, mousePosition) > stopDistance)
            {
                rb.velocity = moveDirection * followSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            // オブジェクトの位置を画面内に制限
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

    }
}
