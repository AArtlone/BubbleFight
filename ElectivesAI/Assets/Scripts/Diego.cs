using UnityEngine;

public class Diego : MonoBehaviour
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
