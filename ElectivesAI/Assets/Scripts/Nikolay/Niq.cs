using System.Collections;
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

    // Depending on the player's stats and circumstances, different states will
    // be toggled triggering different behaviour.
    private enum TankState { Wandering, Combat, Evasive }
    private TankState _currentState;

    private void Start()
    {
        _nodeGrid = GameObject.Find("NodeListN");
        _tankInterface = GetComponent<Tank>();
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _AStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        _nodeDetector = transform.parent.GetComponentInChildren<AStarNodeDetector>();
    }

    private void Update()
    {
        Debug.Log(_eyes.Target);
        if (_tankInterface.GetHealth() < 10 ||
            _tankInterface.GetAmmo() <= 0)
        {
            // When the player is out of ammo, he will be in the evasive state 
            // scouting for different objectives at the same time and trying to avoid
            // enemies
            _currentState = TankState.Evasive;
        }
        
        if (_currentState != TankState.Evasive)
        {
            if (_eyes.Target.tag == "Tank")
            {
                _currentState = TankState.Combat;
            }
            else
            {
                _currentState = TankState.Wandering;
            }
        }

        /*
        Debug.Log(_tankInterface.GetHealth());
        Debug.Log(_currentState);
        Debug.Log(_eyes.Target);
        */
        switch (_currentState)
        {
            case TankState.Combat:
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
                    _tankInterface.RotateTurret(_eyes.Target.transform);

                    _tankInterface.Shoot();
                }
                break;
            case TankState.Wandering:
                //_tankInterface.ResetTurretRotation();
                break;
            case TankState.Evasive:
                if (_lookForNewTarget)
                {
                    ChaseTarget("Ammo Pickup");
                    Invoke("EnableLookingForTarget", 0.2f);
                    _lookForNewTarget = false;
                }
                if (_eyes.Target != null)
                {
                    _tankInterface.RotateTurret(_eyes.Target.transform);
                }
                break;
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

    private void EnableLookingForTarget()
    {
        _lookForNewTarget = true;
    }
}
