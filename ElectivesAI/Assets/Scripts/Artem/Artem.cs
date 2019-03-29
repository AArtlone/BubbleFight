using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlayStyle
{
    aggresive,
    passive
}

public enum TankState
{
    wandering,
    chasing,
    runningAway,
    dodging
}

public class Artem : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank _tankInterface;
    private VisionCone _eyes;
    private AStarPath _aStarPath;
    private AStarNodeDetector _aStarNodeDetector;
    private GameObject _nodeGrid;
    private PlayStyle _currentPlayStyle;
    private TankState _currentTankState;
    public GameObject[] _array;
    public LayerMask NodeMask;
    private GameObject _randomNode;
    private bool _hasANodeToMoveTo;
    private bool _targetToChase;
    private bool _isWallInBetween = false;
    private InterfaceManager _interfaceManager;

    private void Start()
    {
        _tankInterface = GetComponent<Tank>();
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _aStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        _aStarNodeDetector = transform.parent.GetComponentInChildren
            <AStarNodeDetector>();
        _nodeGrid = GameObject.Find("NodeListA");
        _array = new GameObject[26 * 26];
        _interfaceManager = GetComponentInChildren<InterfaceManager>();

        ChooseRandomPlayStyle();
        CreateNodeArray();
        //Invoke("MoveToRandomPlace", 2f);
    }
    
    private void CreateNodeArray()
    {
        int i = 0;
        foreach (Transform node in _nodeGrid.GetComponentInChildren<Transform>())
        {
            if(node.GetComponent<AStarNode>().unWalkable == false)
            {
                _array[i] = node.gameObject;
                i++;
            }
        }
    }

    private void ChooseRandomNode()
    {
        _aStarNodeDetector.PathOfTank = null;
        _randomNode = null;
        _randomNode = _array[Random.Range(0, _array.Length - 1)];

        bool isThereAWalkableNode = false;
        for (int i = 0; i < _array.Length; i++) {
            if (_array[i].GetComponent<AStarNode>().unWalkable == false) {
                isThereAWalkableNode = true;
                break;
            }
        }

        if (isThereAWalkableNode) {
            while (_randomNode.GetComponent<AStarNode>().unWalkable == true)
            { 
                _randomNode = _array[Random.Range(0, _array.Length - 1)];
            }
        }
        
        if (_randomNode.GetComponent<AStarNode>().unWalkable == false)
        {
            if (_randomNode)
            {
                _aStarPath.startNode = _aStarNodeDetector.CurrentNode;
                _aStarPath.endNode = _randomNode.GetComponent<AStarNode>();
                _aStarNodeDetector.PathOfTank = _aStarPath.FindShortestPath();
                _aStarNodeDetector.CurrentNodeIndexInPath = 1;
                _hasANodeToMoveTo = true;
                _currentTankState = TankState.wandering;
            }
        }
    }

    private void GoToTheNode()
    {
        if(_hasANodeToMoveTo == false)
        {
            Invoke("ChooseRandomNode", .1f);
        }
    }

    private void ChooseRandomPlayStyle()
    {
        _currentPlayStyle = PlayStyle.aggresive;
        //int styleToUse = Random.Range(1, 3);
        //if (styleToUse == 1)
        //{
        //    _currentPlayStyle = PlayStyle.aggresive;
        //}
        //else if (styleToUse == 2)
        //{
        //    _currentPlayStyle = PlayStyle.passive;
        //}
    }

    private void SwitchPlayStyle(int playStyleToUse)
    {
        if(playStyleToUse == 1)
        {
            _currentPlayStyle = PlayStyle.aggresive;
        } else if(playStyleToUse == 2)
        {
            _currentPlayStyle = PlayStyle.passive;
        }
    }

    private void MovingTank()
    {
        if(_currentTankState == TankState.wandering &&
            _aStarNodeDetector.CurrentNode != _aStarPath.endNode)
        {
            _tankInterface.RotateTheTank(_aStarNodeDetector.PathOfTank
                [_aStarNodeDetector.PathOfTank.Count - 1 - _aStarNodeDetector.CurrentNodeIndexInPath]
                .transform);

            Invoke("MoveTankDelay", 1f);
        }
        else if (_aStarNodeDetector.CurrentNode == _aStarPath.endNode)
        {
            _hasANodeToMoveTo = false;
            GoToTheNode();
        }
        if (_currentTankState == TankState.chasing)
        {
            if(_eyes.Target != null && !_isWallInBetween) {
                float distance = (_eyes.Target.transform.position - transform.position).magnitude;
                if (distance > (_tankInterface.GetFireRange() / 2)) {
                    _tankInterface.RotateTheTank(_eyes.Target.transform);
                    _tankInterface.MoveTheTank("Forward");
                }
                if (distance <= _tankInterface.GetFireRange())
                {
                    _tankInterface.RotateTheTank(_eyes.Target.transform);
                    _tankInterface.Shoot();
                }
            }
        }
        if (_currentTankState == TankState.runningAway)
        {
            _tankInterface.RotateTheTank(_eyes.Target.transform);
            _tankInterface.MoveTheTank("Backward");
            float distance = (_eyes.Target.transform.position - transform.position).magnitude;
            if (distance <= _tankInterface.GetFireRange())
            {
                _tankInterface.Shoot();
            }
        }
    }

    private void MoveTankDelay()
    {
        _tankInterface.MoveTheTank("Forward");
    }

    private void SensorsCheck()
    {
        if (_eyes.Target == null)
        {
            GoToTheNode();
        }
        else if (_eyes.Target != null)
        {
            _hasANodeToMoveTo = false;
            if (_eyes.Target.tag == "Tank")
            {
                if (_currentPlayStyle == PlayStyle.aggresive)
                {
                    _currentTankState = TankState.chasing;
                }
                if (_currentPlayStyle == PlayStyle.passive)
                {
                    _currentTankState = TankState.runningAway;
                }
            }
        }
    }

    private void Update()
    {
        SensorsCheck();
        _interfaceManager.UpdateStateDisplay(_currentTankState.ToString());
    }

    private void FixedUpdate()
    {
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
                    Debug.DrawLine(transform.position, targetPosition, Color.cyan);
                    _isWallInBetween = true;
                    _eyes.Target = null;
                }
            }
        }
        MovingTank();
    }
}
