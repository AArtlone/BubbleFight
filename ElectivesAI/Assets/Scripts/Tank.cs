using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour 
{
    #region Default tank variables
    private float _health;

    private static float _movementSpeed = 10;
    private static float _rotationSpeed = 10;
    private float _turretRotationSpeed;

    private int _ammo;  
    private float _fireRate;
    private float _fireRange;
    private float _bulletSpeed = 1000f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    #endregion

    #region References for the shooting function
    public GameObject BulletPrefab; // A reference to the the bullet object prefab
    public GameObject CannonHead;  // A reference to the canon head from which the bullet is being shot from
    #endregion

    #region Movement Functions
    //sets the velocity of movement
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    //moves the rigidbody
    public void PerformMovement(Rigidbody rb)
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }
    //i dont know how to name it xD
    public void MoveTheTank(float _zMov, Rigidbody rb, Transform _transform)
    {
        float zMov = _zMov;
        Vector3 movVertical = _transform.forward * zMov;
        Vector3 _velocity = movVertical.normalized * _movementSpeed;
        Move(_velocity);
        PerformMovement(rb);
    }
    #endregion

    #region Rotation functions
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    
    public void PerformRotation(Rigidbody rb)
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }
    
    public void RotateTheTank(float _yRot, Rigidbody rb)
    {
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * _rotationSpeed;
        Rotate(_rotation);
        PerformRotation(rb);
    }
    #endregion

    #region Turret Related Functions
    public void Shoot(Transform shootingPos)
    {
        // An instance of a bullet and setting it a child of a shooting position
        GameObject bullet = Instantiate(BulletPrefab, shootingPos.position, Quaternion.identity, shootingPos);

        // Ignore the collision between the bullet and a cannon head because the bullet comes out of the cannon.
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), CannonHead.GetComponent<Collider>());

        // Adding force to the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingPos.forward * _bulletSpeed * 20 * Time.deltaTime);
    }
    public void RotateTurret()
    {
        //TODO: add rotation of the turret
    }
    #endregion
}
