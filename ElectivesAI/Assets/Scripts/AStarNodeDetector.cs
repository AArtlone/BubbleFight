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
        foreach (var node in PathOfTank)
        {
            if (node != null && node.pathParent != null)
            {
                Debug.DrawLine(node.transform.position, node.pathParent.transform.position, Color.red);
            }
        }
    }

    private void FixedUpdate()
    {
        // We check if the tank is close enough to its target and if so, we 
        // stop moving and target to fire, otherwise we continue chasing it.
        if (_eyes.Target != null)
        {
            if (_eyes.Target.tag == "Ammo Pickup" && CurrentNodeIndexInPath < PathOfTank.Count)
            {
                _tankInterface.MoveTheTank("Forward");

                _tankInterface.RotateTheTank(PathOfTank[PathOfTank.Count - 1 - CurrentNodeIndexInPath].transform);
            }

            if (_eyes.Target.tag == "Tank")
            {
                if (_eyes.DistanceToTarget > _tankInterface.GetFireRange() && CurrentNodeIndexInPath < PathOfTank.Count)
                {
                    _tankInterface.MoveTheTank("Forward");

                    _tankInterface.RotateTheTank(PathOfTank[PathOfTank.Count - 1 - CurrentNodeIndexInPath].transform);
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
