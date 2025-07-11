using System.Collections;
using UnityEngine;

public class AR : Gun
{
    [Header("AR Settings")]
    public bool IsBurstMode = false;
    public int BurstCount = 3;
    public float BurstInterval = 0.1f;

    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 target;
        bool hitSomething = false;

        if (Physics.Raycast(cameraTransform.position,cameraTransform.forward,out hit,GunData.ShootingRange,GunData.TargetLayerMask))
        {
            target = hit.point;
            hitSomething = true;
        }
        else
        {
            target = cameraTransform.position + cameraTransform.forward * GunData.ShootingRange;
        }

        StartCoroutine(BulletFire(target, hit, hitSomething));
    }

    protected override void StartFiring()
    {
        if (isReloading) return;

        if (IsBurstMode)
        {
            StartCoroutine(BurstFire());
        }
        else
        {
            base.StartFiring();
        }
    }

    protected override void StopFiring()
    {
        if (!IsBurstMode)
        {
            base.StopFiring();
        }
    }

    private IEnumerator BurstFire()
    {
        isShooting = true;

        for (int i = 0; i < BurstCount; i++)
        {
            if (currentAmmo <= 0) TryReload();

            if (Time.time >= nextTimeToFire)
            {
                HandleShoot();
                nextTimeToFire = Time.time + (1f / GunData.FireRate);
            }
            yield return new WaitForSeconds(BurstInterval);
        }

        isShooting = false;
    }

}
