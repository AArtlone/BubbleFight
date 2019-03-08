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
    private PlayStyle _currentPlayStyle;
    private TankState _currentTanlState;
    public Renderer ground; // reference to the plane
    
    private float gridSize = 2f; //the offset value for each node
    private GameObject[,] _nodesArray;
    private MyAStarNode startNode; // reference to the pathfinding algorithm - beginning node
    private MyAStarNode endNode; // reference to the pathfinding algorithm - end node

    public List<MyAStarNode> shortestPath = new List<MyAStarNode>();

    private void Start()
    {
        _tankInterface = GetComponent<Tank>();
        _nodesArray = new GameObject[10, 10];

        ChooseRandomPlayStyle();
        CreateAStarGrid();
    }

    private void CreateAStarGrid()
    {
        var nodeParent = new GameObject("NodeList");

        var nodeArray = new MyAStarNode[10, 10];
        
        for (int z = 0; z < 10; z++)
        {
            for (int x = 0; x < 10; x++)
            {
                var _nodeObject = new GameObject("Node");
                float _xPos = -ground.bounds.size.x;
                float _zPos = -ground.bounds.size.z;
                Vector3 _nodePos = new Vector3((_xPos / 2) + gridSize + x * 5, 0f, (_zPos / 2) + gridSize + z * 5);
                _nodeObject.transform.position = _nodePos;
                _nodeObject.transform.parent = nodeParent.transform;
                _nodeObject.transform.name = "Node " + x + "/" + z;
                var node = _nodeObject.AddComponent<MyAStarNode>();
                nodeArray[x, z] = node;
                _nodesArray[x, z] = _nodeObject;
            }
        }
        for (int z = 0; z < 10; z++)
        {
            for (int x = 0; x < 10 - 1; x++)
            {
                var currentNode = nodeArray[x, z];
                var nextNode = nodeArray[x + 1, z];

                currentNode.connections.Add(nextNode);
                nextNode.connections.Add(currentNode);
            }
        }
        for (int z = 0; z < 10 - 1; z++)
        {
            for (int x = 0; x < 10; x++)
            {
                var currentNode = nodeArray[x, z];
                var nextNode = nodeArray[x, z + 1];

                currentNode.connections.Add(nextNode);
                nextNode.connections.Add(currentNode);
            }
        }
    }

    private void FindShortestPathBlablabla()
    {
        GameObject _targetNode = _nodesArray[Random.Range(0, 10), Random.Range(0, 10)];
        endNode = _targetNode.AddComponent<MyAStarNode>();
        //transform.LookAt(_targetNode.transform);
        //shortestPath = FindShortestPath();
    }

    List<MyAStarNode> FindShortestPath()
    {
        var openList = new List<MyAStarNode>();
        var closedList = new List<MyAStarNode>();

        startNode.g = 0;
        startNode.f = startNode.g + Vector3.Distance(startNode.transform.position, endNode.transform.position);
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            var currentNode = openList[0];

            if (currentNode == endNode)
            {
                //TODO: return path to caller
                var path = new List<MyAStarNode>();
                while (currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.pathParent;
                }
                path.Add(startNode);
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var connection in currentNode.connections)
            {
                if (closedList.Contains(connection) || connection.unWalkable)
                {
                    continue;
                }

                float g = currentNode.g + Vector3.Distance(currentNode.transform.position, connection.transform.position);
                bool inOpenList = openList.Contains(connection);
                if (!inOpenList || g < connection.g)
                {
                    connection.g = g;
                    connection.f = connection.g + Vector3.Distance(connection.transform.position, endNode.transform.position);
                    connection.pathParent = currentNode;

                    if (!inOpenList)
                    {
                        int index = 0;
                        while (index < openList.Count && openList[index].f < connection.f)
                        {
                            index++;
                        }
                        openList.Insert(index, connection);
                    }
                }
            }
        }
        return null;
    }

    private void SensorsCheck()
    {
        //Debug.DrawLine(_rotationalObject.transform.position, transform.rotation., Color.blue);
        //_rotationalObject.transform.Rotate(0f, .4f, 0f);
        int RaysToShoot = 80;

        float angle = 0;
        for (int i = 0; i < RaysToShoot; i++)
        {
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / RaysToShoot;

            Vector3 dir = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
            RaycastHit hit;
            Debug.DrawLine(transform.position, dir, Color.red);
            if (Physics.Raycast(transform.position, dir, out hit))
            {
                if(hit.collider.gameObject.tag == "Bullet")
                {
                    //Dodge
                }
            }
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            //FindShortestPathBlablabla();
            _tankInterface.MoveTheTank("Forward");
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            float _targetX = Random.Range(-ground.bounds.size.x, ground.bounds.size.x);
            float _targetZ = Random.Range(-ground.bounds.size.z, ground.bounds.size.z);
            Vector3 _targetPos = new Vector3(_targetX, 0f, _targetZ);
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, .5f);
        }
        if (_currentPlayStyle == PlayStyle.aggresive)
        {
            
        } else if(_currentPlayStyle == PlayStyle.passive)
        {
        }
    }

    private void Update()
    {
        SensorsCheck();
        MovingTank();
    }

    private void FixedUpdate()
    {

    }
}
