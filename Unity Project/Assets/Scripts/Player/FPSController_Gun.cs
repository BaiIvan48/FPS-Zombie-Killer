//FPSontroller_Gun.cs
using System;
using UnityEngine;

public partial class FPSController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask _weaponLayerMask;  
    [SerializeField] private Transform _cameraTransform;  

    [Header("Weapon Handling")]
    [SerializeField] private Transform _weaponHolder;

    private Gun _currentWeapon;
    private Gun _focusedWeapon;

    private IKControl _iKControl;

    public Gun CurrentWeapon => _currentWeapon; 
    
    public event Action<Gun> OnWeaponChanged; 

    private void DetectWeaponInFront()
    {
        Vector3 origin = _cameraTransform.position;
        Vector3 direction = (_target.transform.position - origin).normalized;
        float radius = 0.2f;
        float maxDistance = 2f;

        Ray ray = new Ray(origin, direction);
        if (Physics.SphereCast(ray, radius, out RaycastHit hit, maxDistance, _weaponLayerMask))
        {
            _focusedWeapon = hit.collider.GetComponentInParent<Gun>();
        }
        else
        {
            _focusedWeapon = null;
        }
    }

    private void OnPickupPerformed()
    {
        if (_focusedWeapon != null)
            PickupWeapon(_focusedWeapon);
    }

    public void PickupWeapon(Gun newWeapon)
    {
        if (_currentWeapon != null)
            DropCurrentWeapon();

        _currentWeapon = newWeapon;
        _currentWeapon.transform.SetParent(_weaponHolder, worldPositionStays: false);

        if (_currentWeapon.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;
        if (_currentWeapon.TryGetComponent(out Collider col))
            col.enabled = false;

        Transform rightHandPos = _currentWeapon.transform.Find("Right Hand Pos");
        Transform leftHandPos = _currentWeapon.transform.Find("Left Hand Pos");

        if (rightHandPos == null || leftHandPos == null)
        {
            Debug.LogWarning($"The gun is missing 'Right Hand Pos' and/or 'Left Hand Pos'!");
        }
        else
        {
            _iKControl.RightHandTarget = rightHandPos;
            _iKControl.LeftHandTarget = leftHandPos;
        }

        _currentWeapon.Equip();
        OnWeaponChanged?.Invoke(_currentWeapon); 

        _iKControl.IkActive = true;
    }

    public void DropCurrentWeapon()
    {
        if (_currentWeapon == null) return;

        _currentWeapon.Unequip();

        _currentWeapon.transform.SetParent(null, worldPositionStays: true);
        if (_currentWeapon.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(_cameraTransform.forward * 5f, ForceMode.Impulse);
        }
        if (_currentWeapon.TryGetComponent(out Collider col))
            col.enabled = true;

        _currentWeapon = null;

        OnWeaponChanged?.Invoke(null);

        _iKControl.IkActive = false;
    }

}
