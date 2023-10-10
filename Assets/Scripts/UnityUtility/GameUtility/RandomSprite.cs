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
            if (sr == null)
                sr = this.GetComponentInFamily<SpriteRenderer>();

            if (!sprites.Contains(sr.sprite)) sprites.Add(sr.sprite);
            SetRandom();
        }

        void SetRandom()
        {
            if (!isActive) return;
            sr.sprite = sprites.GetRandom();
        }
    }
}
