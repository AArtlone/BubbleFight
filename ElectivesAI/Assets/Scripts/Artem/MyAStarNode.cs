using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAStarNode : MonoBehaviour
{
    public float g = float.PositiveInfinity;
    public float h = float.PositiveInfinity;
    public float f = float.PositiveInfinity;

    public bool unWalkable = false;

    public List<MyAStarNode> connections = new List<MyAStarNode>();
    public MyAStarNode pathParent = null;

    private void Start()
    {
        Invoke("DrawLines", 1f);
    }

    private void DrawLines()
    {
        foreach (var connection in connections)
        {
            if (connection != null)
            {
                Debug.DrawLine(transform.position, connection.transform.position, Color.blue);
            }
        }
    }
}
