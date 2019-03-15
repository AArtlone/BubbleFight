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
    movingTowardsANode,
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
    private bool _hasANodeToMoveTo;
    private bool _targetToChase;

    private void Start()
    {
        _tankInterface = GetComponent<Tank>();
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _aStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        _aStarNodeDetector = transform.parent.GetComponentInChildren
            <AStarNodeDetector>();
        _nodeGrid = GameObject.Find("NodeListA");
        _array = new GameObject[26 * 26];

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
        GameObject randomNode = _array[Random.Range(0, _array.Length - 1)];
        if(randomNode)
        {
            _aStarPath.startNode = _aStarNodeDetector.CurrentNode;
            _aStarPath.endNode = randomNode.GetComponent<AStarNode>();
            _aStarNodeDetector.PathOfTank = _aStarPath.FindShortestPath();
            _aStarNodeDetector.CurrentNodeIndexInPath = 1;
            _hasANodeToMoveTo = true;
            _currentTankState = TankState.movingTowardsANode;
        }
    }

    private void GoToTheNode()
    {
        if(_hasANodeToMoveTo == false)
        {
            ChooseRandomNode();
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
        if(_currentTankState == TankState.movingTowardsANode && 
            _aStarNodeDetector.CurrentNodeIndexInPath < _aStarNodeDetector.PathOfTank.Count)
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
            _tankInterface.RotateTheTank(_eyes.Target.transform);
            _tankInterface.MoveTheTank("Forward");
            float distance = (_eyes.Target.transform.position - transform.position).magnitude;
            if (distance <= _tankInterface.GetFireRange())
            {
                _tankInterface.Shoot();
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
    }

    private void FixedUpdate()
    {
        MovingTank();
    }
}
