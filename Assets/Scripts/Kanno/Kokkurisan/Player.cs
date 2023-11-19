using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Player : MonoBehaviour
    {
        public float moveSpeed = 5f; // �v���C���[�̏������x
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // �}�E�X�̈ʒu���擾
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // �v���C���[�̈ʒu���}�E�X�Ɍ����Ĉړ�
            transform.position = Vector3.MoveTowards(transform.position, mousePos, moveSpeed * Time.deltaTime);
        }



    }
}
