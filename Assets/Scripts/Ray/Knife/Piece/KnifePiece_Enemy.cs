using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public class KnifePiece_Enemy : KnifePiece_Living
    {
        [System.Serializable]
        public class PreferenceProperties
        {
            [SerializeField]
            KnifeMovePreference movePreference;

            [SerializeField]
            int influence;

            public KnifeMovePreference MovePreference { get => movePreference; }
            public int Influence { get => influence; }
        }

        public class PrefferedTile
        {
            TileCache tile;
            int influence;

            public PrefferedTile(TileCache tile,  int influence)
            {
                this.tile = tile;
                this.influence = influence;
            }

            public TileCache Tile { get => tile; }
            public int Influence { get => influence; }

            public void AddInfluence(int influence)
            {
                this.influence += influence;
            }
        }

        [SerializeField]
        List<PreferenceProperties> movePreferences = new();

        [Header("Enemy: SFX")]
        [SerializeField]
        AudioSourceRandom bloodAudioSource;

        [SerializeField]
        AudioSourceRandom hurtAudioSource;

        [SerializeField]
        string sfxDeadName = "sfxDead";

        public override void WhenWaitingForAct()
        {
            base.WhenWaitingForAct();
            var pieceCache = levelManager.GetPiece(this);
            var validMoves = moveRule.GetValidMoves(pieceCache, levelManager.Pieces, levelManager.LevelProperties);
            if (validMoves.Count > 0)
            {
                var validTiles = new List<TileCache>();
                var prefferedTiles = new List<PrefferedTile>();
                
                foreach (var validMove in validMoves)
                {
                    var validTile = levelManager.GetTile(validMove);
                    validTiles.Add(validTile);
                    prefferedTiles.Add(new PrefferedTile(validTile, 0));
                }

                foreach (var preference in movePreferences)
                    preference.MovePreference.Evaluate(
                        prefferedTiles, 
                        validTiles, 
                        preference.Influence, 
                        pieceCache, 
                        levelManager);
                
                var highestInfluenceTile = prefferedTiles[0];
                for (int i = 1; i < prefferedTiles.Count; i++)
                    if (prefferedTiles[i].Influence > highestInfluenceTile.Influence)
                        highestInfluenceTile = prefferedTiles[i];
                levelManager.TryMovePieceToTile(this, highestInfluenceTile.Tile.Tile);
            }
            else
            {
                Debug.LogWarning("Failed to move to preffered tile");
                SetActState(PieceActingState.PostActing);
            }
        }

        public override void Die(LivingPieceCache otherPiece)
        {
            base.Die(otherPiece);
            bloodAudioSource.PlayOneClipFromPack(sfxDeadName);
            hurtAudioSource.PlayOneClipFromPack(sfxDeadName);
        }
    }
}
