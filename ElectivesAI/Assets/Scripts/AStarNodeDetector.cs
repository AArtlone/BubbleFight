using System.Collections.Generic;
using UnityEngine;

public class AStarNodeDetector : MonoBehaviour
{
    private Tank _tankInterface;
    private AStarPath _AStarPath;

    public AStarNode CurrentNode;
    public List<AStarNode> PathOfTank = new List<AStarNode>();
    public int CurrentNodeIndexInPath = 1;
    private bool _isInPath = false;
    private VisionCone _eyes;

    private string _lastLetter;

    private void Start()
    {
        _tankInterface = transform.parent.GetComponentInChildren<Tank>();
        _AStarPath = GetComponent<AStarPath>();
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _lastLetter = transform.parent.name.Substring(transform.parent.name.Length - 1);
    }

    private void Update()
    {
        transform.position = _tankInterface.transform.position;
        if (PathOfTank != null)
        {
            foreach (var node in PathOfTank)
            {
                if (node != null && node.pathParent != null)
                {
                    //Debug.DrawLine(node.transform.position, node.pathParent.transform.position, Color.red);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Node" + _lastLetter)
        {
            CurrentNode = other.gameObject.GetComponent<AStarNode>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Node" + _lastLetter && _isInPath == false)
        {
            //Debug.Log("Entered " + _currentNodeIndexInPath);
            CurrentNodeIndexInPath = 1;
            CurrentNode = other.gameObject.GetComponent<AStarNode>();
            _AStarPath.startNode = CurrentNode;

            _isInPath = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Node" + _lastLetter && _isInPath && PathOfTank.Contains(other.gameObject.GetComponent<AStarNode>()))
        {
            CurrentNodeIndexInPath++;
        }
    }
}
