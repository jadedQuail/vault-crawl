using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // This script functions as both the inventory and the inventory menu's controller
    GameObject player;

    // This is the actual inventory
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] List<GameObject> inventorySlots;
    [SerializeField] List<GameObject> inventoryImages;
    [SerializeField] GameObject playerWeaponImage;

    [SerializeField] Text attackText;
    [SerializeField] Text bowText;
    [SerializeField] Text hpText;

    [SerializeField] bool[] openSlotTracker = new bool[27];

    private int numberOfItems;
    private bool inventoryFull = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // Initialize the entire slot tracker to be "true" at first
        for (int i = 0; i < openSlotTracker.Length; i++)
        {
            openSlotTracker[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // The playerDamage is always updated; so constantly update the attack stat with it
        SetAttackStatText(player.GetComponent<PlayerProgression>().GetPlayerDamage().ToString());

        // The bowDamage is always updated; so constantly update the bow stat with it
        SetBowStatText(player.GetComponent<PlayerProgression>().GetCurrentBowDamage().ToString());

        // The maxHealth is always updated; so constantly update the HP stat with it
        SetHPStatText(player.GetComponent<Health>().GetMaxHealth().ToString());

        // Check and see if the inventory is full
        if (numberOfItems >= 24)
        {
            inventoryFull = true;
        }
        else
        {
            inventoryFull = false;
        }
    }

    // PUBLIC FUNCTIONS

    // Get the inventory slots
    public List<GameObject> GetInventorySlots()
    {
        return inventorySlots;
    }

    // Set the inventory slots
    public void SetInventorySlots(List<GameObject> slots)
    {
        inventorySlots = slots;
    }

    // Get the inventory images
    public List<GameObject> GetInventoryImages()
    {
        return inventoryImages;
    }

    // Set the inventory images
    public void SetInventoryImages(List<GameObject> images)
    {
        inventoryImages = images;
    }

    // Function that finds the next open inventory image and puts a sprite there
    public void FillInventoryImage(Sprite sprite, Weapon weapon)
    {
        foreach (GameObject imageObject in inventoryImages)
        {
            if (!imageObject.GetComponent<ItemIcon>().GetHasASprite())
            {
                // Give the spot an index value of whatever the next open slot is, according to the slot tracker
                int nextOpenIndex = GetNextOpenSlotInTracker();
                imageObject.GetComponent<ItemIcon>().SetLocationIndex(nextOpenIndex);

                // Call that index location "occupied"
                SetSlotTrackerAt(nextOpenIndex, false);

                // Give this image our sprite (which is the subicon)
                imageObject.GetComponent<ItemIcon>().SetSubIconSprite(sprite);

                // If the subicon needs to be flipped, flip it
                if (weapon.GetSpriteFlipped())
                {
                    imageObject.GetComponent<ItemIcon>().FlipSubIcon();
                }

                // Set the sub icon's angle
                imageObject.GetComponent<ItemIcon>().SetSubIconAngle(weapon.GetSpriteAngle());

                // Store the subIcon's flip status
                imageObject.GetComponent<ItemIcon>().SetIsFlipped(weapon.GetSpriteFlipped());

                // Active this game object
                imageObject.SetActive(true);

                // Set the image's weapon type to be the weapon type of the pick up
                imageObject.GetComponent<ItemIcon>().SetWeaponType(weapon.GetWeaponType());

                // Set the image's weapon damage to be the weapon damage of the pick up
                imageObject.GetComponent<ItemIcon>().SetWeaponDamage(weapon.GetWeaponDamage());

                // Set the image's weapon sprite type
                imageObject.GetComponent<ItemIcon>().SetWeaponSpriteType(weapon.GetWeaponSpriteType());

                // Set the image's weapon name and weapon description
                imageObject.GetComponent<ItemIcon>().SetWeaponName(weapon.GetWeaponName());
                imageObject.GetComponent<ItemIcon>().SetWeaponDescription(weapon.GetWeaponDescription());
                imageObject.GetComponent<ItemIcon>().SetWeaponAttributes(weapon.GetWeaponAttributes());

                // Set the image's weapon header and content
                imageObject.GetComponent<TooltipTrigger>().SetHeader(weapon.GetWeaponName());
                imageObject.GetComponent<TooltipTrigger>().SetContent(weapon.GetWeaponDescription());
                imageObject.GetComponent<TooltipTrigger>().SetAttributes(weapon.GetWeaponAttributes());

                // Set the image as "having found a sprite"
                imageObject.GetComponent<ItemIcon>().SetHasASprite(true);

                // Set the image's buy and sell values
                imageObject.GetComponent<ItemIcon>().SetBuyValue(weapon.GetBuyValue());
                imageObject.GetComponent<ItemIcon>().SetSellValue(weapon.GetSellValue());

                // Set the image's talisman information, if applicable
                if (weapon.GetIsTalisman())
                {
                    imageObject.GetComponent<ItemIcon>().SetTalismanType(weapon.GetTalismanBuff().Item1);
                    imageObject.GetComponent<ItemIcon>().SetTalismanBuff(weapon.GetTalismanBuff().Item2);
                }

                // Assign this image to a location
                imageObject.GetComponent<ItemIcon>().AssignImageToSpot();

                // Stop searching
                break;
            }
        }
    }

    // Function that removes an item from the player's inventory, based on the index number
    public void RemoveInventoryImage(int locIndex)
    {
        foreach (GameObject imageObject in inventoryImages)
        {
            // Identify this item, and ensure that it's actually active (i.e. the slot tracker says it's not open)
            if (imageObject.GetComponent<ItemIcon>().GetLocationIndex() == locIndex && GetSlotTrackerAt(locIndex) == false)
            {
                // Nullify the icon, reset the slot
                imageObject.GetComponent<ItemIcon>().ResetSubIconFlip();
                imageObject.GetComponent<ItemIcon>().SetSubIconSprite(null);
                imageObject.SetActive(false);
                imageObject.GetComponent<ItemIcon>().SetHasASprite(false);

                // Indicate in the slot tracker that this slot is open
                SetSlotTrackerAt(locIndex, true);

                // Open up the icon's slot
                imageObject.GetComponent<ItemIcon>().GetCurrentSlot().GetComponent<ItemSlot>().SetSlotOpen(true);
            }
        }
    }

    // Function that allows/denies raycasts to go through every inventory image
    public void ToggleImageRaycasts(bool value)
    {
        foreach (GameObject imageObject in inventoryImages)
        {
            imageObject.GetComponent<CanvasGroup>().blocksRaycasts = value;
        }
    }

    // Function that fills the sword image for the player image in the inventory menu
    public void FillPlayerWeaponImage(Sprite sprite, int angle)
    {
        playerWeaponImage.GetComponent<Image>().sprite = sprite;
        playerWeaponImage.SetActive(true);

        // Adjust the weapon's angle so it makes sense for the player weapon image
        angle += 135;

        // Adjust the angle (with the new angle adjustment)
        playerWeaponImage.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Function that deactivates the sword image for the player image in the inventory menu
    public void DeactivatePlayerWeaponImage()
    {
        playerWeaponImage.SetActive(false);
    }

    // Function that sets the attack stat text
    public void SetAttackStatText(string theText)
    {
        attackText.text = theText;
    }

    // Function that sets the bow stat text
    public void SetBowStatText(string theText)
    {
        bowText.text = theText;
    }

    // Function that sets the HP stat text
    public void SetHPStatText(string theText)
    {
        hpText.text = theText;
    }

    // Function that gets the player weapon image
    public GameObject GetPlayerWeaponImage()
    {
        return playerWeaponImage;
    }

    // Function that sets the player weapon image
    public void SetPlayerWeaponImage(GameObject image)
    {
        playerWeaponImage = image;
    }

    // Function for getting the number of items in the inventory
    public int GetNumberOfItems()
    {
        return numberOfItems;
    }

    // Function for setting the number of items in the inventory
    public void SetNumberOfItems(int number)
    {
        numberOfItems = number;
    }

    // Function for incrementing the number of items in the inventory
    public void IncrementNumberOfItems()
    {
        numberOfItems += 1;
    }

    // Function for decrementing the number of items in the inventory
    public void DecrementNumberOfItems()
    {
        numberOfItems -= 1;
    }

    // Function for getting whether or not the inventory is full
    public bool GetInventoryFull()
    {
        return inventoryFull;
    }

    // Function for getting whether or not the inventory slot at a particular index is occupied
    public bool GetSlotTrackerAt(int i)
    {
        return openSlotTracker[i];
    }

    // Function for setting whether or not the inventory slot at a particular index is occupied
    public void SetSlotTrackerAt(int i, bool value)
    {
        openSlotTracker[i] = value;
    }

    // Function that finds the next open spot in the slot tracker
    public int GetNextOpenSlotInTracker()
    {
        for (int i = 0; i < openSlotTracker.Length; i++)
        {
            if (openSlotTracker[i] == true)
            {
                return i;
            }
        }

        // This means we have an error
        return 999;
    }
}
