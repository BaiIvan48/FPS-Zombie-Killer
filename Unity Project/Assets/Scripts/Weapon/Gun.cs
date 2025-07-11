using System;
using System.Collections;
using UnityEngine;

public abstract partial class Gun : MonoBehaviour
{
    [Header("References")]
    public GunData GunData;
    public Transform GunMuzzle;
    public bool IsEquipped { get; private set; } = false;
    public int GetCurrentAmmo => currentAmmo; 
    public WeaponType GetWeaponType => GunData.WeaponType;

    public event Action<bool> OnAimStateChanged;

    protected InputManager input;
    protected Transform cameraTransform;
    protected Animator animator;
    
    protected int currentAmmo;
    protected float nextTimeToFire;
    protected bool isReloading = false;
    protected bool isShooting = false;
    protected bool isRecoiling = false;
    protected bool isAimingState = false;
    protected bool hasLanded = false;
    
    private Camera playerCamera;
    private Vector2 Look => input.Look;
    private Vector2 Move => input.Move;


    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void SubscribeInput()
    {
        input.OnShootStart += StartFiring;
        input.OnShootStop += StopFiring;
        input.OnReload += TryReload;
        input.OnAim += HandleAim;
    }

    protected virtual void UnsubscribeInput()
    {
        input.OnShootStart -= StartFiring;
        input.OnShootStop -= StopFiring;
        input.OnReload -= TryReload;
        input.OnAim -= HandleAim;
    }

    protected virtual void OnDestroy()
    {
        if (input != null)
        {
            UnsubscribeInput();
        }
    }

    public void Equip()
    {
        IsEquipped = true;
        hasLanded = false;
        input = GetComponentInParent<InputManager>();
        if (input == null)
        {
            Debug.LogError("Missing InputManager on parent");
            return;
        }

        animator = GetComponentInParent<Animator>();

        SubscribeInput();
        //PlaySound(GunData.EquipSound);
        animator.SetBool("Reloading", false);

        currentAmmo = GunData.MagazineSize;

        playerCamera = Camera.main;
        cameraTransform = playerCamera.transform;
    }

    public void Unequip()
    {
        IsEquipped = false;

        isReloading = false;
        isShooting = false;
        isRecoiling = false;
        isAimingState = false;
        animator.SetBool("Reloading", false);

        StopAllCoroutines();
        UnsubscribeInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlaySound(GunData.FallOnGround);
            hasLanded = true;
        }
    }


    protected virtual void Update()
    {
        if (!IsEquipped) return;

        if (input.Run && isAimingState)
        {
            isAimingState = false;
        }

        ResetTargetRecoil();
        HandleAimTransition();

        if (!isAimingState) {
        ApplySway();
        ApplyBobbing();
        ApplyRecoil();
        }
    }
}
