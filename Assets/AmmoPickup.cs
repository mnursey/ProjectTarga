using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int bullets = 50;

    public int Pickup()
    {
        Destroy(gameObject);

        // Set bullets to zero so 
        // you can't pickup bullets 
        // twice before destory

        int tBullets = bullets;
        bullets = 0;

        return tBullets;
    }
}
