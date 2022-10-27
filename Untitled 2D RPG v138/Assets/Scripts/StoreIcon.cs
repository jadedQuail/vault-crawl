using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StoreIcon : MonoBehaviour
{
    [SerializeField] GameObject checkmark;
    [SerializeField] Image subIcon;

    // True means it is a buy icon; false means it is a sell icon
    [SerializeField] bool isBuyIcon;

    int spriteAngle;
    bool spriteFlipped;

    // The type and the damage of whatever is in this inventory slot
    string itemType;
    int itemDamage;

    // The buy and sell value for whatever item is in this inventory slot
    int buyValue;
    int sellValue;

    // More info that needs to be held here for when the item is selected.
    string itemName;
    string itemDescription;
    string itemAttributes;
    string spriteType;

    // Store the type of supply item this is, if applicable
    string supplyItem = "none";
    int supplyItemQuantity = 0;

    // Store info about talismans
    bool isTalisman;
    string talismanBuffType;
    int talismanBuffValue;

    // Store the item's index value
    int locationIndex;

    // A boolean for storing whether or not this checkmark is active
    bool checkmarkActivated = false;

    GameObject hud;

    private void Start()
    {
        hud = GameObject.FindWithTag("HUD");
    }

    // Function for activating the checkmark - this only applies to Buy icons
    public void ActivateCheckmark()
    {
        // Disable all other checkmarks if this is a buy icon
        hud.GetComponent<StoreController>().DeactivateAllCheckmarks();

        // Activate this checkmark
        checkmark.SetActive(true);

        // This becomes the selected item, send its info to be held in the store controller.

        // Create the item
        SelectedItem theItem = new SelectedItem();
        theItem.FillSelectedItemValues(subIcon.sprite, itemType, itemDamage, itemName, itemDescription, itemAttributes, spriteType, spriteFlipped, spriteAngle, buyValue, sellValue, locationIndex, this, supplyItem, supplyItemQuantity, isTalisman, talismanBuffType, talismanBuffValue);

        // Send the item to the Store Controller
        hud.GetComponent<StoreController>().SetSelectedItem(theItem);

        // Set this object's sprite as the buy menu image
        hud.GetComponent<StoreController>().SetMenuImage(isBuyIcon, subIcon.sprite, spriteAngle, spriteFlipped, itemType, itemDamage.ToString(), buyValue, sellValue, supplyItem);
    }

    // Function for toggling the checkmark - this only applies to sell icons
    public void ToggleCheckmark()
    {
        // If the checkmark is inactive, activate it
        if (!checkmarkActivated)
        {
            checkmark.SetActive(true);
            checkmarkActivated = true;
        }
        else // If the checkmark is active, inactivate it
        {
            checkmark.SetActive(false);
            checkmarkActivated = false;
        }

        if (checkmarkActivated) // Add item to list
        {
            // Create a new selected item
            SelectedItem newItem = new SelectedItem();
            newItem.FillSelectedItemValues(subIcon.sprite, itemType, itemDamage, itemName, itemDescription, itemAttributes, spriteType, spriteFlipped, spriteAngle, buyValue, sellValue, locationIndex, this, supplyItem, supplyItemQuantity, isTalisman, talismanBuffType, talismanBuffValue);

            // Add it to the list
            hud.GetComponent<StoreController>().AddToSelectedItems(newItem);

            // Add value of this item to the total value
            hud.GetComponent<StoreController>().IncrementTotalSellValue(sellValue);
            hud.GetComponent<StoreController>().ResetValueText();

            // Set this object's sprite as the sell menu image
            // hud.GetComponent<StoreController>().SetMenuImage(isBuyIcon, subIcon.sprite, spriteAngle, spriteFlipped, itemType, itemDamage.ToString(), buyValue, sellValue, supplyItem);
        }
        else // Remove item from list
        {
            List<SelectedItem> selectedItems = hud.GetComponent<StoreController>().GetSelectedItems();

            // Go through all the selected items
            foreach (SelectedItem item in selectedItems)
            {
                // If the location index of the item stored in this storeIcon is the same as
                // one in the list, then remove it
                if (locationIndex == item.GetLocationIndex())
                {
                    hud.GetComponent<StoreController>().RemoveFromSelectedItems(item);

                    // Decrement total sell value
                    hud.GetComponent<StoreController>().DecrementTotalSellValue(item.GetSellValue());
                    hud.GetComponent<StoreController>().ResetValueText();

                    break;
                }
            }
        }
    }

    // Function for setting subIcon
    public void SetSubIcon(Sprite sprite)
    {
        subIcon.sprite = sprite;
    }

    // Function for flipping the subIcon
    public void FlipSubIcon()
    {
        subIcon.GetComponent<RectTransform>().localScale = new Vector2(-0.7f, 0.7f);
    }

    // Function for resetting the flip on the subIcon
    public void ResetFlipSubIcon()
    {
        subIcon.GetComponent<RectTransform>().localScale = new Vector2(0.7f, 0.7f);
    }

    // Function for setting the subIcon angle
    public void SetSubIconAngle(int angle)
    {
        subIcon.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Store the sprite angle for when it gets moved to the Buy Image
    public void StoreSpriteAngle(int angle)
    {
        spriteAngle = angle;
    }

    // Store the boolean that indicates whether or not we flip the sprite for when it gets moved to the Buy Image
    public void StoreIsFlipped(bool value)
    {
        spriteFlipped = value;
    }

    // Store the item name
    public void StoreItemType(string type)
    {
        itemType = type;
    }

    // Store the item damage
    public void StoreItemDamage(int damage)
    {
        itemDamage = damage;
    }

    // Store the buy value
    public void StoreBuyValue(int value)
    {
        buyValue = value;
    }

    public void StoreSellValue(int value)
    {
        sellValue = value;
    }

    public void StoreItemName(string name)
    {
        itemName = name;
    }

    public void StoreItemDescription(string description)
    {
        itemDescription = description;
    }

    public void StoreItemAttributes(string attributes)
    {
        itemAttributes = attributes;
    }

    public void StoreSpriteType(string type)
    {
        spriteType = type;
    }

    public void StoreLocationIndex(int index)
    {
        locationIndex = index;
    }

    public void StoreIsTalisman(bool value)
    {
        isTalisman = value;
    }

    public void StoreTalismanBuffType(string value)
    {
        talismanBuffType = value;
    }

    public void StoreTalismanBuffValue(int value)
    {
        talismanBuffValue = value;
    }

    public void SetCheckmarkActivated(bool value)
    {
        checkmark.SetActive(value);
        checkmarkActivated = value;
    }

    // Get whether or not this checkmark is activated
    public bool GetCheckmarkActivated()
    {
        return checkmarkActivated;
    }

    public void SetSupplyItem(string value)
    {
        if (value == "arrow" || value == "potion")
        {
            supplyItem = value;
        }
        else
        {
            supplyItem = "none";
        }
    }

    // Function that gets the sell value stored in this icon
    public int GetSellValue()
    {
        return sellValue;
    }
}
