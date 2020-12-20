using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum WeaponControllerMode { Player, Puppet };

public class WeaponController : MonoBehaviour
{
    public WeaponControllerMode mode = WeaponControllerMode.Puppet;

    [Header("Stats")]

    public int spareAmmunition = 100;
    public int maxSpareAmmunition = 150;

    public int magSize = 20;
    public int bulletsInMag = 20;

    public float reloadTime = 2.0f;
    public float reloadTimer = 0.0f;

    public float rateOfFire = 200;
    public float fireTimer = 0.0f;

    public AnimationCurve damageCurve;

    public LayerMask bulletMask;

    [Header("References")]

    Camera c;
    public CameraShake cameraShake;
    public LineRenderer lr;
    public VisualEffect muzzleFlash;
    public Light muzzleFlashLight;
    public Transform muzzleTransform;
    public Animator anim;

    [Header("Bullet Trail")]

    public float bulletTrailFadeOut = 0.002f;
    public float bulletTrailFadeOutTimer = 0.0f;

    [Header("Camera Shake")]

    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.1f;

    public AnimationCurve knockBackCurve;
    public float knockBackDuration = 0.15f;

    [Header("Cone of Fire")]

    public AnimationCurve coneOfFireCurve;
    [Range(0.0f, 1.0f)]
    public float weaponStability = 0.0f;
    public float bulletStabilityIncrease = 0.15f;
    public float stabilityNormalization = 0.10f;

    void Awake()
    {
        c = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lr.enabled = false;
        muzzleFlash = GetComponentInChildren<VisualEffect>();
        muzzleFlashLight = GetComponentInChildren<Light>();
        muzzleFlashLight.enabled = false;
        cameraShake = c.GetComponent<CameraShake>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        GameUIController.UpdateClipAmmo(bulletsInMag);
        GameUIController.UpdateSpareAmmo(spareAmmunition);
    }

    void Update()
    {
        if(bulletTrailFadeOutTimer <= 0f)
        {
            lr.enabled = false;
            muzzleFlashLight.enabled = false;
        } else
        {
            lr.enabled = true;
            muzzleFlashLight.enabled = true;
            bulletTrailFadeOutTimer -= Time.deltaTime;
        }

        weaponStability = Mathf.MoveTowards(weaponStability, 0.0f, stabilityNormalization * Time.deltaTime);
    }

    void FixedUpdate()
    {
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

        Vector3 coneOffset = Random.insideUnitSphere * coneOfFireCurve.Evaluate(weaponStability);

        Vector3 shotDirection = (c.transform.forward + coneOffset).normalized;

        RaycastHit hit;
        if (Physics.Raycast(c.transform.position, shotDirection, out hit, 1000.0f, bulletMask))
        {
            Debug.DrawRay(c.transform.position, shotDirection * hit.distance, Color.yellow);

            lr.SetPositions(new Vector3[] { Vector3.zero, lr.transform.InverseTransformPoint(hit.point)});
        }
        else
        {
            Debug.DrawRay(c.transform.position, shotDirection * 1000.0f, Color.red);

            lr.SetPositions(new Vector3[] { Vector3.zero, lr.transform.InverseTransformPoint(shotDirection * 1000.0f) });
        }

        muzzleFlash.Play();
        muzzleFlashLight.enabled = true;

        weaponStability += bulletStabilityIncrease;
        weaponStability = Mathf.Clamp01(weaponStability);

        anim.SetTrigger("Shoot");

        if (IsPlayer())
        {
            StartCoroutine(cameraShake.Shake(shakeDuration, shakeStrength));
            StartCoroutine(cameraShake.KnockBack(knockBackDuration, knockBackCurve));
            GameUIController.UpdateClipAmmo(bulletsInMag);
        }
    }

    void Reload()
    {
        reloadTimer = reloadTime;
        anim.SetTrigger("Reload");
    }

    void FinishReload()
    {
        int numBulletsAdded = Mathf.Min(magSize, spareAmmunition) - bulletsInMag;
        spareAmmunition -= numBulletsAdded;
        bulletsInMag += numBulletsAdded;

        if(IsPlayer())
        {
            GameUIController.UpdateClipAmmo(bulletsInMag);
            GameUIController.UpdateSpareAmmo(spareAmmunition);
        }
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

    bool IsPlayer()
    {
        return mode == WeaponControllerMode.Player;
    }
}
