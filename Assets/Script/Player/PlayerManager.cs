using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputHandler _inputHandler;
    private Animator _anim;
    private CameraHandler _cameraHandler;
    private PlayerLocomotion _playerLocomotion;
    
    public bool isInteracting;
    
    [Header("Player Flags")]
    public bool isSprinting;
    
    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

    private void Awake()
    {
        _cameraHandler = CameraHandler.Singleton;
    }

    private void Start()
    {
        _inputHandler = GetComponent<InputHandler>();
        _anim = GetComponentInChildren<Animator>();
        _playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        
        isInteracting = _anim.GetBool(IsInteracting);
        
        _inputHandler.TickInput(delta);
        _playerLocomotion.HandleMovement(delta);
        _playerLocomotion.HandleRollingAndSprinting(delta);
    }
    
    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        if (_cameraHandler == null) return;
        _cameraHandler.FollowTarget(delta);
        _cameraHandler.HandleCameraRotation(delta, _inputHandler.mouseX, _inputHandler.mouseY);
    }

    private void LateUpdate()
    {
        _inputHandler.rollFlag = false;
        _inputHandler.sprintFlag = false;
        isSprinting = _inputHandler.bInput;
    }
}
