using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using Random = UnityEngine.Random;

namespace Hanako
{
    [RequireComponent(typeof(Image))]
    public class LeaderboardGameHeader : MonoBehaviour
    {
        [SerializeField]
        Image iconImage;

        [SerializeField]
        GameObject selectionIcon;

        Image image;
        Action OnClick;

        private void Awake()
        {
            image = GetComponent<Image>();
            iconImage.transform.localEulerAngles = new(0, 0, Random.Range(-2f, 2f));
            image.AddEventTrigger(() => { OnClick?.Invoke(); });
            selectionIcon.SetActive(false);
        }

        public void Init(Sprite icon, Action onClick)
        {
            iconImage.sprite = icon;
            this.OnClick = onClick;
        }


        public void Select()
        {
            selectionIcon.SetActive(true);
        }

        public void Unselect()
        {
            selectionIcon.SetActive(false);
        }
    }
}
