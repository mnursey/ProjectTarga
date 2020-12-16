using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public int spareAmmunition = 100;
    public int maxSpareAmmunition = 150;

    public int magSize = 20;
    public int bulletsInMag = 20;

    public float reloadTime = 2.0f;
    public float reloadTimer = 0.0f;

    public float rateOfFire = 200;
    public float fireTimer = 0.0f;

    public Transform muzzleTransform;

    public AnimationCurve damageCurve;

    public LayerMask bulletMask;

    Camera c;
    public LineRenderer lr;

    public float bulletTrailFadeOut = 0.002f;
    public float bulletTrailFadeOutTimer = 0.0f;

    public float recoilForce = 0.01f;
    public float recoilAcceleration = 0.0f;
    public float recoilReturnForce = 0.0f;
    public float maxRecoilAcceleration = -5f;
    public float recoilReturnForceNotShooting = 0.0f;

    void Awake()
    {
        c = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lr.enabled = false;
    }

    void Update()
    {
        if(bulletTrailFadeOutTimer <= 0f)
        {
            lr.enabled = false;
        } else
        {
            lr.enabled = true;
            bulletTrailFadeOutTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        UpdateRecoil();

        if (PressedReload() && CouldReload())
        {
            Reload();
        }

        if (PressedShoot() && CouldShoot())
        {
            Shoot();
        }

        if(IsReloading())
        {
            reloadTimer -= Time.deltaTime;

            if(!IsReloading())
            {
                FinishReload();
            }
        }

        if(IsChambering())
        {
            fireTimer -= Time.deltaTime;
        }
    }

    bool CouldShoot()
    {
        return !IsReloading() && !IsChambering() && bulletsInMag > 0 ? true : false; 
    }

    bool IsChambering()
    {
        return fireTimer > 0f;
    }

    bool IsReloading()
    {
        return reloadTimer > 0f;
    }

    bool CouldReload()
    {
        return bulletsInMag < magSize && spareAmmunition > 0 && !IsReloading() ? true : false;
    }

    void Shoot()
    {
        fireTimer = 60f / rateOfFire;
        bulletsInMag -= 1;

        lr.positionCount = 2;
        lr.enabled = true;
        bulletTrailFadeOutTimer = bulletTrailFadeOut;

        RaycastHit hit;
        if (Physics.Raycast(c.transform.position, c.transform.forward, out hit, 1000.0f, bulletMask))
        {
            Debug.DrawRay(c.transform.position, c.transform.forward * hit.distance, Color.yellow);

            lr.SetPositions(new Vector3[] { Vector3.zero, Vector3.forward * hit.distance });
            lr.gameObject.transform.LookAt(hit.point);
        }
        else
        {
            Debug.DrawRay(c.transform.position, c.transform.forward * 1000.0f, Color.red);

            lr.SetPositions(new Vector3[] { Vector3.zero, Vector3.forward * 1000.0f });
        }

        recoilAcceleration -= recoilForce;
    }

    void UpdateRecoil()
    {
        recoilAcceleration = Mathf.Clamp(recoilAcceleration, maxRecoilAcceleration, 0.0f);
        recoilAcceleration = Mathf.MoveTowards(recoilAcceleration, 0.0f, recoilReturnForce * Time.deltaTime);

        if(!PressedShoot())
        {
            recoilAcceleration = Mathf.MoveTowards(recoilAcceleration, 0.0f, recoilReturnForceNotShooting * Time.deltaTime);
        }

        transform.parent.GetComponent<MouseLook>().xRotation += recoilAcceleration * Time.deltaTime;
    }

    void Reload()
    {
        reloadTimer = reloadTime;

        // Todo
        // Trigger animation
    }

    void FinishReload()
    {
        // Todo
        bulletsInMag += magSize;
    }

    bool PressedShoot()
    {
        if (Input.GetButton("Fire1"))
        {
            return true;
        }

        return false;
    }

    bool PressedReload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            return true;
        }

        return false;
    }
}
