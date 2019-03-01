using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artem : MonoBehaviour
{
    // Variable to store the tank interface script component for state machine use.
    private Tank _tankInterface;

    private void Start()
    {
        _tankInterface = GetComponent<Tank>();
    }
    
    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}
