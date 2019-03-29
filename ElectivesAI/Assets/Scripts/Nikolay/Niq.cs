using System.Collections.Generic;
using UnityEngine;

public class Niq : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank _tankInterface;
    private VisionCone _eyes;
    private AStarPath _AStarPath;
    private AStarNodeDetector _nodeDetector;
    private GameObject _nodeGrid;
    private bool _lookForNewTarget = true;
    public LayerMask NodeMask;
    private bool _isWallInBetween = false;
    private bool _isDestinationReached = true;
    private bool _initializeWandering = true;

    // Depending on the player's stats and circumstances, different states will
    // be toggled triggering different behaviour.
    private enum TankState { Wandering, Combat, Evasive }
    private TankState _currentState;
    private InterfaceManager _interfaceManager;

    private void Start()
    {
        _nodeGrid = GameObject.Find("NodeListN");
        _tankInterface = GetComponent<Tank>();
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _AStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        _nodeDetector = transform.parent.GetComponentInChildren<AStarNodeDetector>();
        _interfaceManager = GetComponentInChildren<InterfaceManager>();

        _nodeDetector.CurrentNode = _AStarPath.endNode;
    }

    private void Update()
    {
        // Debug.Log(_isDestinationReached);
        // Debug.Log(_eyes.Target);

        // if (_tankInterface.GetHealth() < 10 ||
        //     _tankInterface.GetAmmo() <= 0)
        // {
        //     // When the player is out of ammo, he will be in the evasive state 
        //     // scouting for different objectives at the same time and trying to avoid
        //     // enemies
        //     _currentState = TankState.Evasive;
        // }

        _interfaceManager.UpdateStateDisplay(_currentState.ToString());

        if (_currentState != TankState.Evasive)
        {
            if (_eyes.Target != null)
            {
                if (_eyes.Target.tag == "Tank")
                {
                    _currentState = TankState.Combat;
                }
            }
            else
            {
                _currentState = TankState.Wandering;
            }
        }
        
        //Debug.Log(_tankInterface.GetHealth());
        //Debug.Log(_eyes.Target);
        //Debug.Log(_currentState);
        switch (_currentState)
        {
            case TankState.Combat:
                if (_eyes.Target != null && _isWallInBetween == false)
                {
                    _tankInterface.RotateTurret(_eyes.Target.transform);
                }
                if (_eyes.DistanceToTarget > _tankInterface.GetFireRange())
                {
                    if (_lookForNewTarget)
                    {
                        ChaseTarget("Tank");
                        Invoke("EnableLookingForTarget", 0.2f);
                        _lookForNewTarget = false;
                    }
                } else
                {
                    if (_isWallInBetween == false)
                    {
                        _tankInterface.RotateTurret(_eyes.Target.transform);

                        Invoke("Shoot", 2f);
                    }
                }
                break;
            case TankState.Wandering:
                if (_isDestinationReached == false)
                {
                    MoveToDestination(_eyes.LastSeenDestination);
                } else
                {
                    if (_nodeDetector.CurrentNode == _AStarPath.endNode || _initializeWandering)
                    {
                        //Debug.Log("New Node");
                        int randomIndexNodeInGrid = Random.Range(0, _nodeGrid.transform.childCount - 1);
                        AStarNode randomNode = _nodeGrid.transform.GetChild(randomIndexNodeInGrid).GetComponent<AStarNode>();

                        while (randomNode.unWalkable == true)
                        {
                            randomIndexNodeInGrid = Random.Range(0, _nodeGrid.transform.childCount - 1);

                            randomNode = _nodeGrid.transform.GetChild(randomIndexNodeInGrid).GetComponent<AStarNode>();
                        }

                        Vector3 randomNodePosition = _nodeGrid.transform.GetChild(randomIndexNodeInGrid).position;

                        MoveToDestination(randomNodePosition);
                        _initializeWandering = false;
                    }
                }
                break;
            case TankState.Evasive: // WORK IN PROGRESS
                if (_lookForNewTarget)
                {
                    ChaseTarget("Ammo Pickup");
                    Invoke("EnableLookingForTarget", 0.2f);
                    _lookForNewTarget = false;
                }
                if (_eyes.Target != null && _isWallInBetween == false)
                {
                    _tankInterface.RotateTurret(_eyes.Target.transform);
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(_eyes.Target);
        // We check if the tank is close enough to its target and if so, we 
        // stop moving and target to fire, otherwise we continue chasing it.
        if (_eyes.Target != null)
        {
            Vector3 targetPosition = _eyes.Target.transform.position;
            Vector3 direction = targetPosition - transform.position;
            float length = direction.magnitude;
            direction.Normalize();

            Ray ray = new Ray(transform.position, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, length);

            _isWallInBetween = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag == "Obstacle")
                {
                    //Debug.DrawLine(transform.position, targetPosition, Color.cyan);
                    _isWallInBetween = true;
                    _eyes.Target = null;
                }
            }

            if (_eyes.Target != null)
            {
                if (_eyes.Target.tag == "Ammo Pickup" && _nodeDetector.CurrentNodeIndexInPath < _nodeDetector.PathOfTank.Count)
                {
                    _tankInterface.MoveTheTank("Forward");

                    _tankInterface.RotateTheTank(_nodeDetector.PathOfTank[_nodeDetector.PathOfTank.Count - 1 - _nodeDetector.CurrentNodeIndexInPath].transform);
                }

                if (_eyes.Target.tag == "Tank" && _isWallInBetween == false)
                {
                    if (_nodeDetector.CurrentNodeIndexInPath < _nodeDetector.PathOfTank.Count)
                    {
                        if (_eyes.DistanceToTarget > _tankInterface.GetFireRange())
                        {
                            _tankInterface.MoveTheTank("Forward");

                            _tankInterface.RotateTheTank(_nodeDetector.PathOfTank[_nodeDetector.PathOfTank.Count - 1 - _nodeDetector.CurrentNodeIndexInPath].transform);
                        }
                        else
                        {
                            if (_isWallInBetween)
                            {
                                _tankInterface.MoveTheTank("Forward");

                                _tankInterface.RotateTheTank(_nodeDetector.PathOfTank[_nodeDetector.PathOfTank.Count - 1 - _nodeDetector.CurrentNodeIndexInPath].transform);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (_nodeDetector.CurrentNode != _AStarPath.endNode)
            {
                _tankInterface.MoveTheTank("Forward");

                _tankInterface.RotateTheTank(_nodeDetector.PathOfTank[_nodeDetector.PathOfTank.Count - 1 - _nodeDetector.CurrentNodeIndexInPath].transform);
            } else
            {
                _isDestinationReached = true;
            }
        }
    }

    private void ChaseTarget(string tag)
    {
        _eyes.TargetToLookFor = tag;
        if (_eyes.Target.gameObject.tag == tag)
        {
            Vector3 targetPosition = _eyes.Target.transform.position;
            Vector3 direction = targetPosition - transform.position;
            float length = direction.magnitude;
            direction.Normalize();

            Ray ray = new Ray(transform.position, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, length, NodeMask);

            GameObject node = hits[hits.Length - 1].transform.gameObject;

            _AStarPath.startNode = _nodeDetector.CurrentNode;
            _AStarPath.endNode = node.GetComponent<AStarNode>();
            _nodeDetector.CurrentNodeIndexInPath = 1;
            _nodeDetector.PathOfTank = _AStarPath.FindShortestPath();
        }
    }

    private void MoveToDestination(Vector3 oldPosition)
    {
        Vector3 targetPosition = oldPosition;
        Vector3 direction = targetPosition - transform.position;
        float length = direction.magnitude;
        direction.Normalize();

        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, length, NodeMask);

        GameObject node = hits[hits.Length - 1].transform.gameObject;

        _AStarPath.startNode = _nodeDetector.CurrentNode;
        _AStarPath.endNode = node.GetComponent<AStarNode>();
        _nodeDetector.CurrentNodeIndexInPath = 1;
        _nodeDetector.PathOfTank = _AStarPath.FindShortestPath();
    }

    private void EnableLookingForTarget()
    {
        _lookForNewTarget = true;
    }

    private void Shoot()
    {
        _tankInterface.Shoot();
    }
}
