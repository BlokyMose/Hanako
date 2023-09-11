using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Hanako.Dialogue.DialogueData;
using static Hanako.Dialogue.DialogueEnums;

namespace Hanako.Dialogue
{
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SortingGroup))]
    public class DialogueCharParent : MonoBehaviour
    {
        [Header("Animation Layers")]
        [SerializeField]
        string faceLayerName = "dialogue_face";

        [SerializeField]
        string bodyLayerName = "dialogue_body";

        [Header("Components")]
        [SerializeField]
        DialogueBubble bubble_R;

        [SerializeField]
        DialogueBubble bubble_L;

        Animator animator, charAnimator;
        DialogueBubble currentBubble;
        SortingGroup sortingGroup;
        int int_mode, int_d_face, int_d_body;

        void Awake()
        {
            sortingGroup = GetComponent<SortingGroup>();
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            int_d_face = Animator.StringToHash(nameof(int_d_face));
            int_d_body = Animator.StringToHash(nameof(int_d_body));
            bubble_L.gameObject.SetActive(false);
            bubble_R.gameObject.SetActive(false);
        }

        public void Init(CharID charID, float scale, bool isFacingRight)
        {
            var charGO = Instantiate(charID.Prefab, transform);
            SetLayerAllChildren(charGO.transform, gameObject.layer);
            charGO.layer = gameObject.layer;
            charGO.transform.localScale = new(scale, scale);
            charGO.transform.localEulerAngles = new(0, isFacingRight ? 0 : 180, 0);
            
            charAnimator = charGO.GetComponent<Animator>();
            for (int i = 0; i < charAnimator.layerCount; i++)
                charAnimator.SetLayerWeight(i, 0);
            charAnimator.SetLayerWeight(charAnimator.GetLayerIndex(faceLayerName), 1);
            charAnimator.SetLayerWeight(charAnimator.GetLayerIndex(bodyLayerName), 1);


            currentBubble = isFacingRight ? bubble_R : bubble_L;
            currentBubble.gameObject.SetActive(true);

            animator.SetInteger(int_mode, (int)CharParentAnimation.Idle);
        }

        void SetLayerAllChildren(Transform root, int layer)
        {
            var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
                child.gameObject.layer = layer;
        }

        public void Say(TextLine textLine, DialogueSettings settings, int sortingOrder)
        {
            currentBubble.SetText(textLine, settings);
            sortingGroup.sortingOrder = sortingOrder;
            charAnimator.SetInteger(int_d_face, (int)textLine.FaceAnimation);
        }

        public void ResetAll(int sortingOrder)
        {
            currentBubble.Hide();
            sortingGroup.sortingOrder = sortingOrder;
        }

        public void End()
        {
            currentBubble.Hide();
            animator.SetInteger(int_mode, (int)CharParentAnimation.Hidden);
        }
    }
}
