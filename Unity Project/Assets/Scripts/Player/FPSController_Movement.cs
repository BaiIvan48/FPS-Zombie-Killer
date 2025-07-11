//FPSontroller_Movement.cs
using UnityEngine;

public partial class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _jumpForce = 350f;
    [SerializeField] private float _airResistance = 0.8f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _headTop;

    private Vector2 _currentVelocity;
    private bool _grounded = false;
    private bool _isCrouching = false;
    private bool _jumpRequested = false;
    private float _xRotation;

    private void Move()
    {
        if (!_hasAnimator) return;

        float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
        if (_inputManager.Crouch) targetSpeed = 1.5f;
        if (_inputManager.Move == Vector2.zero) targetSpeed = 0;


        if (_grounded)
        {
            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, _animationBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, _animationBlendSpeed * Time.fixedDeltaTime);

            float xVelDifference = _currentVelocity.x - _rigidBody.linearVelocity.x;
            float zVelDifference = _currentVelocity.y - _rigidBody.linearVelocity.z;

            _rigidBody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
        }
        else
        {
            _rigidBody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * _airResistance, 0, _currentVelocity.y * _airResistance)), ForceMode.VelocityChange);
        }

        _animator.SetFloat(_xVelHash, _currentVelocity.x);
        _animator.SetFloat(_yVelHash, _currentVelocity.y);
    }

    private void HandleCrouch()
    {
        bool wantsToCrouch = _inputManager.Crouch;

        if (wantsToCrouch)
        {
            _isCrouching = true;
        }
        else if (_isCrouching && CanStandUp())
        {
            _isCrouching = false;
        }

        _animator.SetBool(_crouchHash, _isCrouching);
    }

    private bool CanStandUp()
    {
        return !Physics.Raycast(_headTop.position, Vector3.up, 1f, _groundLayer);
    }


    private void HandleJumpRequest()
    {
        _jumpRequested = true;
    }
    private void HandleJump()
    {
        if (!_hasAnimator || !_grounded || !_jumpRequested)
            return;

        _jumpRequested = false;
        _animator.SetTrigger("Jump");

        _rigidBody.AddForce(-_rigidBody.linearVelocity.y * Vector3.up, ForceMode.VelocityChange);
        _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _animator.SetBool("Grounded", false);
        _animator.ResetTrigger(_jumpHash);
        _grounded = false;
    }

    private void SampleGround()
    {
        if (!_hasAnimator) return;

        Vector3 worldCenter = transform.TransformPoint(_capsuleCollider.center);
        if (Physics.SphereCast(worldCenter, 0.2f, Vector3.down, out RaycastHit hitInfo, _capsuleCollider.height/2 + 0.1f, _groundLayer))
        {
            _grounded = true;
            SetAnimationGrounding();
        }
        else
        {
            _grounded = false;
            _animator.SetFloat(_zVelHash, _rigidBody.linearVelocity.y);
            SetAnimationGrounding();
        }
        return;
    }

}
