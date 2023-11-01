using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class RandomHair : MonoBehaviour
    {
        [System.Serializable]
        public class FrontHairProperties
        {
            [SerializeField]
            Sprite sprite;

            [SerializeField]
            bool useHairBack;

            public Sprite Sprite { get => sprite; }
            public bool UseHairBack { get => useHairBack; }
        }

        [SerializeField]
        SpriteRenderer srHairFront;

        [SerializeField]
        SpriteRenderer srHairBack;

        [SerializeField]
        List<FrontHairProperties> frontHairs = new();

        [SerializeField]
        List<Sprite> backHairs = new();

        void Awake()
        {
            if (srHairFront == null) srHairFront = GetComponent<SpriteRenderer>();
            var frontHairProperties = frontHairs.GetRandom();
            srHairFront.sprite = frontHairProperties.Sprite;
            if (frontHairProperties.UseHairBack)
                srHairBack.sprite = backHairs.GetRandom();
            else
                srHairBack.sprite = null;
        }
    }
}
