using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _playerInput;

    // Player actions
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }
    public bool Crouch { get; private set; }

    // Gun actions
    public bool IsShooting { get; private set; }
    public bool IsReloading { get; private set; }
    public bool IsAiming { get; private set; }

    public bool Pickup { get; private set; } 

    public event Action OnShootStart;
    public event Action OnShootStop;
    public event Action OnReload;
    public event Action<bool> OnAim;
    public event Action OnPickup;
    public event Action OnJump;

    private InputActionMap _playerMap;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _runAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;

    private InputActionMap _gunMap;

    private InputAction _shootAction;
    private InputAction _reloadAction;
    private InputAction _aimAction;
    private InputAction _pickupAction;      

    private void Awake()
    {
        _playerMap = _playerInput.actions.FindActionMap("Player");
        _gunMap = _playerInput.actions.FindActionMap("Gun");

        InitializePlayerActions();
        SubscribePlayerInputEvents();

        InitializeGunActions();
        SubscribeGunInputEvents();

        HideCursor();
    }

    private void InitializePlayerActions()
    {
        _moveAction = _playerMap.FindAction("Move");
        _lookAction = _playerMap.FindAction("Look");
        _runAction = _playerMap.FindAction("Sprint");
        _jumpAction = _playerMap.FindAction("Jump");
        _crouchAction = _playerMap.FindAction("Crouch");
    }

    private void InitializeGunActions()
    {
        _shootAction = _gunMap.FindAction("Shoot");
        _reloadAction = _gunMap.FindAction("Reload");
        _aimAction = _gunMap.FindAction("Aim");
        _pickupAction= _gunMap.FindAction("Pickup");     
    }

    private void SubscribePlayerInputEvents()
    {
        _moveAction.performed += ctx => Move = ctx.ReadValue<Vector2>();
        _moveAction.canceled += _ => Move = Vector2.zero;

        _lookAction.performed += ctx => Look = ctx.ReadValue<Vector2>();
        _lookAction.canceled += _ => Look = Vector2.zero;

        _runAction.performed += ctx => Run = true;
        _runAction.canceled += _ => Run = false;

        _jumpAction.performed += ctx => {Jump = true;OnJump?.Invoke();};
        _jumpAction.canceled += _ => Jump = false;

        _crouchAction.performed += ctx => Crouch = true;
        _crouchAction.canceled += _ => Crouch = false;
    }

    private void SubscribeGunInputEvents()
    {
        _shootAction.performed += _ => { IsShooting = true; OnShootStart?.Invoke(); };
        _shootAction.canceled += _ => { IsShooting = false; OnShootStop?.Invoke(); };

        _reloadAction.performed += _ => OnReload?.Invoke();

        _aimAction.performed += _ => { IsAiming = true; OnAim?.Invoke(true); };
        _aimAction.canceled += _ => { IsAiming = false; OnAim?.Invoke(false); };

        _pickupAction.performed += _ => { Pickup = true; OnPickup?.Invoke(); };      
        _pickupAction.canceled += _ => Pickup = false;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnEnable()
    {
        _playerMap?.Enable();
        _gunMap?.Enable();
    }

    private void OnDisable()
    {
        _playerMap?.Disable();
        _gunMap?.Disable();
    }
}