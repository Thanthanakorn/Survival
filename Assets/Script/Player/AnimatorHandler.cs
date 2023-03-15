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
        if (_playerManager.isInteracting == false)
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
}