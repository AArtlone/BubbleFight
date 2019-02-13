using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour 
{
    #region Default tank variables
    private float _health;

    private float _movementSpeed;
    private float _rotationSpeed;

    private int _ammo;  
    private float _fireRate;
    private float _fireRange;
    private float _bulletSpeed = 1000f;
    #endregion

    #region References for the shooting function
    public GameObject BulletPrefab; // A reference to the the bullet object prefab
    public GameObject CannonHead;  // A reference to the canon head from which the bullet is being shot from
    #endregion

    public virtual void Shoot(Transform shootingPos)
    {
        // An instance of a bullet and setting it a child of a shooting position
        GameObject bullet = Instantiate(BulletPrefab, shootingPos.position, Quaternion.identity, shootingPos);

        // Ignore the collision between the bullet and a cannon head because the bullet comes out of the cannon.
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), CannonHead.GetComponent<Collider>());

        // Adding force to the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingPos.forward * _bulletSpeed * 20 * Time.deltaTime);
    }
}
