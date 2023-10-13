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
        private int score = 0; // スコアを管理
        public TMP_Text scoreText; // TextMeshProのテキストオブジェクトを関連付ける

        private void Start()
        {
            // 初めにすべてのオブジェクトを非表示にする
            foreach (var obj in objectsToFade)
            {
                Color currentColor = obj.GetComponent<Renderer>().material.color;
                currentColor.a = 0;
                obj.GetComponent<Renderer>().material.color = currentColor;
            }

            // 最初のオブジェクトをフェードイン開始
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

            // スコアを増やし、TextMeshProのテキストを更新
            score++;
            if (scoreText != null)
            {
                scoreText.text = "スコア: " + score.ToString();
            }

            yield return new WaitForSeconds(delayBetweenObjects);

            // 次のオブジェクトをフェードイン
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
            // フェードが終了したら何か特別なアクションを実行できます
            if (!fading)
            {
                // ここに任意のアクションを追加
            }
        }
    }
}
