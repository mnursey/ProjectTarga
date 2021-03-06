﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AISoldierMode { IDLE, SEARCH, ATTACK, RELOAD, RETREAT }

public class AISoldier : MonoBehaviour
{

    public AISoldierMode mode;

    public float viewRadius = 20f;
    public float maxIntrestArea = 75f;
    public float lookAtSpeed = 300f;

    public float lowerTargetRange = 5f;
    public float upperTargetRange = 15f;

    public float maximumAttackRange = 20f;

    public float burstValue = 0.9f;
    public float burstFrequency = 1f;
    float burstOffset = 0.0f;

    public GameObject target = null;
    public GameObject lookHolder;

    public float heightYAimOffset = 1.6f;

    TeamTracker teamTracker;
    PlayerMovement playerMovement;
    WeaponController weaponController;
    Health health;

    private void Awake()
    {
        teamTracker = GetComponent<TeamTracker>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponController = GetComponentInChildren<WeaponController>();
        health = GetComponent<Health>();
        health.OnDeathSubscribe(OnDead);
        burstOffset = Random.Range(0.0f, 100f);
    }

    // Start is called before the first frame updatel
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(mode)
        {
            case AISoldierMode.IDLE:

                if(target == null)
                {
                    // Find new target
                    DetectNearTarget();
                } else
                {
                    LookTowardsTarget();

                    // Give up on target
                    if (Vector3.Distance(target.transform.position, transform.position) > maxIntrestArea)
                    {
                        target = null;
                    }

                    if (Vector3.Distance(target.transform.position, transform.position) < lowerTargetRange) {
                        MoveBackwards();
                    } else if ((Vector3.Distance(target.transform.position, transform.position) > upperTargetRange)) {
                        MoveForwards();
                    } else {
                        StandStill();
                    }

                    if(CanBurst() && Vector3.Distance(target.transform.position, transform.position) < maximumAttackRange)
                    {
                        Fire();
                    } else
                    {
                        if (weaponController.bulletsInMag < (weaponController.magSize / 3.0f))
                        {
                            Reload();
                        }
                        else
                        {
                            IdleGun();
                        }
                    }
                }

                break;
        }
    }

    bool CanBurst()
    {
        if(Mathf.Sin((Time.time * burstFrequency) + burstOffset) > burstValue)
        {
            return true;
        } else
        {
            return false;
        }
    }

    void DetectNearTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewRadius);

        foreach(Collider c in colliders)
        {
            TeamTracker tt = c.GetComponent<TeamTracker>();

            if(tt != null && c.transform != transform && !teamTracker.CompareTeam(tt))
            {
                if(target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(c.transform.position, transform.position))
                {
                    target = c.gameObject;
                }
            }
        }
    }

    void LookTowardsTarget()
    {
        Vector3 lookDir = target.transform.position - transform.position;
        Vector3 lookDirXZ = new Vector3(lookDir.x, 0f, lookDir.z).normalized;

        Vector3 lhLookDir = transform.InverseTransformPoint(target.GetComponent<Collider>().bounds.center) - lookHolder.transform.localPosition + new Vector3(0f, heightYAimOffset, 0f);
        Vector3 lookDirY = lhLookDir;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirXZ, Vector3.up), lookAtSpeed * Time.deltaTime);

        lookHolder.transform.localRotation = Quaternion.RotateTowards(lookHolder.transform.localRotation, Quaternion.LookRotation(lookDirY, Vector3.up), lookAtSpeed * Time.deltaTime);
        lookHolder.transform.localEulerAngles = new Vector3(lookHolder.transform.localEulerAngles.x, 0f, 0f);
    }

    void MoveBackwards()
    {
        playerMovement.z -= 1.0f;
    }

    void MoveForwards()
    {
        playerMovement.z += 1.0f;
    }

    void StandStill()
    {
        playerMovement.x = 0.0f;
        playerMovement.z = 0.0f;
        playerMovement.jump = false;
    }

    void Fire()
    {
        if(weaponController.bulletsInMag > 0)
        {
            weaponController.fire = true;
            weaponController.reload = false;
        }
        else
        {
            Reload();
        }
    }

    void Reload()
    {
        weaponController.fire = false;
        weaponController.reload = true;
    }
    void IdleGun()
    {
        weaponController.fire = false;
        weaponController.reload = false;
    }

    void OnDead()
    {
        gameObject.SetActive(false);
    }
}
