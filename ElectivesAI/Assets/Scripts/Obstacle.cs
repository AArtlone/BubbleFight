using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "NodeN" ||
            other.transform.tag == "NodeY" ||
            other.transform.tag == "NodeD" ||
            other.transform.tag == "NodeA")
        {
            other.GetComponent<AStarNode>().unWalkable = true;
        }
    }
}
