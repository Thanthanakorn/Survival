using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    private AnimatorHandler _animatorHandler;
    private InputHandler _inputHandler;
    private WeaponSlotManager _weaponSlotManager;
    public string lastAttack;
    private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");

    private void Awake()
    {
        _animatorHandler = GetComponentInChildren< AnimatorHandler>();
        _weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        _inputHandler = GetComponent<InputHandler>();
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        _weaponSlotManager.attackingWeapon = weapon;
        _animatorHandler.PlayTargetAttackingAnimation(weapon.ohLightAttack1, true);
        lastAttack = weapon.ohLightAttack1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        _weaponSlotManager.attackingWeapon = weapon;
        _animatorHandler.PlayTargetAttackingAnimation(weapon.ohHeavyAttack1, true);
        lastAttack = weapon.ohHeavyAttack1;
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (_inputHandler.comboFlag)
        {
            _animatorHandler.anim.SetBool(CanDoCombo, false);
            if (lastAttack == weapon.ohLightAttack1)
            {
                _animatorHandler.PlayTargetAttackingAnimation(weapon.ohLightAttack2, true);
                lastAttack = weapon.ohLightAttack2;
            }
            else if (lastAttack == weapon.ohLightAttack2)
            {
                _animatorHandler.PlayTargetAttackingAnimation(weapon.ohLightAttack3, true);
            }
            else if (lastAttack == weapon.ohHeavyAttack1)
            {
                _animatorHandler.PlayTargetAttackingAnimation(weapon.ohHeavyAttack2, true);
                lastAttack = weapon.ohHeavyAttack2;
            }
            else if (lastAttack == weapon.ohHeavyAttack2)
            {
                _animatorHandler.PlayTargetAttackingAnimation(weapon.ohHeavyAttack3, true);
            }
        }
    }
}
