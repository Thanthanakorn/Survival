using System.Collections;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private Transform _cameraObject;
    private InputHandler _inputHandler;
    private PlayerManager _playerManager;
    private PlayerStats _playerStats;
    private Animator _anim;
    public Vector3 moveDirection;
    

    [HideInInspector] public Transform myTransform;
    [HideInInspector] public AnimatorHandler animatorHandler;

    public new Rigidbody rigidbody;

    [Header("Ground & Air Detection Stats")] 
    [SerializeField] private float groundDetectionRayStartPoint = 0.5f;
    [SerializeField] private float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField] private float groundDirectionRayDistance = 0.2f;
    private LayerMask _ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")] [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rollingSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float fallingSpeed = 45f;
    
    [SerializeField] private float rollingDistance = 3f;
    [SerializeField] private float stepBackDistance = 1f;
    [SerializeField] private float stepBackSpeed = 5f;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        _playerManager = GetComponent<PlayerManager>();
        _inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        _playerStats = GetComponent<PlayerStats>();
        if (Camera.main != null) _cameraObject = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialize();

        _playerManager.isGrounded = true;
        _ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
    }

    #region Movement

    private Vector3 _normalVector;
    private Vector3 _targetPosition;

    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

    private void HandleRotation(float delta)
    {
        if (_playerStats.isDead)
            return;
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
        if (_playerManager.isAttacking)
        {
            return;
        }
        if (_inputHandler.rollFlag)
            return;

        if (_playerManager.isInteracting)
            return;

        moveDirection = _cameraObject.forward * _inputHandler.vertical;
        moveDirection += _cameraObject.right * _inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        var speed = movementSpeed;

        if (_inputHandler.sprintFlag && _inputHandler.moveAmount > 0.5)
        {
            speed = sprintSpeed;
            _playerManager.isSprinting = true;

            // Reduce the horizontal speed factor while sprinting (e.g., multiply by 0.5)
            float horizontalSpeedFactor = 0.5f;
            moveDirection.x *= horizontalSpeedFactor;
            moveDirection.z *= horizontalSpeedFactor;

            moveDirection *= speed;
        }
        else
        {
            if (_inputHandler.moveAmount < 0.5)
            {
                moveDirection *= movementSpeed;
                _playerManager.isSprinting = false;
            }
            else
            {
                moveDirection *= speed;
                _playerManager.isSprinting = false;
            }
        }

        var projectedVelocity = Vector3.ProjectOnPlane(moveDirection, _normalVector);
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
            
            moveDirection = _cameraObject.forward * _inputHandler.vertical;
            moveDirection += _cameraObject.right * _inputHandler.horizontal;

            if (_inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                moveDirection.Normalize();
                moveDirection *= rollingDistance;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;

                Vector3 targetPosition = myTransform.position + moveDirection;
                StartCoroutine(MoveOverSpeed(gameObject, targetPosition, rollingSpeed));
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
                moveDirection = -myTransform.forward;
                moveDirection.y = 0;
                moveDirection.Normalize();
                moveDirection *= stepBackDistance;

                Vector3 targetPosition = myTransform.position + moveDirection;
                StartCoroutine(MoveOverSpeed(gameObject, targetPosition, stepBackSpeed));
            }
        }

        
    }

    IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        float startTime = Time.time;
        float overTime = 1 / speed;
        Vector3 startPosition = objectToMove.transform.position;

        while (Time.time < startTime + overTime)
        {
            objectToMove.transform.position = Vector3.Lerp(startPosition, end, (Time.time - startTime) * speed);
            yield return null;
        }

        objectToMove.transform.position = end;
    }

    public void HandleFalling(float delta, Vector3 moveDirectionVector3)
    {
        _playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirectionVector3 = Vector3.zero;
        }

        if (_playerManager.isInAir)
        {
            rigidbody.AddForce(-Vector3.up * fallingSpeed);
            rigidbody.AddForce(moveDirectionVector3 * fallingSpeed / 10f);
        }

        Vector3 dir = moveDirectionVector3;
        dir.Normalize();
        origin += dir * groundDirectionRayDistance;

        _targetPosition = myTransform.position;
        
        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, _ignoreForGroundCheck))
        {
            _normalVector = hit.normal;
            Vector3 tp = hit.point;
            _playerManager.isGrounded = true;
            _targetPosition.y = tp.y;

            if (_playerManager.isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    Debug.Log("You were in the air for " + inAirTimer);
                    animatorHandler.PlayTargetAnimation("Landing", true);
                    inAirTimer = 0;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Locomotion", false);
                    inAirTimer = 0;
                }

                _playerManager.isInAir = false;
            }
        }
        else
        {
            if (_playerManager.isGrounded)
            {
                _playerManager.isGrounded = false;
            }

            if (_playerManager.isInAir == false)
            {
                if (_playerManager.isInteracting == false)
                {
                    animatorHandler.PlayTargetAnimation("Falling",true);
                }

                Vector3 vel = rigidbody.velocity;
                vel.Normalize();
                rigidbody.velocity = vel * (movementSpeed / 2);
                _playerManager.isInAir = true;
            }
        }

        if (_playerManager.isGrounded)
        {
            if (_playerManager.isInteracting || _inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, _targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = _targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (_playerManager.isInteracting) return;
        if (!_inputHandler.jumpInput) return;
        if (!(_inputHandler.moveAmount > 0)) return;
        moveDirection = _cameraObject.forward * _inputHandler.vertical;
        moveDirection += _cameraObject.right * _inputHandler.horizontal;
        animatorHandler.PlayTargetAnimation("Moving Jump", true);
        moveDirection.y = 0;
        var jumpRotation = Quaternion.LookRotation(moveDirection);
        myTransform.rotation = jumpRotation;
    }
    
    public void ResetVelocity()
    {
        rigidbody.velocity = Vector3.zero;
    }
    
    #endregion
}