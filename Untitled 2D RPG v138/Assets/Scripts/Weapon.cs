using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Weapon statistics
    [SerializeField] int weaponDamage;
    [SerializeField] string weaponType;
    [SerializeField] string weaponName;

    [TextArea]
    [SerializeField] string weaponDescription;

    [TextArea]
    [SerializeField] string weaponAttributes;

    // Buy and sell value
    [SerializeField] int buyValue;
    float sellValue_float;
    int sellValue;

    // Sprite specifications
    [SerializeField] string weaponSpriteType;
    [SerializeField] bool spriteFlipped = false;
    [SerializeField] int spriteAngle;

    // Talisman specifications
    [SerializeField] bool isTalisman;
    [SerializeField] string talismanType;

    [SerializeField] int fixedAttackBuff;
    [SerializeField] int fixedBowBuff;
    [SerializeField] int fixedHPBuff;
    [SerializeField] int percentAttackBuff;
    [SerializeField] int percentBowBuff;
    [SerializeField] int percentHPBuff;
    [SerializeField] int percentSpeedBuff;

    private void Start()
    {
        // Set the sell value of the weapon
        sellValue_float = buyValue * 0.75f;
        sellValue = (int)Math.Round(sellValue_float, 0);
    }

    // PUBLIC FUNCTIONS

    // Function that acts an a constructor for this class; initializes all the values, for code-initialized Weapon objects
    public void Initialize(string type, int damage, string name, string desc, string attributes, string spriteType, bool flipBool, int angle, int buyVal, int sellVal, bool isTalis, string talisBuffType, int talisBuffVal)
    {
        weaponType = type;
        weaponDamage = damage;
        weaponName = name;
        weaponDescription = desc;
        weaponAttributes = attributes;
        weaponSpriteType = spriteType; 
        spriteFlipped = flipBool;
        spriteAngle = angle;
        buyValue = buyVal;
        sellValue = sellVal;
        isTalisman = isTalis;
        talismanType = talisBuffType;
        SetTalismanBuff(talismanType, talisBuffVal);
    }

    // Function that returns the weapon damage
    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    // Function for getting the weapon type
    public string GetWeaponType()
    {
        return weaponType;
    }

    // Function for getting the weapon name
    public string GetWeaponName()
    {
        return weaponName;
    }

    // Function for getting the weapon description
    public string GetWeaponDescription()
    {
        return weaponDescription;
    }

    // Function for getting the weapon attributes
    public string GetWeaponAttributes()
    {
        return weaponAttributes;
    }

    // Function for getting the weapon sprite type
    public string GetWeaponSpriteType()
    {
        return weaponSpriteType;
    }

    // Function for getting whether or not the weapon sprite needs to be flipped
    public bool GetSpriteFlipped()
    {
        return spriteFlipped;
    }

    // Function for getting the sprite's angle
    public int GetSpriteAngle()
    {
        return spriteAngle;
    }

    // Function for getting the buy value of the item
    public int GetBuyValue()
    {
        return buyValue;
    }

    // Function for getting the sell value of the item
    public int GetSellValue()
    {
        return sellValue;
    }

    // Function for getting the actual sprite of the GameObject
    public Sprite GetWeaponSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    // Function for getting whether or not this weapon is a Talisman
    public bool GetIsTalisman()
    {
        return isTalisman;
    }

    public (string, int) GetTalismanBuff()
    {
        int buffToReturn;

        switch (talismanType)
        {
            case "fixed_attack":
                buffToReturn = fixedAttackBuff;
                break;
            case "fixed_bow":
                buffToReturn = fixedBowBuff;
                break;
            case "fixed_HP":
                buffToReturn = fixedHPBuff;
                break;
            case "percent_attack":
                buffToReturn = percentAttackBuff;
                break;
            case "percent_bow":
                buffToReturn = percentBowBuff;
                break;
            case "percent_HP":
                buffToReturn = percentHPBuff;
                break;
            case "percent_speed":
                buffToReturn = percentSpeedBuff;
                break;
            default:
                return ("999", 999);
        }

        // Return the talisman type and the buff itself
        return (talismanType, buffToReturn);
    }

    // Function that sets the talisman buff (from an item bought, basically)
    public void SetTalismanBuff(string type, int buff)
    {
        switch (type)
        {
            case "fixed_attack":
                fixedAttackBuff = buff;
                break;
            case "fixed_bow":
                fixedBowBuff = buff;
                break;
            case "fixed_HP":
                fixedHPBuff = buff;
                break;
            case "percent_attack":
                percentAttackBuff = buff;
                break;
            case "percent_bow":
                percentBowBuff = buff;
                break;
            case "percent_HP":
                percentHPBuff = buff;
                break;
            case "percent_speed":
                percentSpeedBuff = buff;
                break;
            default:
                break;
        }
    }
}