using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    private PlayerManager _playerManager;
    public Animator anim;
    private PlayerLocomotion _playerLocomotion;
    private int _vertical;
    private int _horizontal;
    public bool canRotate;

    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");

    public void Initialize()
    {
        anim = GetComponent<Animator>();
        _playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        _playerManager = GetComponentInParent<PlayerManager>();
        _vertical = Animator.StringToHash("Vertical");
        _horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        #region Vertical

        float v = verticalMovement switch
        {
            > 0 and < 0.55f => 0.5f,
            > 0.55f => 1,
            < 0 and > -0.55f => -0.5f,
            < -0.55f => -1,
            _ => 0
        };

        #endregion

        #region Horizontal

        var h = horizontalMovement switch
        {
            > 0 and < 0.55f => 1,
            < 0 and > -0.55f => -0.5f,
            < -0.55f => -1,
            _ => 0
        };

        #endregion

        if (isSprinting)
        {
            v = 2;
            h = horizontalMovement;
        }

        anim.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
    }

    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }

    private void OnAnimatorMove()
    {
        if (_playerManager.isAttacking == false)
            return;
    
        var delta = Time.deltaTime;
        _playerLocomotion.rigidbody.drag = 0;
        var deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        var velocity = deltaPosition / delta;
        _playerLocomotion.rigidbody.velocity = velocity;
    }


    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool(IsInteracting, isInteracting);
        anim.CrossFade(targetAnim, 0.2f);
        
    }
    
    public void PlayTargetAttackingAnimation(string targetAnim, bool isAttacking)
    {
        anim.applyRootMotion = isAttacking;
        anim.SetBool(IsAttacking, isAttacking);
        anim.CrossFade(targetAnim, 0.2f);
        
        if (isAttacking)
        {
            _playerLocomotion.ResetVelocity();
        }
    }

    public void EnableCombo()
    {
        anim.SetBool(CanDoCombo, true);
    }

    public void DisableCombo()
    {
        anim.SetBool(CanDoCombo, false);
    }

    public void EnableIsInteracting()
    {
        anim.SetBool(IsInteracting,true);
    }
    
    public void DisableIsInteracting()
    {
        anim.SetBool(IsInteracting, false);
    }
    
    public void EnableIsAttacking()
    {
        anim.SetBool(IsAttacking, true);
    }
    
    public void DisableIsAttacking()
    {
        anim.SetBool(IsAttacking, false);
    }
}