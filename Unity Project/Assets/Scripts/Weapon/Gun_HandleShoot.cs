using System;
using System.Collections;
using UnityEngine;

public abstract partial class Gun : MonoBehaviour
{

    public event Action<int, int> OnAmmoChanged;
    private void HandleAim(bool isAiming)
    {
        if (isReloading || input.Run)
        {
            isAimingState = false;
            return;
        }

        isAimingState = isAiming;
        OnAimStateChanged?.Invoke(isAimingState);
    }

    private void HandleAimTransition()
    {
        Vector3 targetPos = isAimingState ? GunData.WeaponAimPosition : GunData.WeaponDefaultPosition;
        Quaternion targetRot = isAimingState ? GunData.WeaponAimRotation : GunData.WeaponDefaultRotation;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * GunData.AimTransitionSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * GunData.AimTransitionSpeed);

        float desiredFOV = isAimingState ? GunData.AimFOV : GunData.DefaultFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, desiredFOV, Time.deltaTime * GunData.AimTransitionSpeed);
    }

    protected virtual void StartFiring()
    {
        if (isReloading) return;
        isShooting = true;
        StartCoroutine(AutoFireLoop());
    }

    protected virtual void StopFiring() => isShooting = false;

    protected virtual IEnumerator AutoFireLoop()
    {
        while (isShooting && !isReloading && currentAmmo > 0)
        {
            if (Time.time >= nextTimeToFire)
            {
                HandleShoot();
                nextTimeToFire = Time.time + (1f / GunData.FireRate);
            }
            yield return null;
        }

        if (currentAmmo <= 0 && !isReloading)
        {
            TryReload();
        }
    }

    protected void HandleShoot()
    {
        currentAmmo--;
        isRecoiling = true;
        OnAmmoChanged?.Invoke(currentAmmo, GunData.MagazineSize);
        Shoot();
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        ApplyTargetRecoil();
        PlaySound(GunData.FireSound);
    }
    public abstract void Shoot();

    public void TryReload()
    {
        if (!isReloading && currentAmmo < GunData.MagazineSize)
        {
            isAimingState = false;
            PlaySound(GunData.ReloadSound);
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        isShooting = false;
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(GunData.ReloadTime);
        currentAmmo = GunData.MagazineSize;
        isReloading = false;
        animator.SetBool("Reloading", false);
        OnAmmoChanged?.Invoke(currentAmmo, GunData.MagazineSize); 
    }
}
