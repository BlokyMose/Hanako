using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                this.influence = influence;
            }
        }

        [SerializeField]
        List<PreferenceProperties> movePreferences = new();

        public override void WhenWaitingForAct()
        {
            base.WhenWaitingForAct();
            var pieceCache = levelManager.GetPiece(this);
            var validMoves = moveRule.GetValidMoves(pieceCache, levelManager.Pieces, levelManager.LevelProperties);
            if (validMoves.Count > 0)
            {
                var prefferedTiles = new List<PrefferedTile>();
                foreach (var validMove in validMoves)
                    prefferedTiles.Add(new PrefferedTile(levelManager.GetTile(validMove), 0));

                foreach (var preference in movePreferences)
                    preference.MovePreference.Evaluate(prefferedTiles, preference.Influence, pieceCache, levelManager.Pieces, levelManager.LevelProperties);
                
                PrefferedTile highestInfluenceTile = prefferedTiles[0];
                for (int i = 1; i < prefferedTiles.Count; i++)
                    if (prefferedTiles[i].Influence > highestInfluenceTile.Influence)
                        highestInfluenceTile = prefferedTiles[i];
                levelManager.TryMovePieceToTile(this, highestInfluenceTile.Tile.Tile);

                //var preferredTile = movePreferences[0].MovePreference.GetPrefferedTile(validMoves, pieceCache, levelManager.Pieces, levelManager.LevelProperties, levelManager.Tiles);
                //levelManager.TryMovePieceToTile(this, preferredTile);
            }
            else
            {
                // Piece failed to move
                Debug.LogWarning("Failed to move to preffered tile");
                SetActState(PieceActingState.PostActing);
            }
        }


    }
}
