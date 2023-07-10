using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class Player : MonoBehaviour
    {
        // X, Y���W�̈ړ��\�͈�
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
            // �}�E�X�ʒu���X�N���[�����W���烏�[���h���W�ɕϊ�����
            var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // X, Y���W�͈̔͂𐧌�����
            targetPos.x = Mathf.Clamp(targetPos.x, bounds.xMin, bounds.xMax);
            targetPos.y = Mathf.Clamp(targetPos.y, bounds.yMin, bounds.yMax);

            // Z���W���C������
            targetPos.z = 0f;

            // ���̃X�N���v�g���A�^�b�`���ꂽ�Q�[���I�u�W�F�N�g���A�}�E�X�ʒu�ɐ��`��ԂŒǏ]������
            transform.position = Vector3.Lerp(transform.position, targetPos, followStrength);
        }
    }
}
