using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private Transform _cameraObject;
    private InputHandler _inputHandler;
    private PlayerManager _playerManager;
    private Vector3 _moveDirection;

    [HideInInspector] public Transform myTransform;
    [HideInInspector] public AnimatorHandler animatorHandler;

    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Stats")] [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rollingSpeed;
    [SerializeField] private float sprintSpeed;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        _playerManager = GetComponent<PlayerManager>();
        _inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        if (Camera.main != null) _cameraObject = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialize();
    }

    #region Movement

    private Vector3 _normalVector;
    private Vector3 _targetPosition;

    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

    private void HandleRotation(float delta)
    {
        //var moveOverride = _inputHandler.moveAmount;
    
        var targetDir = _cameraObject.forward * _inputHandler.vertical;
        targetDir += _cameraObject.right * _inputHandler.horizontal;
        
        targetDir.Normalize();
        targetDir.y = 0;
    
        if (targetDir == Vector3.zero)
        {
            targetDir = myTransform.forward;
        }
    
        float rs = rotationSpeed;
    
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
    
        myTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta)
    {
        if (_inputHandler.rollFlag)
            return;

        _moveDirection = _cameraObject.forward * _inputHandler.vertical;
        _moveDirection += _cameraObject.right * _inputHandler.horizontal;
        _moveDirection.Normalize();
        _moveDirection.y = 0;

        var speed = movementSpeed;

        if (_inputHandler.sprintFlag)
        {
            speed = sprintSpeed;
            _playerManager.isSprinting = true;
            _moveDirection *= speed;
        }
        else
        {
            _moveDirection *= speed;
        }

        var projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
        rigidbody.velocity = projectedVelocity;

        animatorHandler.UpdateAnimatorValues(_inputHandler.moveAmount, 0, _playerManager.isSprinting);

        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollingAndSprinting(float delta)
    {
        if (animatorHandler.anim.GetBool(IsInteracting))
            return;

        if (_inputHandler.rollFlag)
        {
            _moveDirection = _cameraObject.forward * _inputHandler.vertical;
            _moveDirection += _cameraObject.right * _inputHandler.horizontal;

            if (_inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                _moveDirection.y = 0;
                _moveDirection.Normalize();
                _moveDirection *= rollingSpeed; 
                Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    #endregion
}