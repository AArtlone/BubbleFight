using System.Collections.Generic;
using UnityEngine;

public class AStarPath : MonoBehaviour
{
    public AStarNode startNode;
    public AStarNode endNode;

    public List<AStarNode> shortestPath = new List<AStarNode>();

    public List<AStarNode> FindShortestPath()
    {
        var openList = new List<AStarNode>();
        var closedList = new List<AStarNode>();

        startNode.g = 0;
        startNode.f = startNode.g + Vector3.Distance(startNode.transform.position, endNode.transform.position);
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            var currentNode = openList[0];

            // Once the final node is reached, we trace back to the beginning node by adding
            // the child of the current node and so on until we reach the beginning node to form a path
            // for the tank to traverse.
            if (currentNode == endNode)
            {
                var path = new List<AStarNode>();

                while (currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.pathParent;
                }
                path.Add(startNode);
                return path;
            } // otherwise we begin to evaluate all the nodes starting from the current one.

            // Because we already evaluated the start node's parameters at the beginning of the function,
            // we need to move it from the open list to the closed list.
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Once the start/current node is evaluated, we evaluate the parameters of its neighbours/connections
            // to determine if their node's path to the goal is fastest to reach and is if its traversable or not.
            foreach (var connection in currentNode.connections)
            {
                // if the neighbour we are going to evaluate is not traversable or is already evaluated, then we
                // skip it and move on to the next neighbour.
                if (closedList.Contains(connection) || connection.unWalkable)
                {
                    continue;
                }

                // Now that we know if that neighbour is optimal for traversal, we calculate its heuristic and f cost.
                float g = currentNode.g + Vector3.Distance(currentNode.transform.position, connection.transform.position);
                bool inOpenList = openList.Contains(connection);

                // We must also check if the neighbour we are currently evaluating is in the open list already or not, because
                // we dont want to waste performance repeating the same task twice. And if the new path from the neighbour to the
                // end node is shorter than the path from the current node's distance to the end node then we can populate its parameters.
                if (!inOpenList || g < connection.g)
                {
                    // We populate the node parameters of that neighbour/connection based on the new current node and set their
                    // parent to the current node.
                    connection.g = g;
                    connection.f = connection.g + Vector3.Distance(connection.transform.position, endNode.transform.position);
                    connection.pathParent = currentNode;

                    if (!inOpenList)
                    {
                        int index = 0;
                        while (index < openList.Count && openList[index].f < connection.f)
                        {
                            index++;
                        }
                        openList.Insert(index, connection);
                    }
                }
            }
        }

        return null;
    }
}
