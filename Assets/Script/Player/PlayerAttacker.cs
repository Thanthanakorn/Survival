using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    private AnimatorHandler _animatorHandler;

    private void Awake()
    {
        _animatorHandler = GetComponentInChildren< AnimatorHandler>();
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        _animatorHandler.PlayTargetAnimation(weapon.ohLightAttack1, true);
    }

    public void HandleHeaveAttack(WeaponItem weapon)
    {
        _animatorHandler.PlayTargetAnimation(weapon.ohHeavyAttack1, true);
    }
}
