using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemData")]
public class SkillData : ScriptableObject
{
    public enum ItemType
    {
        Torch,
        Rifle,
        Firebomb,
        Shoe,
        Glove,
        Heal,
        Coin
    }

    [Header("Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

    [Header("Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("Weapon")]
    public GameObject projectile;
}
