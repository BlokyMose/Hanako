using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;
using static Hanako.Knife.KnifeLevel;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    [CreateAssetMenu(fileName = "KnifeMovePref_ToInteraction", menuName = "SO/Knife/Move Preference/To Interaction")]

    public class KnifeMovePreference_ToInteraction : KnifeMovePreference
    {
        [SerializeField]
        KnifeInteraction targetInteraction;

        public override void Evaluate(
            List<KnifePiece_Enemy.PrefferedTile> prefferedTiles, 
            List<TileCache> validTiles, 
            int influence, 
            PieceCache thisPiece, 
            KnifeLevelManager levelManager)
        {
            var allPieces = levelManager.Pieces;
            var allTiles = levelManager.Tiles;
            var startTile = levelManager.GetTile(thisPiece.ColRow);
            var validMoves = new List<ColRow>();
            if (thisPiece.Piece is KnifePiece_Living)
                validMoves = (thisPiece.Piece as KnifePiece_Living).MoveRule.ValidMoves;

            foreach (var piece in allPieces)
            {
                if (piece.Piece.HasInteraction(targetInteraction))
                {
                    var endTile = levelManager.GetTile(piece.ColRow);
                    var shortestPath = GetShortestPath(startTile, endTile, allTiles, validMoves, thisPiece);
                    foreach (var tile in shortestPath)
                    {
                        var prefferedTile = prefferedTiles.Find(x => x.Tile.ColRow.IsEqual(tile.ColRow));
                        if (prefferedTile != null)
                            prefferedTile.AddInfluence(influence);
                    }
                }
            }
        }

        public List<TileCache> GetShortestPath(
            TileCache startTile, 
            TileCache endTile, 
            TileGrid tileGrid,
            List<ColRow> validMoves,
            PieceCache thisPiece)
        {
            var shortestPath = new List<TileCache>();

            var aStarNodes = new List<List<AStar.Grid.Node>>();
            foreach (var row in tileGrid.Tiles)
            {
                var rowList = new List<AStar.Grid.Node>();
                aStarNodes.Add(rowList);
                foreach (var tile in row)
                {
                    var x = tile.ColRow.col;
                    var y = tile.ColRow.row;
                    var isObstacle = false;
                    if (tile.Tile.TryGetPiece(out var tilePiece))
                    {
                        if (!tilePiece.IsInteractable)
                            isObstacle = true;
                        else if (!tilePiece.CheckInteractabilityAgainst(thisPiece))
                            isObstacle = true;
                    } 

                    rowList.Add(new(x, y, isObstacle));
                }
            }

            var aStarStartPos = new AStar.Pos(startTile.ColRow.col, startTile.ColRow.row);
            var aStarEndPos = new AStar.Pos(endTile.ColRow.col, endTile.ColRow.row);
            var aStarValidMoves = new List<AStar.Pos>();
            foreach (var move in validMoves)
                aStarValidMoves.Add(new(move.col, move.row));

            var shortestPathAStar = AStar.Grid.GetShortestPath(aStarNodes, aStarStartPos, aStarEndPos, aStarValidMoves);
            foreach (var node in shortestPathAStar)
            {
                var foundTile = tileGrid.GetTile(node.Pos.x, node.Pos.y);
                if (foundTile != null)
                {
                    shortestPath.Add(foundTile);
                }
            }

            return shortestPath;
        }


    }
}
