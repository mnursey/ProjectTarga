using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public delegate void OnDeath();
    public event OnDeath onDeath;

    public int health = 100;
    public bool dead = false;

    public bool IsAlive()
    {
        if(health > 0)
        {
            return true;
        }

        return false;
    }

    public void Damage(int delta)
    {
        health -= delta;

        if(!dead && !IsAlive())
        {
            dead = true;

            Debug.Log("Dead");

            onDeath?.Invoke();
        }
    }

    public void Heal(int delta)
    {
        health += delta;
    }

    public void OnDeathSubscribe(OnDeath func)
    {
        onDeath += func;
    }
}
