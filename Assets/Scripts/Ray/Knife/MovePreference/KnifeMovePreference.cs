using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifeMovePreference : ScriptableObject
    {
        [SerializeField]
        string preferenceName;

        public abstract ColRow GetPrefferedMove(
            List<ColRow> validMoves, 
            PieceCache thisPiece,
            List<PieceCache> allPieces,
            KnifeLevel levelProperties
            );

        public virtual KnifeTile GetPrefferedTile(
            List<ColRow> validMoves,
            PieceCache thisPiece,
            List<PieceCache> allPieces,
            KnifeLevel levelProperties,
            List<TileCache> allTiles
            )
        {
            if (validMoves.Count == 0)
            {
                Debug.Log("No valid moves");
                foreach (var tile in allTiles)
                    if (tile.ColRow.IsEqual(thisPiece.ColRow))
                        return tile.Tile;
            }
            
            var prefferedMove = GetPrefferedMove(validMoves, thisPiece, allPieces, levelProperties);
            foreach (var tile in allTiles)
            {
                if (tile.ColRow.IsEqual(prefferedMove))
                    return tile.Tile;
            }
            return null;
        }
    }
}
