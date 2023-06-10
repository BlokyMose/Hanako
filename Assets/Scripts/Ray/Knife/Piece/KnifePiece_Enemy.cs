using Hanako.Knife;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Knife
{
    public class KnifePiece_Enemy : KnifePiece_Living
    {
        [SerializeField]
        KnifeMovePreference movePreference;

        public override void WhenWaitingForDestinationTile()
        {
            base.WhenWaitingForDestinationTile();
            var pieceCache = levelManager.GetPiece(this);
            var validMoves = moveRule.GetValidMoves(pieceCache, levelManager.Pieces, levelManager.LevelProperties);
            if (validMoves != null)
            {
                var preferredTile = movePreference.GetPrefferedTile(validMoves, pieceCache,levelManager.Pieces,levelManager.LevelProperties,levelManager.Tiles);
                if (preferredTile != null)
                {
                    destinationTile = preferredTile;
                    MoveToTile(destinationTile);
                }
                else
                {

                }
            }
            else
            {

            }
        }


    }
}
