using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class AStar_TestManager : MonoBehaviour
    {
        public List<AStar_Node> nodes = new List<AStar_Node>();
        public int colCount = 8;
        public int rowCount = 6;

        [Button]
        public void Test()
        {
            var startNode = nodes[0];
            var endNode = nodes[0];

            foreach (var node in nodes)
            {
                if (node.isStart)
                    startNode = node;
                else if (node.isEnd)
                    endNode = node;
            }

            var aStarNodes = new List<List<AStar.Grid.Node>>();
            for (int row = 0; row < rowCount; row++)
            {
                var rowNodes = new List<AStar.Grid.Node>();
                aStarNodes.Add(rowNodes);
                for (int col = 0; col < colCount; col++)
                {
                    var foundNode = nodes.Find(x => x.x == col && x.y == row);
                    rowNodes.Add(new AStar.Grid.Node(foundNode.x, foundNode.y, foundNode.isObstacle));
                }
            }

            var shortestPath = AStar.Grid.GetShortestPath(aStarNodes, new(startNode.x, startNode.y), new(endNode.x, endNode.y), new()
            {
                new (-1,0),
                new (0,1),
                new (1,0),
                new (0,-1),
            });

            //foreach (var node in nodesExplored)
            //{
            //    var foundNode = nodes.Find(x => x.x == node.Pos.x && x.y == node.Pos.y);
            //    if (foundNode == null)
            //    {
            //        Debug.LogWarning("WHYY");
            //        continue;
            //    }

            //    if (!foundNode.isStart && !foundNode.isEnd)
            //        foundNode.Set(Color.magenta, node.Cost, node.DistanceToStart, node.DistanceToEnd);
            //}

            foreach (var node in shortestPath)
            {
                var foundNode = nodes.Find(x => x.x == node.Pos.x && x.y == node.Pos.y);
                if (foundNode == null)
                    Debug.LogWarning("WHYY");
                foundNode.Set(Color.blue,node.Cost,node.DistanceToStart,node.DistanceToEnd);
            }
        }
    }
}
