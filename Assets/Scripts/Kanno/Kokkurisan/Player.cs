using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Player : MonoBehaviour
    {
        public float followSpeed = 5.0f; // �Ǐ]�̑��x�𒲐��\�ɂ���ϐ�
        public float stopDistance = 0.1f; // �}�E�X�̈ʒu�ɒ�~���鋗��

        private Rigidbody2D rb;
        private Camera mainCamera;
        private float minX, maxX, minY, maxY;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;

            // �J�����͈̔͂��擾���A�I�u�W�F�N�g����ʊO�ɏo�Ȃ��悤�ɐ������邽�߂͈̔͂��v�Z
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
            mousePosition.z = 0f; // �I�u�W�F�N�g��Z���W�𒲐�

            // �I�u�W�F�N�g���}�E�X�̈ʒu�Ɍ������Ĉړ�������
            Vector2 moveDirection = (mousePosition - transform.position).normalized;

            // �}�E�X�̈ʒu�ɏ\���ɋ߂Â������~
            if (Vector2.Distance(transform.position, mousePosition) > stopDistance)
            {
                rb.velocity = moveDirection * followSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            // �I�u�W�F�N�g�̈ʒu����ʓ��ɐ���
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

    }
}
