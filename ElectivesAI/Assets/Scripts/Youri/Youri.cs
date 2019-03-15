﻿using UnityEngine;
using System.Collections.Generic;

public class Youri : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank _tankInterface;
    private VisionCone _eyes;
    private AStarPath _AStarPath;
    private AStarNodeDetector _nodeDetector;
    private GameObject _nodeGrid;
    private bool _lookForNewTarget = true;
    public LayerMask NodeMask;
    private float RotateSpeedScan = 40;

    private enum TankState
    {
        Fight, Run, Patrol, Alerted
    }
    private TankState tankBehaviour;

    private void Start()
    {
        _nodeGrid = GameObject.Find("NodeListY");
        _tankInterface = GetComponent<Tank>();
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _AStarPath = transform.parent.GetComponentInChildren<AStarPath>();
        _nodeDetector = transform.parent.GetComponentInChildren<AStarNodeDetector>();
    }

    private void Update()
    {
        //Fight
        //stay in patrol state until enemy tank is found 
        //when seeing an enemy, try to get in range for shooting
        //when in shooting range, fire at enemy 
        if (_eyes.Target != null)
        {
            if (_eyes.Target.tag == "Tank")
            {
                tankBehaviour = TankState.Fight;
                Debug.Log("BackToFighting");
            }
            else
            {
                tankBehaviour = TankState.Patrol;
                Debug.Log("DontSeeAnythingBoss");
            }
        }

        switch (tankBehaviour)
        {
            case TankState.Fight:
                _tankInterface.RotateTurret(_eyes.Target.transform);

                Invoke("Shoot", 0.3f);
                break;

            case TankState.Alerted:
                transform.parent.Rotate(new Vector3(0, 0.5f, 0));
                break;

            case TankState.Patrol:

                break;
        }

        //Run 
        //when health falls below a certain threshold, shift to run state
        //move from the current position 
        //try to find healthpacks

        //Patrol
        //The usual state the tank is in when no enemy is found, heath is not below a certain level and you're not currently being shot at.
        //Walk around the area until a tank enters the line of sight or until being hit by an opposing tank

        //Alerted
        //Shift to this state when the tank is being damaged by a bullet without any tanks being in the vision cone of the tank
        //rotate the tank to find the enemy as soon as possible to fire back
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet" && _eyes.Target == null)
        {
            Debug.Log("Alerted");
            tankBehaviour = TankState.Alerted;
        }
        else if (other.tag == "Bullet" && _eyes.Target.tag == "Tank" && _tankInterface.GetHealth() > 4)
        {
            Debug.Log("Fight");
            tankBehaviour = TankState.Fight;
        }
        else if (other.tag == "Bullet" && _eyes.Target.tag == "Tank" && _tankInterface.GetHealth() <= 4)
        {
            Debug.Log("Run");
            tankBehaviour = TankState.Run;
        }
    }

    //have to write a function for getting in range of the tank 

    //Shooting function
    void Shoot()
    {
        _tankInterface.Shoot();
    }
}