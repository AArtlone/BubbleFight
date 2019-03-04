using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlayStyle
{
    aggresive,
    passive
}

public class Artem : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank _tankInterface;

    private PlayStyle _currentPlayStyle;

    public Renderer ground;

    private void Start()
    {
        _tankInterface = GetComponent<Tank>();

        ChooseRandomPlayStyle();
        CreateAStarGrid();
    }

    private void CreateAStarGrid()
    {
        var nodeParent = new GameObject("NodeList");

        var nodeArray = new MyAStarNode[11, 11];
        
        for (int z = 0; z < 11; z++)
        {
            for (int x = 0; x < 11; x++)
            {
                var gameObject = new GameObject("Node");
                float _xPos = -ground.bounds.size.x;
                float _zPos = -ground.bounds.size.z;
                Vector3 _nodePos = new Vector3(_xPos / 2 + x * 5, 0f, _zPos / 2 + z * 5);
                gameObject.transform.position = _nodePos;
                gameObject.transform.parent = nodeParent.transform;
                gameObject.transform.name = "Node " + x + "/" + z;
                var node = gameObject.AddComponent<MyAStarNode>();
                nodeArray[x, z] = node;
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
        if (Input.GetKeyUp(KeyCode.K))
        {
            float _targetX = Random.Range(-ground.bounds.size.x, ground.bounds.size.x);
            float _targetZ = Random.Range(-ground.bounds.size.z, ground.bounds.size.z);
            Vector3 _targetPos = new Vector3(_targetX, 0f, _targetZ);
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, .5f);
        }
        if (_currentPlayStyle == PlayStyle.aggresive)
        {
            //apply agressive movement
            if(Input.GetKeyUp(KeyCode.K))
            {
                float _targetX = Random.Range(-ground.bounds.size.x, ground.bounds.size.x);
                float _targetZ = Random.Range(-ground.bounds.size.z, ground.bounds.size.z);
                Vector3 _targetPos = new Vector3(_targetX, 0f, _targetZ);
                transform.position = Vector3.MoveTowards(transform.position, _targetPos, 5f);
            }
        } else if(_currentPlayStyle == PlayStyle.passive)
        {
            //apply passive movement
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
