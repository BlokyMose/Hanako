using Encore.Utility;
using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

namespace Hanako.Knife
{
    public class GameInfoCanvas : MonoBehaviour
    {
        [SerializeField]
        Animator ligthingAnimator;

        [Header("Timer")]
        [SerializeField]
        TextMeshProUGUI playTimeText;

        [Header("Targets")]
        [SerializeField]
        Transform targetsParent;

        [SerializeField]
        TargetCheckBox targetCheckBoxPrefab;

        [Header("Rounds")]
        [SerializeField]
        Transform roundsParent;

        [SerializeField]
        RoundCheckBox roundCheckBoxPrefab;

        List<TargetCheckBox> targetCheckBoxes = new();
        List<RoundCheckBox> roundCheckBoxes = new();
        int tri_red;

        void Awake()
        {
            tri_red = Animator.StringToHash(nameof(tri_red));

            var levelManager = FindObjectOfType<KnifeLevelManager>();
            if (levelManager != null)
            {
                levelManager.OnPlayTime += SetPlayTimeText;
                levelManager.OnStartGame += () => { OnStartGame(levelManager); };
                levelManager.OnLivingPieceDied += Eliminate;
                levelManager.OnNextRound += (roundIndex) => { PlayRedAnimation(); CheckRound(); };
            }
        }

        void OnDestroy()
        {
            var levelManager = FindObjectOfType<KnifeLevelManager>();
            if (levelManager != null)
            {
                levelManager.OnPlayTime -= SetPlayTimeText;
                levelManager.OnStartGame -= () => { OnStartGame(levelManager); };
                levelManager.OnLivingPieceDied -= Eliminate;
                levelManager.OnNextRound -= (roundIndex) => { PlayRedAnimation(); CheckRound(); };
            }
        }

        void OnStartGame(KnifeLevelManager levelManager)
        {
            var targetsInfo = new List<KnifePieceInformation>();
            foreach (var livingPiece in levelManager.LivingPieces)
            {
                if (livingPiece.Piece is KnifePiece_Enemy)
                {
                    targetsInfo.Add(livingPiece.Piece.Information);
                }
            }

            Init(levelManager.LevelProperties.RoundCount, targetsInfo);
        }

        void Init(int roundCount, List<KnifePieceInformation> pieces)
        {
            targetCheckBoxes = new();
            targetsParent.DestroyChildren();
            for (int i = 0; i < pieces.Count; i++)
            {
                var checkBox = Instantiate(targetCheckBoxPrefab, targetsParent);
                checkBox.Init(pieces[i]);
                targetCheckBoxes.Add(checkBox);
            }

            roundCheckBoxes = new();
            roundsParent.DestroyChildren();
            for (int i = 0; i < roundCount; i++)
            {
                var checkBox = Instantiate(roundCheckBoxPrefab, roundsParent);
                checkBox.Init();
                roundCheckBoxes.Add(checkBox);
            }
            RefreshCanvas();
        }

        public void SetPlayTimeText(float time)
        {
            playTimeText.text = MathUtility.SecondsToTimeString(time);
        }

        void PlayRedAnimation()
        {
            ligthingAnimator.SetTrigger(tri_red);
        }

        public void RefreshCanvas()
        {
            Canvas.ForceUpdateCanvases();
            var allLayoutGroups = gameObject.GetComponentsInFamily<HorizontalOrVerticalLayoutGroup>();
            foreach (var layoutGroup in allLayoutGroups)
                layoutGroup.enabled = false;
            foreach (var layoutGroup in allLayoutGroups)
                layoutGroup.enabled = true;
        }

        public void Eliminate(KnifePiece_Living livingPiece)
        {
            foreach (var target in targetCheckBoxes)
            {
                if (target.PieceInfo == livingPiece.Information && !target.IsEliminated)
                {
                    target.Eliminated();
                    break;
                }
            }
        }

        public void CheckRound()
        {
            foreach (var round in roundCheckBoxes)
            {
                if (round.IsDotted)
                {
                    round.CancelDot();
                    round.Check();
                    break;
                }
            }            
            
            foreach (var round in roundCheckBoxes)
            {
                if (!round.IsChecked)
                {
                    round.Dot();
                    break;
                }
            }
        }

        public void CheckAllRound()
        {
            foreach (var round in roundCheckBoxes)
            {
                round.Check();
            }
        }
    }
}
