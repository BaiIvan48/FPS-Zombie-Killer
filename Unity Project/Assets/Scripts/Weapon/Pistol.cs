using System.Collections;
using UnityEngine;

public class Pistol : Gun
{
    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 target = Vector3.zero;
        bool hitSomething = false;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, GunData.ShootingRange, GunData.TargetLayerMask))
        {
            target = hit.point;
            hitSomething = true;
        }
        else
        {
            target = cameraTransform.position + (cameraTransform.forward) * GunData.ShootingRange;
        }

        StartCoroutine(BulletFire(target, hit, hitSomething));
    }

}
