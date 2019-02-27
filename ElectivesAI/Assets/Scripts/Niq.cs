using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Niq : MonoBehaviour
{
    public Transform shootingPos; // a reference to the position at which the bullet is instantiated
    private Tank tankN;
    private Rigidbody rb;

    private float zMov; // float value containing info about direction of the movement
    private float yRot;// float value containing info about direction of the rotation

    private void Start()
    {
        tankN = FindObjectOfType<Tank>(); // reference to the tank class script
        rb = GetComponent<Rigidbody>(); // reference to the rigidbody
    }

    //function responsible for the movement and rotation of the tank
    private void TankMovement()
    {
        zMov = Input.GetAxisRaw("Vertical");
        yRot = Input.GetAxisRaw("Mouse X");
        tankN.MoveTheTank(zMov, rb, gameObject.transform);
        tankN.RotateTheTank(yRot, rb);
    }
    //shooting function
    private void TankShooting()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            tankN.Shoot(shootingPos);
        }
    }

    private void Update()
    {
        TankShooting();
        TankMovement();
    }
}
