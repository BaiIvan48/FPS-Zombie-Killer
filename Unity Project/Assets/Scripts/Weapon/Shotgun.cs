using System.Collections;
using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun Settings")]
    public int PelletCount = 8;
    public float SpreadAngle = 12f;

    public override void Shoot()
    {
        for (int i = 0; i < PelletCount; i++)
        {
            float yaw = Random.Range(-SpreadAngle, SpreadAngle);
            float pitch = Random.Range(-SpreadAngle, SpreadAngle);
            Vector3 dir = Quaternion.Euler(pitch, yaw, 0) * cameraTransform.forward;

            RaycastHit hit;
            Vector3 target;
            bool hitSomething = false;

            if (Physics.Raycast(cameraTransform.position,dir,out hit,GunData.ShootingRange,GunData.TargetLayerMask))
            {
                target = hit.point;
                hitSomething = true;
            }
            else
            {
                target = cameraTransform.position + dir * GunData.ShootingRange;
            }

            StartCoroutine(BulletFire(target, hit, hitSomething));
        }
    }
}
