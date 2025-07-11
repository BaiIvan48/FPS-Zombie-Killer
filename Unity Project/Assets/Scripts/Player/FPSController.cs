//FPSontroller.cs
using UnityEngine;

public partial class FPSController : MonoBehaviour
{
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidBody;
    private Animator _animator;
    private InputManager _inputManager;

    private void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        _rigidBody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();

        _capsuleCollider = GetComponent<CapsuleCollider>();

        _iKControl = GetComponent<IKControl>();

        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");
        _zVelHash = Animator.StringToHash("Z_Velocity");
        _jumpHash = Animator.StringToHash("Jump");
        _groundHash = Animator.StringToHash("Grounded");
        _fallingHash = Animator.StringToHash("Falling");
        _crouchHash = Animator.StringToHash("Crouch");

        _inputManager = GetComponent<InputManager>();
        if (_inputManager == null)
            Debug.LogError("FPSController: Missing InputManager on same GameObject");

        _inputManager.OnPickup += OnPickupPerformed;
        _iKControl.IkActive = false;

        _inputManager.OnJump += HandleJumpRequest;
    }

    private void Update()
    {
        DetectWeaponInFront();
    }

    private void FixedUpdate()
    {
        SetCamera();
        SampleGround();
        Move();
        HandleJump();
        HandleCrouch();
    }
    private void LateUpdate()
    {
        CamMovements();
        UpdateHeadLookTarget();
    }

    private void OnDestroy()
    {
        if (_inputManager != null)
            _inputManager.OnJump -= HandleJumpRequest;

        if (_inputManager != null)
            _inputManager.OnPickup -= OnPickupPerformed;
    }
}
