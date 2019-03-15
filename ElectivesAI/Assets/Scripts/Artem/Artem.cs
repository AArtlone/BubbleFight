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
    public Renderer ground; // reference to the plane
    public GameObject[] _array;
    private bool _hasANodeToMoveTo;

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
        GameObject randomNode = _array[Random.Range(0, _array.Length - 1)];
    }

    private void MoveToRandomPlace()
    {
        _aStarNodeDetector.PathOfTank = null;
        GameObject randomNode = _array[Random.Range(0, _array.Length - 1)];
        if(randomNode)
        {
            _aStarPath.startNode = _aStarNodeDetector.CurrentNode;
            _aStarPath.endNode = randomNode.GetComponent<AStarNode>();
            _aStarNodeDetector.PathOfTank = _aStarPath.FindShortestPath();
            _hasANodeToMoveTo = true;
        }
    }

    private void ChooseRandomPlayStyle()
    {
        int styleToUse = Random.Range(1, 3);
        if (styleToUse == 1)
        {
            _currentPlayStyle = PlayStyle.aggresive;
        }
        else if (styleToUse == 2)
        {
            _currentPlayStyle = PlayStyle.passive;
        }
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
        if(_hasANodeToMoveTo && _aStarNodeDetector.CurrentNodeIndexInPath < _aStarNodeDetector.PathOfTank.Count)
        {
            _tankInterface.MoveTheTank("Forward");

            _tankInterface.RotateTheTank(_aStarNodeDetector.PathOfTank
                [_aStarNodeDetector.PathOfTank.Count - 1 - _aStarNodeDetector.CurrentNodeIndexInPath]
                .transform);
        }
        if(_eyes.Target)
        {
            Debug.Log(_eyes.Target.name);
        }
        if (_eyes.Obstacle)
        {
            Debug.Log(_eyes.Obstacle.name);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            MoveToRandomPlace();
        }
        if (_currentPlayStyle == PlayStyle.aggresive)
        {
            
        } else if(_currentPlayStyle == PlayStyle.passive)
        {
        }
    }

    private void Update()
    {
        //SensorsCheck();
        MovingTank();
    }

    private void FixedUpdate()
    {

    }





    private void SensorsCheck()
    {
        //Debug.DrawLine(_rotationalObject.transform.position, transform.rotation., Color.blue);
        //_rotationalObject.transform.Rotate(0f, .4f, 0f);
        //int RaysToShoot = 80;

        //float angle = 0;
        //for (int i = 0; i < RaysToShoot; i++)
        //{
        //    float x = Mathf.Sin(angle);
        //    float z = Mathf.Cos(angle);
        //    angle += 2 * Mathf.PI / RaysToShoot;

        //    Vector3 dir = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        //    RaycastHit hit;
        //    Debug.DrawLine(transform.position, dir, Color.red);
        //    if (Physics.Raycast(transform.position, dir, out hit))
        //    {
        //        if(hit.collider.gameObject.tag == "Bullet")
        //        {
        //            //Dodge
        //        }
        //    }
        //}
    }
}
