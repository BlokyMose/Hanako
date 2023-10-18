using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hanako
{
    public class AlphaChange : MonoBehaviour
    {
        public GameObject[] objectsToFade;
        public float fadeDuration = 1.0f;
        public float delayBetweenObjects = 1.0f;
        private int currentIndex = 0;
        private bool fading = false;
        private int score = 0; // �X�R�A���Ǘ�
        public TMP_Text scoreText; // TextMeshPro�̃e�L�X�g�I�u�W�F�N�g���֘A�t����

        private void Start()
        {
            // ���߂ɂ��ׂẴI�u�W�F�N�g���\���ɂ���
            foreach (var obj in objectsToFade)
            {
                Color currentColor = obj.GetComponent<Renderer>().material.color;
                currentColor.a = 0;
                obj.GetComponent<Renderer>().material.color = currentColor;
            }

            // �ŏ��̃I�u�W�F�N�g���t�F�[�h�C���J�n
            StartCoroutine(FadeInObject(currentIndex));
        }

        IEnumerator FadeInObject(int index)
        {
            if (index >= objectsToFade.Length)
            {
                yield break;
            }

            fading = true;
            Color startColor = objectsToFade[index].GetComponent<Renderer>().material.color;
            Color endColor = startColor;
            endColor.a = 1;

            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                objectsToFade[index].GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectsToFade[index].GetComponent<Renderer>().material.color = endColor;

            // �X�R�A�𑝂₵�ATextMeshPro�̃e�L�X�g���X�V
            score++;
            if (scoreText != null)
            {
                scoreText.text = "�X�R�A: " + score.ToString();
            }

            yield return new WaitForSeconds(delayBetweenObjects);

            // ���̃I�u�W�F�N�g���t�F�[�h�C��
            currentIndex++;
            if (currentIndex < objectsToFade.Length)
            {
                StartCoroutine(FadeInObject(currentIndex));
            }
            else
            {
                fading = false;
            }
        }

        private void Update()
        {
            // �t�F�[�h���I�������牽�����ʂȃA�N�V���������s�ł��܂�
            if (!fading)
            {
                // �����ɔC�ӂ̃A�N�V������ǉ�
            }
        }
    }
}
