using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hanako.Knife
{
    public class TargetCheckBox : MonoBehaviour
    {
        [SerializeField]
        Image logo;

        [SerializeField]
        Image eliminatedIcon;

        [SerializeField]
        List<Sprite> eliminatedIconVariants = new();

        KnifePieceInformation pieceInfo;

        public KnifePieceInformation PieceInfo { get => pieceInfo; }
        public bool IsEliminated { get => isEliminated; }

        bool isEliminated = false;

        public void Init(KnifePieceInformation pieceInfo, bool randomize = true)
        {
            this.pieceInfo = pieceInfo;
            if (randomize)
            {
                eliminatedIcon.sprite = eliminatedIconVariants.GetRandom();
            }

            eliminatedIcon.gameObject.SetActive(false);
        }

        public void Eliminated()
        {
            eliminatedIcon.gameObject.SetActive(true);
            isEliminated = true;
        }

        public void CancelElimination()
        {
            eliminatedIcon.gameObject.SetActive(false);
            isEliminated = false;
        }
    }
}
