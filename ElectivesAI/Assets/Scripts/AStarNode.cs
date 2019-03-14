using System.Collections.Generic;
using UnityEngine;

public class AStarNode : MonoBehaviour
{
    public float g = float.PositiveInfinity;
    public float h = float.PositiveInfinity;
    public float f = float.PositiveInfinity;
    public bool unWalkable = true;

    public List<AStarNode> connections = new List<AStarNode>();
    public AStarNode pathParent = null;

    void OnDrawGizmosSelected()
    {
        // Draws a yellow line from this transform to the connections
        Gizmos.color = Color.yellow;
        foreach (var connection in connections) {
            if (connection != null) {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}
