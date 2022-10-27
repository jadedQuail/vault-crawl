using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItem
{
    // This class pretty much just holds information
    // for items selected in the store menu

    // Selected item attributes
    Sprite selectedItemSprite;
    string selectedWeaponType;
    int selectedWeaponDamage;
    string selectedWeaponName;
    string selectedWeaponDescription;
    string selectedWeaponAttributes;
    string selectedWeaponSpriteType;
    bool selectedIsFlipped;
    int selectedAngle;
    int selectedBuyValue;
    int selectedSellValue;
    bool selectedIsTalisman;
    string selectedTalismanBuffType;
    int selectedTalismanBuffValue;
    int selectedLocationIndex;

    string selectedSupplyItem;
    int selectedSupplyItemQuantity;

    StoreIcon selectedStoreIcon;

    // PUBLIC FUNCTIONS

    // Function for filling out the values of a selected item
    public void FillSelectedItemValues(Sprite sprite, string weaponType, int weaponDamage, string weaponName, string weaponDescription, string weaponAttributes, string spriteType, bool isFlipped, int angle, int buyValue, int sellValue, int locationIndex, StoreIcon storeIcon, string supplyItem, int supplyItemQuantity, bool isTalis, string talisBuffType, int talisBuffVal)
    {
        selectedItemSprite = sprite;
        selectedWeaponType = weaponType;
        selectedWeaponDamage = weaponDamage;
        selectedWeaponName = weaponName;
        selectedWeaponDescription = weaponDescription;
        selectedWeaponAttributes = weaponAttributes;
        selectedWeaponSpriteType = spriteType;
        selectedIsFlipped = isFlipped;
        selectedAngle = angle;
        selectedBuyValue = buyValue;
        selectedSellValue = sellValue;
        selectedLocationIndex = locationIndex;

        selectedStoreIcon = storeIcon;

        selectedSupplyItem = supplyItem;
        selectedSupplyItemQuantity = supplyItemQuantity;

        selectedIsTalisman = isTalis;
        selectedTalismanBuffType = talisBuffType;
        selectedTalismanBuffValue = talisBuffVal;
    }

    // Function for getting the item's sprite
    public Sprite GetItemSprite()
    {
        return selectedItemSprite;
    }

    // Function for getting the weapon's type
    public string GetWeaponType()
    {
        return selectedWeaponType;
    }

    // Function for getting the weapon's damage
    public int GetWeaponDamage()
    {
        return selectedWeaponDamage;
    }

    // Function for getting the weapon's name
    public string GetWeaponName()
    {
        return selectedWeaponName;
    }

    // Function for getting the weapon's description
    public string GetWeaponDescription()
    {
        return selectedWeaponDescription;
    }

    // Function for getting the weapon's attributes
    public string GetWeaponAttributes()
    {
        return selectedWeaponAttributes;
    }

    // Function for getting the weapon's sprite type
    public string GetSpriteType()
    {
        return selectedWeaponSpriteType;
    }

    // Function for getting whether or not the sprite is flipped
    public bool GetIsFlipped()
    {
        return selectedIsFlipped;
    }

    // Function for getting the sprite angle
    public int GetAngle()
    {
        return selectedAngle;
    }

    // Function for getting the weapon's buy value
    public int GetBuyValue()
    {
        return selectedBuyValue;
    }

    // Function for getting the weapon's sell value
    public int GetSellValue()
    {
        return selectedSellValue;
    }

    // Function for getting the location index 
    public int GetLocationIndex()
    {
        return selectedLocationIndex;
    }

    // Function for getting the selected store icon
    public StoreIcon GetStoreIcon()
    {
        return selectedStoreIcon;
    }

    // Function for getting the supply item status of the selected item
    public string GetSupplyItem()
    {
        return selectedSupplyItem;
    }

    // Function for getting the supply item quantity (if applicable)
    public int GetSupplyItemQuantity()
    {
        return selectedSupplyItemQuantity;
    }

    // Function for getting whether or not the selected item is a talisman
    public bool GetIsTalisman()
    {
        return selectedIsTalisman;
    }

    // Function for getting the selected item's talisman buff type, if it's a talisman
    public string GetTalismanBuffType()
    {
        return selectedTalismanBuffType;
    }

    // Function for getting the selected item's talisman buff value, if it's a talisman
    public int GetTalismanBuffValue()
    {
        return selectedTalismanBuffValue;
    }
}
