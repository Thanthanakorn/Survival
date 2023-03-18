using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool shiftInput;
    public bool ctrlInput;
    public bool rollFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public bool rbInput;
    public bool rtInput;
    public bool jumpInput;

    private PlayerControls _inputActions;
    private PlayerAttacker _playerAttacker;
    private CameraHandler _cameraHandler;
    private PlayerInventory _playerInventory;
    private PlayerManager _playerManager;

    private Vector2 _movementInput;
    private Vector2 _cameraInput;

    private void Awake()
    {
        _playerAttacker = GetComponent<PlayerAttacker>();
        _playerInventory = GetComponent<PlayerInventory>();
        _playerManager = GetComponent<PlayerManager>();
    }

    public void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerControls();
            _inputActions.PlayerMovement.Move.performed +=
                inputActions => _movementInput = inputActions.ReadValue<Vector2>();
            _inputActions.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
        }

        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollingAndSprintingInput(delta);
        HandleAttackInput(delta);
        HandleJumpInput();
    }

    private void MoveInput(float delta)
    {
        horizontal = _movementInput.x;
        vertical = _movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = _cameraInput.x;
        mouseY = _cameraInput.y;
    }

    private void HandleRollingAndSprintingInput(float delta)
    {
        ctrlInput = _inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        shiftInput = _inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

        rollFlag = ctrlInput;
        sprintFlag = shiftInput;
    }

    private void HandleAttackInput(float delta)
    {
        _inputActions.PlayerActions.RB.performed += _ => rbInput = true;
        _inputActions.PlayerActions.RT.performed += _ => rtInput = true;

        if (rbInput)
        {
            if (_playerManager.canDoCombo)
            {
                comboFlag = true;
                _playerAttacker.HandleWeaponCombo(_playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (_playerManager.isInteracting || _playerManager.canDoCombo)
                {
                    return;
                }
                _playerAttacker.HandleLightAttack(_playerInventory.rightWeapon);
            }
        }

        if (rtInput)
        {
            if (_playerManager.canDoCombo)
            {
                comboFlag = true;
                _playerAttacker.HandleWeaponCombo(_playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                _playerAttacker.HandleHeavyAttack(_playerInventory.rightWeapon);
            }
        }
    }
    
    private void HandleJumpInput()
    {
        _inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
    }
}