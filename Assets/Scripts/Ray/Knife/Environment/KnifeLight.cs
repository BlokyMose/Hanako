using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    [RequireComponent(typeof(Animator))]
    public class KnifeLight : MonoBehaviour
    {
        Animator animator;
        int tri_blink;

        void Awake()
        {
            animator = GetComponent<Animator>();
            tri_blink = Animator.StringToHash(nameof(tri_blink));
        }

        void OnEnable()
        {
            var levelManager = FindObjectOfType<KnifeLevelManager>();
            if (levelManager != null)
            {
                levelManager.OnNextRound += OnNextRound;
            }
        }

        void OnDisable()
        {
            var levelManager = FindObjectOfType<KnifeLevelManager>();
            if (levelManager != null)
            {
                levelManager.OnNextRound -= OnNextRound;
            }
        }


        void OnNextRound(int currentRoundIndex)
        {
            animator.SetTrigger(tri_blink);
        }
    }
}
