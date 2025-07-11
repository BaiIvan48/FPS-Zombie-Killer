//FPSontroller_Camera.cs
using UnityEngine;

public partial class FPSController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera _firstPersonCamera;
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _cameraPosition;
    [SerializeField] private LayerMask _targetLayer;

    [SerializeField] private float _mouseSensitivity = 21.9f;
    [SerializeField] private float _cameraUpperLimit = -40f;
    [SerializeField] private float _cameraBottomLimit = 70f;
    private void CamMovements()
    {
        if (!_hasAnimator) return;

        float Mouse_X = _inputManager.Look.x;
        float Mouse_Y = _inputManager.Look.y;

        _xRotation -= Mouse_Y * _mouseSensitivity * Time.smoothDeltaTime;
        _xRotation = Mathf.Clamp(_xRotation, _cameraUpperLimit, _cameraBottomLimit);

        float recoilY = _currentWeapon != null ? _currentWeapon.CurrentRecoil.y : 0f;
        float recoilX = _currentWeapon != null ? _currentWeapon.CurrentRecoil.x : 0f;

        _firstPersonCamera.transform.localRotation = Quaternion.Euler(_xRotation + recoilY, recoilX, 0);

        //FirstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        _rigidBody.MoveRotation(_rigidBody.rotation * Quaternion.Euler(0, Mouse_X * _mouseSensitivity * Time.smoothDeltaTime, 0));
    }

    private void SetCamera()
    {
        _firstPersonCamera.transform.position = _cameraPosition.transform.position;
        _firstPersonCamera.transform.LookAt(_target.transform);
    }

    private void UpdateHeadLookTarget()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = _firstPersonCamera.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _targetLayer))
        {
            _target.transform.position = raycastHit.point;
        }
    }
}
