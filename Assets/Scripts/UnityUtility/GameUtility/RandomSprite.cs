using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public class RandomSprite : MonoBehaviour
    {
        [SerializeField]
        bool isActive = true;

        [SerializeField]
        SpriteRenderer sr;

        [SerializeField]
        List<Sprite> sprites = new();

        void Awake()
        {
            if (!isActive) return;

            if (sr == null)
                sr = this.GetComponentInFamily<SpriteRenderer>();

            if (!sprites.Contains(sr.sprite)) sprites.Add(sr.sprite);

            sr.sprite = sprites.GetRandom();
        }
    }
}
