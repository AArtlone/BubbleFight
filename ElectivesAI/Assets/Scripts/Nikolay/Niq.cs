using UnityEngine;

public class Niq : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank _tankInterface;
    private VisionCone _eyes;
    private AStarPath _AStarPath;
    private AStarNodeDetector _nodeDetector;
    private GameObject _nodeGrid;

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
            if (_eyes.Target != null)
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
                    ChaseTarget("Tank");
                } else
                {
                    _tankInterface.RotateTurret(_eyes.Target.transform);

                    _tankInterface.Shoot();
                }
                break;
            case TankState.Wandering:
                _tankInterface.ResetTurretRotation();
                break;
            case TankState.Evasive:
                ChaseTarget("Ammo Pickup");
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
        if (_eyes.Target != null && _eyes.Target.gameObject.tag == tag)
        {
            GameObject node = _nodeGrid.transform.GetChild(0).gameObject;
            float closestNodeDistance =
                Vector3.Distance(
                    _eyes.Target.transform.position,
                    node.transform.position);

            for (int i = 0; i < _nodeGrid.transform.childCount; i++)
            {
                GameObject currentNode = _nodeGrid.transform.GetChild(i).gameObject;

                float currentNodeToEnemyDistance = Vector3.Distance(
                        _eyes.Target.transform.position,
                        currentNode.transform.position);

                if (closestNodeDistance > currentNodeToEnemyDistance)
                {
                    closestNodeDistance = currentNodeToEnemyDistance;
                    node = currentNode;
                }
            }

            _AStarPath.startNode = _nodeDetector.CurrentNode;
            _AStarPath.endNode = node.GetComponent<AStarNode>();
            _nodeDetector.CurrentNodeIndexInPath = 1;
            _nodeDetector.PathOfTank = _AStarPath.FindShortestPath();
        }
    }
}
