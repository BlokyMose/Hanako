using System;
using System.Collections.Generic;

namespace AStar
{
    public enum DistanceCalculation
    {
        Manhattan,
        Euclidean
    }

    public struct Pos
    {
        public int x;
        public int y;

        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns Manhattan distance <code>?x2?x1?+?y2?y1? </code> 
        /// </summary>
        public static double Distance(Pos startPos, Pos endPos, DistanceCalculation distanceCalculation = DistanceCalculation.Manhattan)
        {
            return distanceCalculation switch
            {
                DistanceCalculation.Manhattan => 
                    Math.Abs(endPos.x - startPos.x) + Math.Abs(endPos.y - startPos.y),
                DistanceCalculation.Euclidean =>
                    Math.Sqrt(Math.Pow(endPos.x - startPos.x, 2) + Math.Pow(endPos.y - startPos.y, 2)),
                _ => 0,
            };
        }

        public static Pos Add(Pos a, Pos b) => new(a.x + b.x, a.y + b.y);

        public static bool Equal(Pos a, Pos b) => a.x == b.x && a.y == b.y;
    }

    public static class Grid
    {
        public class Node
        {
            public Pos Pos { get; private set; }
            public bool IsObstacle { get; private set; }
            public bool IsExplored { get; set; }
            public Node Parent { get; set; }
            public int DistanceToEnd { get; set; } = int.MaxValue;
            public int DistanceToStart { get; set; } = int.MaxValue;
            public int Cost => DistanceToStart + DistanceToEnd;

            public Node(int x, int y, bool isObstacle)
            {
                Pos = new(x, y);
                IsObstacle = isObstacle;
                IsExplored = isObstacle;
            }

            public void UpdateDistanceToStart(Pos startPos, Node parent)
            {
                var distanceToStart = (int)Pos.Distance(startPos, Pos);
                if (DistanceToStart > distanceToStart)
                {
                    DistanceToStart = distanceToStart;
                    Parent = parent;
                }
            }

            public void UpdateDistanceToEnd(Pos endPos, Node parent)
            {
                var distanceToEnd = (int)Pos.Distance(endPos, Pos);
                if (DistanceToEnd > distanceToEnd)
                {
                    DistanceToEnd = distanceToEnd;
                    Parent = parent;
                }
            }
        }

        public static List<Node> GetShortestPath(
            List<List<Node>> nodes, 
            Pos startPos, 
            Pos endPos,
            List<Pos> validMoves)
        {
            var lastNode = Pathfind(nodes, startPos, endPos, validMoves);
            var shortestPath = CreateListFromStartToLastNode(lastNode, startPos);
            return shortestPath;

            #region [Methods]

            // Explore moveable nodes with lowest cost while updating its cost; return last node closest to endPos
            static Node Pathfind(List<List<Node>> nodes, Pos startPos, Pos endPos, List<Pos> validMoves)
            {
                Node currentNode = null;
                var nodesToExplore = new HashSet<Node>() { nodes[startPos.y][startPos.x] };
                while (nodesToExplore.Count > 0)
                {
                    currentNode = GetNodeWithLowestCostFrom(nodesToExplore);
                    nodesToExplore.Remove(currentNode);
                    currentNode.IsExplored = true;

                    var moveableNodes = GetMoveableNodes(nodes, currentNode.Pos, validMoves);
                    foreach (var moveableNode in moveableNodes)
                    {
                        if (moveableNode.IsObstacle)
                            continue;

                        else if (!moveableNode.IsExplored)
                            nodesToExplore.Add(moveableNode);

                        moveableNode.UpdateDistanceToStart(startPos, currentNode);
                        moveableNode.UpdateDistanceToEnd(endPos, currentNode);

                        if (Pos.Equal(moveableNode.Pos, endPos))
                        {
                            moveableNode.Parent = currentNode;
                            return moveableNode;
                        }
                    }

                    if (nodesToExplore.Count == 0)
                    {
                        Node closestNode = currentNode;
                        var closestDistanceToEnd = Pos.Distance(closestNode.Pos, endPos);
                        foreach (var row in nodes)
                        {
                            foreach (var node in row)
                            {
                                var distanceToEnd = Pos.Distance(node.Pos, endPos);
                                if (node.IsExplored && !node.IsObstacle && distanceToEnd < closestDistanceToEnd)
                                {
                                    closestNode = node;
                                    closestDistanceToEnd = distanceToEnd;
                                }
                            }
                        }
                        return closestNode;
                    }
                }
                return currentNode;
            }

            // Create list of nodes, ordered by parent relationship
            static List<Node> CreateListFromStartToLastNode(Node lastNode, Pos startPos)
            {
                var shortestPath = new List<Node>();
                Node currentNode = lastNode;
                while (currentNode != null)
                {
                    shortestPath.Add(currentNode);

                    if (!Pos.Equal(currentNode.Pos, startPos))
                        currentNode = currentNode.Parent;
                    else
                        break;
                }
                shortestPath.Reverse();
                return shortestPath;
            }

            static bool TryGetNodeAt(List<List<Node>> nodes, Pos pos, out Node foundNode)
            {
                if (pos.y < 0 || nodes.Count - 1 < pos.y ||
                    pos.x < 0 || nodes[pos.y].Count - 1 < pos.x)
                {
                    foundNode = null;
                    return false;
                }

                foundNode = nodes[pos.y][pos.x];
                return true;
            }

            static List<Node> GetMoveableNodes(List<List<Node>> nodes, Pos pos, List<Pos> validMoves)
            {
                var validNodes = new List<Node>();
                foreach (var move in validMoves)
                    if (TryGetNodeAt(nodes, Pos.Add(pos, move), out var foundNode))
                        validNodes.Add(foundNode);

                return validNodes;
            }
    
            static int GetNodesCount(List<List<Node>> nodes)
            {
                int count = 0;
                foreach (var row in nodes)
                    foreach (var col in row)
                        count++;
                return count;
            }

            static Node GetNodeWithLowestCostFrom(HashSet<Node> nodes)
            {
                if (nodes == null || nodes.Count == 0) 
                    return null;
            
                Node lowestCostNode = null;
                foreach (var node in nodes)
                {
                    if (lowestCostNode == null)
                        lowestCostNode = node;
                    else if (lowestCostNode.Cost > node.Cost)
                        lowestCostNode = node;
                    else if (lowestCostNode.Cost == node.Cost && lowestCostNode.DistanceToEnd > node.DistanceToEnd)
                        lowestCostNode = node;
                }

                return lowestCostNode;
            }

            #endregion
        }

    }
}

