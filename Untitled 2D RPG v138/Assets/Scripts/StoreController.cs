using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreController : MonoBehaviour
{
    [Header("Checkmarks")]
    [SerializeField] List<GameObject> buyCheckmarks;
    [SerializeField] List<GameObject> sellCheckmarks;

    [Header("Store Images")]
    [SerializeField] List<GameObject> storeImages;
    [SerializeField] Image buyMenuImage;
    [SerializeField] List<GameObject> sellStoreImages;
    [SerializeField] Image sellMenuImage;

    [Header("Buy Texts")]
    [SerializeField] Text typeText;
    [SerializeField] Text descriptionTitle;
    [SerializeField] Text descriptionText;
    [SerializeField] Text costText;

    [Header("Sell Texts")]
    [SerializeField] Text sellTypeText;
    [SerializeField] Text sellDescriptionText;
    [SerializeField] Text valueText;

    [Header("Buy Dialogue Items")]
    [SerializeField] Text buyDialogueText_Single;
    [SerializeField] Text buyDialogueText_Multiple;
    [SerializeField] Button buy_yesButton;
    [SerializeField] Button buy_noButton;
    [SerializeField] Text buyQuantityText;
    [SerializeField] Text buyCostText;
    [SerializeField] Text inInventoryCount;
    [SerializeField] Image inInventoryImage;

    [Header("Sell Dialogue Items")]
    [SerializeField] Text sellDialogueText;
    [SerializeField] Button sell_yesButton;
    [SerializeField] Button sell_noButton;

    [Header("Buy Menu - Arrows")]
    [SerializeField] Sprite arrowSprite;
    [SerializeField] int arrowSubIconAngle = 135;
    [SerializeField] bool arrowIsFlipped = false;
    [SerializeField] string arrowSpriteType = "Straight";
    [SerializeField] string arrowItemType = "Arrow";
    [SerializeField] int arrowBuyValue = 5;
    [SerializeField] string arrowItemName = "Arrows";
    [SerializeField] string arrowDescription = "Ammunition for your bow and arrow.";

    // Single selected item (for buy menu)
    SelectedItem theSelectedItem;

    // Weapon object, basically to cast the SelectedItem as a Weapon to pass to inventory, when necessary
    Weapon selectedWeapon;

    // List of selected items (for sell menu)
    List<SelectedItem> selectedItems = new List<SelectedItem>();

    // Stores the value of all the items the player is selling
    int totalSellValue;

    int inventorySpots = 24;

    int buyQuantityValue = 0;

    GameObject master;
    GameObject gameController;
    GameObject player;

    bool atConfirmationDialogue = false;

    private void Start()
    {
        // Unity bug - set false on start
        arrowIsFlipped = false;

        master = GameObject.FindWithTag("Master");
        gameController = GameObject.FindWithTag("GameController");
        player = GameObject.FindWithTag("Player");

        // Initialize at 10 for now
        SetBuyInventory(10);

        // Initialize the buy quantity text
        buyQuantityText.text = buyQuantityValue.ToString();
    }

    private void Update()
    {
        // If we're at the confirmation dialogue, let the player click to move on (for both buy and sell)
        if (atConfirmationDialogue)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // Close the dialogue
                GetComponent<HUD>().CloseBuyDialogue();
                GetComponent<HUD>().CloseSellDialogue();
                atConfirmationDialogue = false;

                // Re-enable the yes and no buttons
                Buy_EnableYesButton();
                Buy_EnableNoButton();
                Sell_EnableYesButton();
                Sell_EnableNoButton();
            }
        }
    }

    // Function that formats money text
    public string FormatMoneyText(int money)
    {
        string moneyString = money.ToString();

        // Add commas if the money is above $1,000
        if (money >= 100000)
        {
            moneyString = "$" + moneyString.Substring(0, 3) + "," + moneyString.Substring(3);
        }
        else if (money >= 10000)
        {
            moneyString = "$" + moneyString.Substring(0, 2) + "," + moneyString.Substring(2);
        }
        else if (money >= 1000)
        {
            moneyString = "$" + moneyString.Substring(0, 1) + "," + moneyString.Substring(1);
        }
        else
        {
            moneyString = "$" + moneyString;
        }

        return moneyString;
    }

    // Function that sets up the shop arrow
    private void SetShopArrow(GameObject givenStoreImage)
    {
        // Set the icon
        givenStoreImage.GetComponent<StoreIcon>().SetSubIcon(arrowSprite);

        // Adjust the sprite angle
        givenStoreImage.GetComponent<StoreIcon>().SetSubIconAngle(arrowSubIconAngle);

        // Manually activate
        givenStoreImage.SetActive(true);

        // Store the sprite angle and the flip status in the image (and the sprite type)
        // (for later when the image is highlighted as the Buy Image)
        givenStoreImage.GetComponent<StoreIcon>().StoreSpriteAngle(arrowSubIconAngle);
        givenStoreImage.GetComponent<StoreIcon>().StoreIsFlipped(arrowIsFlipped);
        givenStoreImage.GetComponent<StoreIcon>().StoreSpriteType(arrowSpriteType);

        // Store the item's type and damage
        givenStoreImage.GetComponent<StoreIcon>().StoreItemType(arrowItemType);

        // Store the item's buy and sell value
        givenStoreImage.GetComponent<StoreIcon>().StoreBuyValue(arrowBuyValue);

        // Store the item's name and description
        givenStoreImage.GetComponent<StoreIcon>().StoreItemName(arrowItemName);

        // Store as a supply item of type "arrow"
        givenStoreImage.GetComponent<StoreIcon>().SetSupplyItem("arrow");

        // Set the image's weapon header and content
        givenStoreImage.GetComponent<TooltipTrigger>().SetHeader(arrowItemName);
        givenStoreImage.GetComponent<TooltipTrigger>().SetContent(arrowDescription);
        givenStoreImage.GetComponent<TooltipTrigger>().SetAttributes(" ");
        // There is no "Attribute" text for the arrow
    }

    // PUBLIC FUNCTIONS

    // Function for deactivating every checkmark in the list of checkmarks (buys and sells)
    public void DeactivateAllCheckmarks()
    {
        // Deactivate buy checkmarks
        foreach (GameObject checkmark in buyCheckmarks)
        {
            checkmark.SetActive(false);
        }

        // Deactivate sell checkmarks
        foreach (GameObject checkmark in sellCheckmarks)
        {
            checkmark.SetActive(false);
        }

        // Set the boolean "checkmarkActivated" to false
        foreach (GameObject image in storeImages)
        {
            image.GetComponent<StoreIcon>().SetCheckmarkActivated(false);
        }
        foreach (GameObject image in sellStoreImages)
        {
            image.GetComponent<StoreIcon>().SetCheckmarkActivated(false);
        }
    }

    // Function that indicates whether an object is selected (a checkmark is activated)
    public bool IsItemSelected(string menuType)
    {
        if (menuType == "buy")
        {
            // Check each checkmark; if one is active, then an object is selected
            foreach (GameObject checkmark in buyCheckmarks)
            {
                if (checkmark.activeInHierarchy)
                {
                    return true;
                }
            }
            // Otherwise, none are selected
            return false;
        }
        else if (menuType == "sell")
        {
            // Check each checkmark; if one is active, then an object is selected
            foreach (GameObject checkmark in sellCheckmarks)
            {
                if (checkmark.activeInHierarchy)
                {
                    return true;
                }
            }
            // Otherwise, none are selected
            return false;
        }
        else
        {
            // Improper value of "buy" or "sell"
            return false;
        }
    }

    // Function for setting the buy menu image
    public void SetMenuImage(bool isBuyIcon, Sprite sprite, int spriteAngle, bool isFlipped, string type, string description, int buyValue, int sellValue, string supplyItem)
    {
        // Set buy menu image
        if (isBuyIcon)
        {
            // Make the image visible
            buyMenuImage.color = new Color(1f, 1f, 1f, 1f);

            // Set the sprite image
            buyMenuImage.sprite = sprite;

            // Set the correct angle
            buyMenuImage.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, spriteAngle);

            // Change the type
            type = char.ToUpper(type[0]) + type.Substring(1); // Make the first letter uppercase
            SetTypeText(type);

            // Set description (unless it's a supply item)
            if (supplyItem == "none")
            {
                SetDescriptionText(description);
                ToggleDescription(true);
            }
            else
            {
                SetDescriptionText("");
                ToggleDescription(false);
            }
            
            // Set the cost
            SetCostText("$" + buyValue.ToString());

            // Flip the image if necessary
            if (isFlipped)
            {
                buyMenuImage.GetComponent<RectTransform>().localScale = new Vector2(-1f, 1f);
            }
            else
            {
                buyMenuImage.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
            }
        }
    }

    // Function for setting the description and type texts
    public void SetStatTexts(string typeTxt, string descTxt)
    {
        typeText.text = typeTxt;
        descriptionText.text = descTxt;
    }

    // Function that randomizes the buy inventory up to a desired spot, then disables
    // everything else
    public void SetBuyInventory(int index)
    {
        int counter = 0;
        foreach (GameObject storeImage in storeImages)
        {
            // There are only 18 regular store images
            // The last three are for special, permanent items
            // Anymore should be ignored
            if (counter < index && counter < 18)
            {
                // Get an item
                ItemCard currentItem = master.GetComponent<Master>().GetRandomItem();

                // Set the icon
                storeImage.GetComponent<StoreIcon>().SetSubIcon(currentItem.weaponSprite);

                // Flip the sprite if necessary
                if (currentItem.spriteFlipped)
                {
                    storeImage.GetComponent<StoreIcon>().FlipSubIcon();
                }

                // Adjust the sprite angle
                storeImage.GetComponent<StoreIcon>().SetSubIconAngle(currentItem.spriteAngle);

                // Store the sprite angle and the flip status in the image (and the sprite type)
                // (for later when the image is highlighted as the Buy Image)
                storeImage.GetComponent<StoreIcon>().StoreSpriteAngle(currentItem.spriteAngle);
                storeImage.GetComponent<StoreIcon>().StoreIsFlipped(currentItem.spriteFlipped);
                storeImage.GetComponent<StoreIcon>().StoreSpriteType(currentItem.weaponSpriteType);

                // Store the item's type and damage
                storeImage.GetComponent<StoreIcon>().StoreItemType(currentItem.weaponType);
                storeImage.GetComponent<StoreIcon>().StoreItemDamage(currentItem.weaponDamage);

                // Store the item's buy and sell value
                storeImage.GetComponent<StoreIcon>().StoreBuyValue(currentItem.buyValue);
                storeImage.GetComponent<StoreIcon>().StoreSellValue(currentItem.sellValue);

                // Store the item's talisman information
                if (currentItem.isTalisman)
                {
                    storeImage.GetComponent<StoreIcon>().StoreIsTalisman(currentItem.isTalisman);
                    storeImage.GetComponent<StoreIcon>().StoreTalismanBuffType(currentItem.talismanType);
                    storeImage.GetComponent<StoreIcon>().StoreTalismanBuffValue(GetTalismanBuff(currentItem));
                }

                // Store the item's name and description
                storeImage.GetComponent<StoreIcon>().StoreItemName(currentItem.weaponName);
                storeImage.GetComponent<StoreIcon>().StoreItemDescription(currentItem.weaponDescription);
                storeImage.GetComponent<StoreIcon>().StoreItemAttributes(currentItem.weaponAttributes);

                // Set the image's weapon header and content
                storeImage.GetComponent<TooltipTrigger>().SetHeader(currentItem.weaponName);
                storeImage.GetComponent<TooltipTrigger>().SetContent(currentItem.weaponDescription);
                storeImage.GetComponent<TooltipTrigger>().SetAttributes(currentItem.weaponAttributes);
            }
            else // If outside the desired # of items, disable the slot
            {
                storeImage.SetActive(false);
            }

            // Item 19 is buying arrows always
            if (counter == 18)
            {
                SetShopArrow(storeImage);
            }

            // Increment the counter
            counter += 1;
        }
    }

    // Function that sets the sell menu to be the player's inventory
    public void SetSellMenu()
    {
        // Get the player's inventory
        List<GameObject> playerInventory = gameController.GetComponent<Inventory>().GetInventoryImages();

        // Put the legitimate items in a separate list
        List<GameObject> playerRealItems = new List<GameObject>();
        foreach (GameObject item in playerInventory)
        {
            if (item.GetComponent<ItemIcon>().GetSubIconSprite() != null)
            {
                // Do not add items from the player's inventory that are currently equipped (i.e. anything
                // with an index value higher than 24
                if (item.GetComponent<ItemIcon>().GetLocationIndex() < 24)
                {
                    playerRealItems.Add(item);
                }

            }
        }

        // List for holding active spots
        List<int> activeSpots = new List<int>();

        // Place the real items in the sell menu, deactivate the others
        foreach (GameObject item in playerRealItems)
        {
            int i = item.GetComponent<ItemIcon>().GetLocationIndex();

            // Add the active index to the "activeSpots"
            activeSpots.Add(i);

            // Set the icon
            sellStoreImages[i].GetComponent<StoreIcon>().SetSubIcon(item.GetComponent<ItemIcon>().GetSubIconSprite());

            // Flip the sprite if necessary
            if (item.GetComponent<ItemIcon>().GetIsFlipped())
            {
                sellStoreImages[i].GetComponent<StoreIcon>().FlipSubIcon();
            }

            // Adjust the sprite angle
            sellStoreImages[i].GetComponent<StoreIcon>().SetSubIconAngle(item.GetComponent<ItemIcon>().GetSubIconAngle());

            // Store the sprite angle and the flip status in the image
            // (for later when the image is highlighted as the Buy Image)
            sellStoreImages[i].GetComponent<StoreIcon>().StoreSpriteAngle(item.GetComponent<ItemIcon>().GetSubIconAngle());
            sellStoreImages[i].GetComponent<StoreIcon>().StoreIsFlipped(item.GetComponent<ItemIcon>().GetIsFlipped());
            sellStoreImages[i].GetComponent<StoreIcon>().StoreSpriteType(item.GetComponent<ItemIcon>().GetWeaponSpriteType());

            // Store the item's type and damage
            sellStoreImages[i].GetComponent<StoreIcon>().StoreItemType(item.GetComponent<ItemIcon>().GetWeaponType());
            sellStoreImages[i].GetComponent<StoreIcon>().StoreItemDamage(item.GetComponent<ItemIcon>().GetWeaponDamage());

            // There's no real reason to store talisman details for an item being sold, so we're not.

            // Store the item's buy and sell value
            sellStoreImages[i].GetComponent<StoreIcon>().StoreBuyValue(item.GetComponent<ItemIcon>().GetBuyValue());
            sellStoreImages[i].GetComponent<StoreIcon>().StoreSellValue(item.GetComponent<ItemIcon>().GetSellValue());

            // Store the item's name and description
            sellStoreImages[i].GetComponent<StoreIcon>().StoreItemName(item.GetComponent<ItemIcon>().GetWeaponName());
            sellStoreImages[i].GetComponent<StoreIcon>().StoreItemDescription(item.GetComponent<ItemIcon>().GetWeaponDescription());

            // Store the item's location index
            sellStoreImages[i].GetComponent<StoreIcon>().StoreLocationIndex(item.GetComponent<ItemIcon>().GetLocationIndex());

            // Set the image's weapon header and content
            sellStoreImages[i].GetComponent<TooltipTrigger>().SetHeader(item.GetComponent<ItemIcon>().GetWeaponName());
            sellStoreImages[i].GetComponent<TooltipTrigger>().SetContent(item.GetComponent<ItemIcon>().GetWeaponDescription());
            sellStoreImages[i].GetComponent<TooltipTrigger>().SetAttributes(item.GetComponent<ItemIcon>().GetWeaponAttributes());
        }

        // Deactivate every spot that is not active
        for (int i = 0; i < inventorySpots; i++)
        {
            if (!activeSpots.Contains(i))
            {
                sellStoreImages[i].SetActive(false);
            }
        }
    }

    // Function that selects every item in the sell menu
    public void SelectAllSellMenu()
    {
        foreach (GameObject sellStoreImage in sellStoreImages)
        {
            // "Select" every sell menu item that holds an item to sell, and is not currently active
            if (sellStoreImage.activeSelf && !sellStoreImage.GetComponent<StoreIcon>().GetCheckmarkActivated())
            {
                sellStoreImage.GetComponent<StoreIcon>().ToggleCheckmark();
            }
        }
        ResetValueText();
    }

    // Function that holds information for the selected item
    public void SetSelectedItem(SelectedItem theItem)
    {
        theSelectedItem = theItem;
    }

    // Function that returns all the selected items
    public List<SelectedItem> GetSelectedItems()
    {
        return selectedItems;
    }

    // Function that returns the total value of the selected items
    public int GetSelectedItemsValue()
    {
        int total = 0;
        foreach (SelectedItem item in selectedItems)
        {
            total += item.GetSellValue();
        }

        return total;
    }

    // Function that adds to the list of selected items
    public void AddToSelectedItems(SelectedItem newItem)
    {
        // If it's the first item, add it to the list
        if (selectedItems.Count == 0)
        {
            selectedItems.Add(newItem);
        }
        else // Not the first item
        {
            // Do not add this item if it is already in the list
            foreach (SelectedItem item in selectedItems)
            {
                // If the item's location index is found, it is already there; return out
                if (item.GetLocationIndex() == newItem.GetLocationIndex())
                {
                    return;
                }
            }

            // Add the item
            selectedItems.Add(newItem);
        }
    }

    // Function that removes from the list of selected items
    public void RemoveFromSelectedItems(SelectedItem item)
    {
        selectedItems.Remove(item);
    }

    // Function that removes all selected items from the list of selected items
    public void RemoveAllSelectedItems()
    {
        // Removes everything by initializing a new list
        selectedItems = new List<SelectedItem>();
    }

    // Getter functions for selected items //

    // Function that gets the selected weapon name
    public string GetSelectedWeaponName()
    {
        return theSelectedItem.GetWeaponName();
    }

    // Function that gets the selected item's buy value
    public int GetSelectedBuyValue()
    {
        return theSelectedItem.GetBuyValue();
    }

    // Function that gets the selected item's sell value
    public int GetSelectedSellValue()
    {
        return theSelectedItem.GetSellValue();
    }

    // Function that sets the text for the Type Text (Buy Menu)
    public void SetTypeText(string theText)
    {
        typeText.text = theText;
    }

    // Function that sets the text for the Description Text (Buy Menu)
    public void SetDescriptionText(string theText)
    {
        descriptionText.text = theText;
    }

    // Function that sets the text for the Cost Text (Buy Menu)
    public void SetCostText(string theText)
    {
        costText.text = theText;
    }

    // Function that sets the text for the type text (Sell Menu)
    public void SetSellTypeText(string theText)
    {
        sellTypeText.text = theText;
    }

    // Function that sets the text for the Description text (Sell Menu)
    public void SetSellDescriptionText(string theText)
    {
        sellDescriptionText.text = theText;
    }

    // Function that sets the text for the Value text (Sell Menu)
    public void SetValueText(string theText)
    {
        valueText.text = theText;
    }
    
    // Function that reset the Buy Menu Image
    public void ResetBuyMenuImage()
    {
        // Make sprite blank, make description text blank
        buyMenuImage.sprite = null;
        buyMenuImage.color = new Color(1f, 1f, 1f, 0f);
        SetTypeText("");
        SetDescriptionText("");
        SetCostText("");
    }

    // Function that resets the Sell Menu Image
    public void ResetSellMenuImage()
    {
        // Make sprite blank, make description text blank
        sellMenuImage.sprite = null;
        sellMenuImage.color = new Color(1f, 1f, 1f, 0f);
        SetSellTypeText("--------");
        SetSellDescriptionText("-----");
        SetValueText("$0");
    }

    // Function that re-activates all the images in the sell store
    public void ReactivateSellStoreImages()
    {
        foreach (GameObject image in sellStoreImages)
        {
            image.SetActive(true);
        }
    }

    // Function that is called when "Yes" is selected on the Buy Dialogue buttons.
    public void BuyDialogueYes()
    {
        // Supply items
        if (theSelectedItem.GetSupplyItem() != "none")
        {
            // Temporary variable to hold cost
            int costOfSupplies = buyQuantityValue * theSelectedItem.GetBuyValue();

            // Make sure the player can afford the item
            if (player.GetComponent<MoneyController>().GetPlayerMoney() >= costOfSupplies)
            {
                // Case where the player is buying arrows
                if (theSelectedItem.GetSupplyItem() == "arrow")
                {
                    // Subtract money from the player
                    player.GetComponent<MoneyController>().SubtractFromPlayerMoney(costOfSupplies);

                    // Toggle menus - turn off Multiple, turn on Single
                    GetComponent<HUD>().ToggleBuyDialogueBoxMultiple(false);
                    GetComponent<HUD>().ToggleBuyDialogueBoxSingle(true);

                    // Change the confirmation text (use the regular menu)
                    SetBuyDialogueText("Great! Here's your arrows. Thank you for your purchase!");

                    // Add 25 arrows to the player's inventory
                    player.GetComponent<ArrowCount>().AddArrows(buyQuantityValue);

                    // Disable the "yes" and "no" buttons
                    Buy_DisableYesButton();
                    Buy_DisableNoButton();

                    // Reset all the quantity values and texts
                    ResetBuyQuantity();

                    // Indicate that we are at the confirmation dialogue
                    atConfirmationDialogue = true;
                }
            }
            else // Cannot afford the item
            {
                // Swap from "Multiple" menu to "Single" menu, if necessary
                GetComponent<HUD>().ToggleBuyDialogueBoxSingle(true);
                GetComponent<HUD>().ToggleBuyDialogueBoxMultiple(false);

                // Indicate to the player that they can't afford the item
                SetBuyDialogueText("You can't afford that!");

                // Turn off the yes and no buttons
                Buy_DisableYesButton();
                Buy_DisableNoButton();

                // Indicate that we are at the confirmation dialogue
                atConfirmationDialogue = true;
            }
        }
        // Individual items
        else
        {
            // Make sure the player can afford the item
            if (player.GetComponent<MoneyController>().GetPlayerMoney() >= theSelectedItem.GetBuyValue())
            {
                // Make sure the player has enough inventory slots
                if (gameController.GetComponent<Inventory>().GetNumberOfItems() < 24 && theSelectedItem.GetSupplyItem() == "none")
                {
                    // Subtract money from the player
                    player.GetComponent<MoneyController>().SubtractFromPlayerMoney(theSelectedItem.GetBuyValue());

                    // Increase the player's inventory count
                    gameController.GetComponent<Inventory>().IncrementNumberOfItems();

                    // Change the confirmation text
                    SetBuyDialogueText("Great! Here's your " + GetSelectedWeaponName() + ". Thank you for your purchase!");

                    // Get rid of the store icon for the item, since it has been bought
                    theSelectedItem.GetStoreIcon().gameObject.SetActive(false);

                    // Fill the player's inventory with the object - cast to "Weapon" first
                    // PROBLEM - Selected weapon is not "initialized" to anything, so this is coming up null (line below)
                    selectedWeapon = new Weapon();
                    selectedWeapon.Initialize(theSelectedItem.GetWeaponType(), theSelectedItem.GetWeaponDamage(), theSelectedItem.GetWeaponName(), theSelectedItem.GetWeaponDescription(), theSelectedItem.GetWeaponAttributes(), theSelectedItem.GetSpriteType(), theSelectedItem.GetIsFlipped(), theSelectedItem.GetAngle(), theSelectedItem.GetBuyValue(), theSelectedItem.GetSellValue(), theSelectedItem.GetIsTalisman(), theSelectedItem.GetTalismanBuffType(), theSelectedItem.GetTalismanBuffValue());
                    gameController.GetComponent<Inventory>().FillInventoryImage(theSelectedItem.GetItemSprite(), selectedWeapon);

                    // Disable the "yes" and "no" buttons
                    Buy_DisableYesButton();
                    Buy_DisableNoButton();

                    // Indicate that we are at the confirmation dialogue
                    atConfirmationDialogue = true;

                    // Rest the buy menu image (because that item is gone now)
                    ResetBuyMenuImage();
                }
                else if (gameController.GetComponent<Inventory>().GetNumberOfItems() >= 24 && theSelectedItem.GetSupplyItem() == "none") // Player does not have the inventory space
                {
                    // Indicate to the player that they don't have enough space for that item
                    SetBuyDialogueText("You don't have enough space for that!");

                    // Turn off the yes and no buttons
                    Buy_DisableYesButton();
                    Buy_DisableNoButton();

                    // Indicate that we are at the confirmation dialogue
                    atConfirmationDialogue = true;
                }
            }
            else
            {
                // Swap from "Multiple" menu to "Single" menu, if necessary
                GetComponent<HUD>().ToggleBuyDialogueBoxSingle(true);
                GetComponent<HUD>().ToggleBuyDialogueBoxMultiple(false);

                // Indicate to the player that they can't afford the item
                SetBuyDialogueText("You can't afford that!");

                // Turn off the yes and no buttons
                Buy_DisableYesButton();
                Buy_DisableNoButton();

                // Indicate that we are at the confirmation dialogue
                atConfirmationDialogue = true;
            }
        }
    }

    // Function that is called when "Yes" is selected on the Sell Dialogue buttons.
    public void SellDialogueYes()
    {
        // Give the player the monetary value of the selected item
        player.GetComponent<MoneyController>().AddToPlayerMoney(GetSelectedItemsValue());

        // Take the total sell value down to zero
        totalSellValue = 0;

        // Set the confirmation text
        SetSellDialogueText("Great! Here's $" + GetSelectedItemsValue().ToString() + ". Thank you for your purchase!");

        // Remove the item from the inventory
        foreach (SelectedItem item in selectedItems)
        {
            // Decrement the number of items in player's inventory
            gameController.GetComponent<Inventory>().DecrementNumberOfItems();

            // Removes from inventory
            gameController.GetComponent<Inventory>().RemoveInventoryImage(item.GetLocationIndex());

            // Removes from sell menu (reset the flip on the icon)
            item.GetStoreIcon().gameObject.SetActive(false);
            item.GetStoreIcon().gameObject.GetComponent<StoreIcon>().ResetFlipSubIcon();
        }

        // Turn off the yes and no buttons
        Sell_DisableYesButton();
        Sell_DisableNoButton();

        // Reset the sell menu image
        ResetSellMenuImage();

        // Re-initialize the list (because it's now empty)
        selectedItems = new List<SelectedItem>();

        // Indicate that we are at the confirmation dialogue
        atConfirmationDialogue = true;
    }

    // Function that is called when "No" is selected on the Buy Dialogue buttons.
    public void BuyDialogueNo()
    {
        // Close the BuyDialogue
        GetComponent<HUD>().CloseBuyDialogue();
    }

    // Function that is called when "No" is selected on the Buy Dialogue buttons.
    public void SellDialogueNo()
    {
        // Close the BuyDialogue
        GetComponent<HUD>().CloseSellDialogue();
    }

    // Function that sets the Buy Dialogue Text
    public void SetBuyDialogueText(string text)
    {
        buyDialogueText_Single.text = text;
    }

    // Function that sets the Buy Dialogue Text for the Multiple box
    public void SetBuyDialogueText_Multiple(string text)
    {
        buyDialogueText_Multiple.text = text;
    }

    // Function that sets the Sell Dialogue Text
    public void SetSellDialogueText(string text)
    {
        sellDialogueText.text = text;
    }

    // Function that enables the buy yes button
    public void Buy_EnableYesButton()
    {
        buy_yesButton.gameObject.SetActive(true);
    }

    // Function that disables the buy yes button
    public void Buy_DisableYesButton()
    {
        buy_yesButton.gameObject.SetActive(false);
    }

    // Function that enables the buy no button
    public void Buy_EnableNoButton()
    {
        buy_noButton.gameObject.SetActive(true);
    }

    // Function that disables the buy no button
    public void Buy_DisableNoButton()
    {
        buy_noButton.gameObject.SetActive(false);
    }

    // Function that enables the sell yes button
    public void Sell_EnableYesButton()
    {
        sell_yesButton.gameObject.SetActive(true);
    }

    // Function that disables the sell yes button
    public void Sell_DisableYesButton()
    {
        sell_yesButton.gameObject.SetActive(false);
    }

    // Function that enables the sell no button
    public void Sell_EnableNoButton()
    {
        sell_noButton.gameObject.SetActive(true);
    }

    // Function that disables the sell no button
    public void Sell_DisableNoButton()
    {
        sell_noButton.gameObject.SetActive(false);
    }

    // Function for the arrows in the buy quantity box
    // Increments or decrements the quantity
    public void ChangeQuantity(int value)
    {
        if (value == 1 && buyQuantityValue < 999)  // Incrementing; limit is 999
        {
            buyQuantityValue += value;
        }
        else if (value == -1 && buyQuantityValue > 0)
        {
            buyQuantityValue += value;
        }
        else if (value == 10 && buyQuantityValue < 999)
        {
            buyQuantityValue += value;
            if (buyQuantityValue >= 999)  // Don't go over 999
            {
                buyQuantityValue = 999;
            }
        }
        else if (value == -10 && buyQuantityValue > 0)
        {
            buyQuantityValue += value;
            if (buyQuantityValue <= 0) // Don't go under 0
            {
                buyQuantityValue = 0;
            }
        }

        // Update the buy quantity text
        buyQuantityText.text = buyQuantityValue.ToString();

        // Update the buy cost text
        buyCostText.text = FormatMoneyText(buyQuantityValue * theSelectedItem.GetBuyValue());
    }

    // Function that resets the quantity in the buy quantity box
    public void ResetBuyQuantity()
    {
        buyQuantityValue = 0;
        buyQuantityText.text = buyQuantityValue.ToString();
        buyCostText.text = FormatMoneyText(buyQuantityValue * theSelectedItem.GetBuyValue());
    }

    // Function that sets the In Inventory count text
    public void SetInInventoryCountText(string theText)
    {
        inInventoryCount.text = theText;
    }

    // Function that sets the In Inventory Image's sprite
    public void SetInInventoryImageSprite(Sprite sprite)
    {
        inInventoryImage.sprite = sprite;
    }

    // Function that sets the In Inventory Box
    public void SetInInventoryBox(string supplyItem)
    {
        if (supplyItem == "arrow")
        {
            // Set the count of the arrows to be the player's inventory amount
            inInventoryCount.text = "x" + player.GetComponent<ArrowCount>().GetArrowCount().ToString();

            // Set the sprite
            inInventoryImage.sprite = theSelectedItem.GetItemSprite();
            inInventoryImage.transform.rotation = Quaternion.Euler(0, 0, theSelectedItem.GetAngle());
        }
    }

    // Function that toggles the description
    public void ToggleDescription(bool value)
    {
        descriptionTitle.gameObject.SetActive(value);
        descriptionText.gameObject.SetActive(value);
    }

    // Function that increments the total sell value
    public void IncrementTotalSellValue(int amount)
    {
        totalSellValue += amount;
    }

    // Function that decrements the total sell value
    public void DecrementTotalSellValue(int amount)
    {
        totalSellValue -= amount;
    }

    // Function that resets the total sell value to 0
    public void ResetTotalSellValue()
    {
        totalSellValue = 0;
    }

    // Function that resets the value text
    public void ResetValueText()
    {
        SetValueText("$" + totalSellValue.ToString());
    }

    // Function that determines what buff type should be returned from a talisman item card
    public int GetTalismanBuff(ItemCard itemCard)
    {
        int buffToReturn;

        switch (itemCard.talismanType)
        {
            case "fixed_attack":
                buffToReturn = itemCard.fixedAttackBuff;
                break;
            case "fixed_bow":
                buffToReturn = itemCard.fixedBowBuff;
                break;
            case "fixed_HP":
                buffToReturn = itemCard.fixedHPBuff;
                break;
            case "percent_attack":
                buffToReturn = itemCard.percentAttackBuff;
                break;
            case "percent_bow":
                buffToReturn = itemCard.percentBowBuff;
                break;
            case "percent_HP":
                buffToReturn = itemCard.percentHPBuff;
                break;
            case "percent_speed":
                buffToReturn = itemCard.percentSpeedBuff;
                break;
            default:
                return 999;
        }
        return buffToReturn;
    }
}
