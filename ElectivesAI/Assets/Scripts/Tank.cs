﻿using System.Collections;
using UnityEngine;

public class Tank : MonoBehaviour 
{
    #region Default tank variables
    private int _health = 10;

    private const float _movementSpeed = 250f;
    private const float _rotationSpeed = 100f;
    private const float _turretRotationSpeed = 5f;
    private Vector3 _turretStartRotation;
    private bool _isTurretRotated = true;

    #region Shooting mechanic variables
    private int _ammo = 100;
    private const float _fireRate = 1;
    private bool _canShoot = true;
    #endregion

    private const float _fireRange = 10;
    private const float _bulletSpeed = 150f;
    #endregion

    #region Tank component references
    private Rigidbody _rb;
    private GameObject _turret;
    private VisionCone _eyes;
    #endregion

    #region References for the shooting function
    public GameObject BulletPrefab; // A reference to the the bullet object prefab
    public GameObject CannonHead;  // A reference to the canon head from which the bullet is being shot from
    #endregion

    private void Awake()
    {
        string lastLetter = transform.parent.name.Substring(transform.parent.name.Length - 1);
        GenerateGrid(lastLetter);
    }

    private void Start()
    {
        _eyes = transform.parent.GetComponentInChildren<VisionCone>();
        _rb = GetComponent<Rigidbody>();
        _turret = transform.GetChild(0).gameObject;
        _turretStartRotation = transform.position;
    }

    #region Tank movement and rotation functions
    // The tank can move forward or backward and the behaviour of that is
    // going to be extended in the individual scripts 
    public void MoveTheTank(string direction)
    {
        Vector3 _velocity = transform.forward.normalized * _movementSpeed * 0.01f;

        if (_velocity != Vector3.zero)
        {
            if (direction == "Forwards" ||
                direction == "Forward")
            {
                _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
            }
            else if (direction == "Backwards" ||
              direction == "Backward")
            {
                _rb.MovePosition(_rb.position + (_velocity * -1) * Time.fixedDeltaTime);
            }
        }
    }

    // Rotates the tank in such a way as to turn its direction to
    // an object's position on the map
    public void RotateTheTank(Transform objectToRotateTowards)
    {
        Quaternion newRotation = Quaternion.LookRotation(objectToRotateTowards.position - transform.position);

        // It will slowly turn the player to the new direction it has to
        // look towards.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, _rotationSpeed * Time.deltaTime);
    }

    // Rotates the turret towards a specified object's transform.
    public void RotateTurret(Transform objectToRotateTowards)
    {
        Vector3 targetDir = objectToRotateTowards.position - _turret.transform.position;

        Vector3 newDir = Vector3.RotateTowards(_turret.transform.forward, targetDir, _turretRotationSpeed * Time.deltaTime, 0.0f);
        
        _turret.transform.rotation = Quaternion.LookRotation(newDir);
    }

    // We use this to reset the tank's turret position if there is no
    // target to look at.
    public void ResetTurretRotation()
    {
        Vector3 newDir = Vector3.RotateTowards(_turret.transform.forward, _turretStartRotation, _turretRotationSpeed * Time.deltaTime, 0.0f);

        _turret.transform.rotation = Quaternion.LookRotation(newDir);
    }
    #endregion

    #region Tank Stats getter functions
    public int GetAmmo()
    {
        return _ammo;
    }
    public int GetHealth()
    {
        return _health;
    }
    public bool GetEnableShooting()
    {
        return _canShoot;
    }
    // Can be used to define different behaviour such as spotting or being 
    // spotted and alerting the tank in diffrent ways to react accordingly.
    public Vector3 GetTankRotation()
    {
        return transform.eulerAngles;
    }
    public Vector3 GetTurretRotation()
    {
        return _turret.transform.eulerAngles;
    }
    public float GetFireRange()
    {
        return _fireRange;
    }
    #endregion

    #region Shooting Related functions
    public void Shoot()
    {
        // We allow the tank to shoot only if he has ammo in the reserves.
        if (_ammo > 0 && _canShoot)
        {
            // This line stores the position where we want the bullet to be shot from.
            Transform shootingPos = transform.GetChild(0).transform.GetChild(2).transform;

            // An instance of a bullet is created and is set to a child of the shooting position (end of the barrel)
            GameObject bullet = Instantiate(BulletPrefab, shootingPos.position, Quaternion.identity);

            // Makes it so that the bullet ignores the collisions between itself and the cannon head since the bullet comes out of the cannon and it might cause unexpected behaviour.
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), CannonHead.GetComponent<Collider>());

            // Adding force to the bullet
            bullet.GetComponent<Rigidbody>().AddForce(shootingPos.forward * _bulletSpeed * 200 * Time.deltaTime);

            // We stop the player from shooting twice in order to emulate
            // the projectiles being loaded in after a moment depending on
            // the fire rate.
            Invoke("EnableShooting", _fireRate);
            StartCoroutine(DestroyBullet(bullet));
            _ammo--;
            _canShoot = false;

        }
    }

    // This enables the tank to shoot based on his fire rate value
    private void EnableShooting()
    {
        _canShoot = true;
    }

    // Destroys the instantiated bullet when its shot after a few seconds so
    // that it does not go on forever.
    private IEnumerator DestroyBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(1f);
        Destroy(bullet);
    }
    #endregion

    // This checks for collisions with bullets from foreign tanks
    // and damages the tank, potentially destroying it.
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            _health--;

            if (_health <= 0)
            {
                Destroy(gameObject);
            }

            Destroy(other.gameObject);
        }

        if (other.tag == "Ammo Pickup")
        {
            _ammo += 5;
            _eyes.Target = null;
            Destroy(other.gameObject);
            //Debug.Log("Ammo retrieved.");
        }
    }

    private void GenerateGrid(string letter)
    {
        var nodeParent = new GameObject("NodeList" + letter);
        int gridDepth = 26;
        int gridWidth = 26;

        var nodeArray = new AStarNode[gridDepth, gridWidth];
        for (int y = 0; y < gridDepth; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //GameObject(GameObject, new Vector3(x, 0.0f, y), Quaternion.identity);
                var gameObject = new GameObject("Node" + letter);
                gameObject.tag = "Node" + letter;
                if (letter == "N")
                {
                    gameObject.layer = 9;
                }

                BoxCollider nodeCollider = gameObject.AddComponent<BoxCollider>();
                nodeCollider.size = new Vector3(1.9f, 0.5f, 1.9f);
                nodeCollider.isTrigger = true;

                gameObject.transform.position = new Vector3(x * 2f - 25, 0.32f, y * 2f - 25);
                gameObject.transform.parent = nodeParent.transform;
                var node = gameObject.AddComponent<AStarNode>();

                nodeArray[x, y] = node;
                node.unWalkable = false;
            }
        }

        for (int y = 0; y < gridDepth; y++)
        {
            for (int x = 0; x < gridWidth - 1; x++)
            {
                var currentNode = nodeArray[x, y];
                var nextNode = nodeArray[x + 1, y];

                currentNode.connections.Add(nextNode);
                nextNode.connections.Add(currentNode);
            }
        }

        for (int y = 0; y < gridDepth - 1; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                var currentNode = nodeArray[x, y];
                var nextNode = nodeArray[x, y + 1];

                currentNode.connections.Add(nextNode);
                nextNode.connections.Add(currentNode);
            }
        }
    }
}
