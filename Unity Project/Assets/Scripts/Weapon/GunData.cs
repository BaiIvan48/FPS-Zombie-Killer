using UnityEngine;

public enum WeaponType 
{
    Pistol,
    AR,
    SMG,
    Shotgun
}


[CreateAssetMenu(fileName = "NewGunData", menuName = "Gun/GunData")]
public class GunData : ScriptableObject
{
    public string GunName;
    public WeaponType WeaponType;
    public LayerMask TargetLayerMask;

    [Header("Fire Config")]
    public float ShootingRange;
    public float FireRate;
    public float Damage;

    [Header("Reload config")]
    public int MagazineSize;
    public float ReloadTime;

    [Header("Aim Settings")]
    public Vector3 WeaponDefaultPosition;
    public Quaternion WeaponDefaultRotation;
    public Vector3 WeaponAimPosition;
    public Quaternion WeaponAimRotation;
    public float AimTransitionSpeed = 10f;
    public float DefaultFOV = 60f; 
    public float AimFOV = 40f;

    [Header("Recoil Settings")]
    public float RecoilAmount = 2f;
    public float RecoilResetSpeed = 5f;

    [Header("VFX")]
    public float BulletSpeed;

    [Header("SFX")]
    public AudioClip FireSound;
    public AudioClip EquipSound;
    public AudioClip ReloadSound;
    public AudioClip FallOnGround;

    [Header("Weapon Sway")]
    public float PositionalSway = 0.2f;
    public float RotationalSway = 1f;
    public float SwaySmoothness = 5f;

    [Header("Weapon Bobbing")]
    public float BobbingSpeed = 2f;
    public float BobbingAmount = 0.001f;

    [Header("Recoil Animation")]
    public float PositionalRecoilAmount = 0.11f;
    public float RotationaRecoilAmount = 5f;
    public float RecoilSmoothness = 20f;
}
