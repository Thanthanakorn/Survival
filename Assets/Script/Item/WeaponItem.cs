using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
   public GameObject modelPrefab;
   public bool isUnarmed;

   [Header("One Handed Attack Animations")]
   public string ohLightAttack1;
   public string ohHeavyAttack1;
   
}
