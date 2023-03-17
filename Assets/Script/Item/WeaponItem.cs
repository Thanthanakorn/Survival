using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
   public GameObject modelPrefab;
   public bool isUnarmed;

   [Header("One Handed Attack Animations")]
   public string ohLightAttack1;
   public string ohLightAttack2;
   public string ohLightAttack3;
   
   public string ohHeavyAttack1;
   public string ohHeavyAttack2;
   public string ohHeavyAttack3;

   [Header("Stamina Costs")] 
   public int baseStamina = 0;
   public float lightAttackMultiplier;
   public float heavyAttackMultiplier;

}
