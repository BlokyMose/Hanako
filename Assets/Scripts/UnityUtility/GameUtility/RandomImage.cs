using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtility
{
    public class RandomImage : MonoBehaviour
    {
        [SerializeField]
        Image image;

        [SerializeField]
        List<Sprite> sprites = new();

        void Awake()
        {
            if (image == null)
                image = GetComponent<Image>();

            if (image.sprite != null && !sprites.Contains(image.sprite))
                sprites.Add(image.sprite);

            image.sprite = sprites.GetRandom();
        }
    }
}
