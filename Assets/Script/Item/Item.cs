using UnityEngine;
using UnityEngine.Serialization;

public class Item : ScriptableObject
{
    [Header("Item Information")] 
    public Sprite itemIcon;
    public string itemName;

    public string standPose;

    [Header("Attack Animation")] 
    public string lightAttack1;
    public string lightAttack2;
    public string lightAttack3;
   
    public string heavyAttack1;
    public string heavyAttack2;
    public string heavyAttack3;
}
