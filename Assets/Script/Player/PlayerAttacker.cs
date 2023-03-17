using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    private AnimatorHandler _animatorHandler;
    private InputHandler _inputHandler;

    public string lastAttack;
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");

    private void Awake()
    {
        _animatorHandler = GetComponentInChildren<AnimatorHandler>();
        _inputHandler = GetComponent<InputHandler>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (_inputHandler.comboFLag)
        {
            _animatorHandler.anim.SetBool(CanDoCombo, false);
            if (lastAttack == weapon.ohLightAttack1)
            {
                _animatorHandler.PlayTargetAnimation(weapon.ohLightAttack2, true);
            }

            else if (lastAttack == weapon.ohLightAttack2)
            {
                _animatorHandler.PlayTargetAnimation(weapon.ohLightAttack3, true);
            }
            
            else if (lastAttack == weapon.ohHeavyAttack1)
            {
                _animatorHandler.PlayTargetAnimation(weapon.ohHeavyAttack2, true);
            }
            else if (lastAttack == weapon.ohHeavyAttack2)
            {
                _animatorHandler.PlayTargetAnimation(weapon.ohHeavyAttack3, true);
            }
            
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        _animatorHandler.PlayTargetAnimation(weapon.ohLightAttack1, true);
        lastAttack = weapon.ohLightAttack1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        _animatorHandler.PlayTargetAnimation(weapon.ohHeavyAttack1, true);
        lastAttack = weapon.ohHeavyAttack1;
    }
}