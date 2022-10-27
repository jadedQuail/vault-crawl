using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Card", menuName = "Item Card")]
public class ItemCard : ScriptableObject
{
    // Item statistics
    public string weaponName;
    public int weaponDamage;
    public string weaponType;
    public string weaponDescription;

    [TextArea]
    public string weaponAttributes;

    public Sprite weaponSprite;
    public string weaponSpriteType;
    public bool spriteFlipped = false;
    public int spriteAngle;

    // Buy and sell value
    public int buyValue;
    public int sellValue;

    // Talisman details
    public bool isTalisman;
    public string talismanType;

    public int fixedAttackBuff;
    public int fixedBowBuff;
    public int fixedHPBuff;
    public int percentAttackBuff;
    public int percentBowBuff;
    public int percentHPBuff;
    public int percentSpeedBuff;
}
