using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Youri : Tank
{
    public Transform shootingPos; // a reference to the position at which the bullet is instantiated

    public override void Shoot(Transform shootingPos)
    {
        shootingPos = this.shootingPos; //assigning the shooting position to the current tank's shooting position
        base.Shoot(shootingPos);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            Shoot(shootingPos);
        }
    }
}
