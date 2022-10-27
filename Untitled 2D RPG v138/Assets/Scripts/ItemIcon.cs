using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    [SerializeField] Image subIcon;

    bool isFlipped = false;

    private string weaponType;
    private int weaponDamage;
    private string weaponName;
    private string weaponDescription;
    private string weaponAttributes;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private GameObject gameController;
    private GameObject player;

    private GameObject currentSlot;
    private int locationIndex;

    private bool hasASpot = true;
    private bool hasASprite = false;
    private bool inUseSpot = false;
    private bool wasInUseSpot = false;

    private Vector2 lastPosition;

    private string weaponSpriteType;
    private int spriteAngle;

    // Rect Transform boundaries of the menu
    private float minX = -700f;
    private float maxX = 365f;
    private float minY = -325f;
    private float maxY = 325f;

    // These are pretty much just going to be passed to the store view of the inventory
    // (Mainly the sell value)
    private int buyValue;
    private int sellValue;

    // Talisman type and buff
    private string talismanType = "null";
    private int talismanBuff = 0;

    // Main object references
    GameObject abilityManager;
    GameObject master;

    // PRIVATE FUNCTIONS
    private void Start()
    {
        abilityManager = GameObject.FindWithTag("AbilityManager");
        master = GameObject.FindWithTag("Master");
    }

    private void Update()
    {
        if (player.GetComponent<Health>().GetIsHurt() && weaponType == "talisman" && inUseSpot)
        {
            // Store this icon elsewhere
            master.GetComponent<Master>().SetDisabledIcon(this);

            // Disable this icon
            this.enabled = false;
        }
    }

    // Function that checks to see if the dragged item is the bounds of the menu
    private bool IsInBounds()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform.anchoredPosition.x < minX || rectTransform.anchoredPosition.x > maxX || rectTransform.anchoredPosition.y < minY || rectTransform.anchoredPosition.y > maxY)
        {
            return false;
        }
        return true;
    }

    // Function that reverts this item icon to its previous spot
    private void RevertToLastSlot()
    {
        rectTransform.anchoredPosition = lastPosition;

        // Still has a spot, even if it reverts back to its old one
        hasASpot = true;

        // Current slot is still occupied
        currentSlot.GetComponent<ItemSlot>().SetSlotOpen(false);

        // If the reverted slot is still a use spot, then the image should be reactivated (and the stats)
        if (currentSlot.GetComponent<ItemSlot>().GetIsUseSlot())
        {
            if (weaponType == "sword")
            {
                // Reactivate the sprite
                gameController.GetComponent<Inventory>().FillPlayerWeaponImage(GetSubIconSprite(), spriteAngle);

                // Reactivate the weapon itself
                player.GetComponent<PlayerController>().EquipWeapon(GetSubIconSprite(), weaponSpriteType, weaponDamage,
                                                                       abilityManager.GetComponent<CybilMight>().GetIsMighted());
            }

            if (weaponType == "bow")
            {
                // Activate the right bow sprite type
                if (weaponSpriteType == "Straight")
                {
                    player.GetComponent<PlayerController>().ActivateStraightBow();
                }
                else if (weaponSpriteType == "Kyrises")
                {
                    player.GetComponent<PlayerController>().ActivateKyrisesBow();
                }

                // Reactivate the sprite
                player.GetComponent<PlayerController>().ChangeBowSprite(GetSubIconSprite());

                // Reactivate the weapon itself
                player.GetComponent<PlayerProgression>().SetCurrentBowDamage(weaponDamage);
            }

            if (weaponType == "talisman")
            {
                // Add the player's buff
                player.GetComponent<PlayerProgression>().AddBuff(talismanType, talismanBuff);
            }

            inUseSpot = true;
            wasInUseSpot = false;
        }
        else // Not a use slot; re-increment the inventory count
        {
            gameController.GetComponent<Inventory>().IncrementNumberOfItems();
        }
    }

    // PUBLIC FUNCTIONS

    // Function that assigns this image to a spot
    public void AssignImageToSpot()
    {
        // VERY IMPORTANT //

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        gameController = GameObject.FindWithTag("GameController");
        player = GameObject.FindWithTag("Player");

        // We are doing this on Awake for now as a test; in the future, the item should be assigned a default slot when it gets picked up
        // (because we're gonna save the game, and we want the slot to save across sessions)

        // Assign a default location
        foreach (GameObject theItemSlot in gameController.GetComponent<Inventory>().GetInventorySlots())
        {
            if (theItemSlot.GetComponent<ItemSlot>().IsSlotOpen() == true)
            {
                rectTransform.anchoredPosition = theItemSlot.GetComponent<ItemSlot>().GetRectTransform();
                lastPosition = rectTransform.anchoredPosition;

                // Close the slot
                theItemSlot.GetComponent<ItemSlot>().SetSlotOpen(false);

                // Set the current slot
                currentSlot = theItemSlot;

                // Assign the icon to the item slot's records
                theItemSlot.GetComponent<ItemSlot>().SetCurrentIcon(gameObject);

                // Stop looping
                break;
            }
        }

        // Because Unity messes this up sometimes
        hasASpot = true;
    }

    // Event for mouse dragging beginning
    public void OnBeginDrag(PointerEventData eventData)
    {
        // As soon as we begin dragging, we no longer have a spot for the item
        hasASpot = false;

        // If we were in a use spot, indicate we are no longer in it and deactivate the player weapon image
        if (inUseSpot)
        {
            inUseSpot = false;
            wasInUseSpot = true;
            gameController.GetComponent<Inventory>().DeactivatePlayerWeaponImage();

            // Also, unequip the player's weapon
            if (weaponType == "sword")
            {
                // Deactivate weapon, reset player's damage
                player.GetComponent<PlayerController>().DeactivateWeapon();

                // Set the player's damage to 0 (so it's just his inherent attack)
                player.GetComponent<PlayerProgression>().SetPlayerDamage(0f);
            }
            else if (weaponType == "bow")
            {
                // Activate the right bow sprite type
                if (weaponSpriteType == "Straight")
                {
                    player.GetComponent<PlayerController>().ActivateStraightBow();
                }
                else if (weaponSpriteType == "Kyrises")
                {
                    player.GetComponent<PlayerController>().ActivateKyrisesBow();
                }

                // Change the bow sprite
                player.GetComponent<PlayerController>().ChangeBowSprite(player.GetComponent<PlayerController>().GetDefaultBowSprite());

                // Back to "straight" bow type
                player.GetComponent<PlayerController>().ActivateStraightBow();

                // Readjust the player's bow damage based on this weapon
                player.GetComponent<PlayerProgression>().SetCurrentBowDamage(10f);
            }
            else if (weaponType == "talisman")
            {
                // Remove the player's buff
                player.GetComponent<PlayerProgression>().RemoveBuff();
            }
        }
        else
        {
            // Decrement the inventory counter (but only if it's not in a use spot)
            gameController.GetComponent<Inventory>().DecrementNumberOfItems();
        }

        // When the item is being dragged, let raycasts go through it
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // We also need every other icon to allow raycasts to go through it (for spot swapping)
        gameController.GetComponent<Inventory>().ToggleImageRaycasts(false);

        // Set this slot to open again!
        currentSlot.GetComponent<ItemSlot>().SetSlotOpen(true);

    }

    // Event for every moment of mouse drag in-between beginning and end
    public void OnDrag(PointerEventData eventData)
    {
        // The position of the image moves with the mouse as it drags
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // The image is set to be the last sibling, so that it will not show up "under" other images
        transform.SetAsLastSibling();
    }

    // Event for mouse dragging ending
    public void OnEndDrag(PointerEventData eventData)
    {
        // When the item is finished being dragged, don't let raycasts go through it anymore
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (hasASpot == false && !IsInBounds())
        {
            // Removes from inventory
            gameController.GetComponent<Inventory>().RemoveInventoryImage(GetLocationIndex());

            // Drop item on the ground
            player.GetComponent<PlayerController>().DropByPlayer(GetWeaponName());

            // Reset this icon's flip
            ResetSubIconFlip();
        }
        // If the item never found a place, set it back to it's last position
        else if (hasASpot == false)
        {
            RevertToLastSlot();
        }
        else // Weapon has found a spot
        {
            // If the weapon spot is a use spot, then we need to change some things based on what was equipped
            if (inUseSpot)
            {
                if (weaponType == "sword")
                {
                    // Assign the weapon to the player
                    player.GetComponent<PlayerController>().EquipWeapon(GetSubIconSprite(), weaponSpriteType, weaponDamage,
                                                                            abilityManager.GetComponent<CybilMight>().GetIsMighted());
                }

                else if (weaponType == "bow")
                {
                    // Activate the right bow sprite type
                    if (weaponSpriteType == "Straight")
                    {
                        player.GetComponent<PlayerController>().ActivateStraightBow();
                    }
                    else if (weaponSpriteType == "Kyrises")
                    {
                        player.GetComponent<PlayerController>().ActivateKyrisesBow();
                    }

                    // Change the bow sprite
                    player.GetComponent<PlayerController>().ChangeBowSprite(GetSubIconSprite());

                    // Set player's current bow damage
                    player.GetComponent<PlayerProgression>().SetCurrentBowDamage(weaponDamage);

                    // Readjust the player's bow damage based on this weapon
                    player.GetComponent<PlayerProgression>().SetCurrentBowDamage(weaponDamage);
                }
                else if (weaponType == "talisman")
                {
                    // Add the player's buff
                    player.GetComponent<PlayerProgression>().AddBuff(talismanType, talismanBuff);
                }
            }
            else // Not a use spot; re-increment the weapon count;
            {
                gameController.GetComponent<Inventory>().IncrementNumberOfItems();
            }
        }

        // Once dragging is over, re-enable all other images' raycast detection
        gameController.GetComponent<Inventory>().ToggleImageRaycasts(true);
    }

    // Event for the mouse clicking down on the object
    public void OnPointerDown(PointerEventData eventData)
    {
        // Unused presently
    }

    // Function for setting whether or not the item has a spot
    public void SetHasASpot(bool value)
    {
        hasASpot = value;
    }

    // Function for getting whether or not the item has a spot
    public bool GetHasASpot()
    {
        return hasASpot;
    }

    // Function for setting the last stable position of the item
    public void SetLastPosition(Vector2 thePosition)
    {
        lastPosition = thePosition;
    }

    // Function for getting the last stable position of the item
    public Vector2 GetLastPosition()
    {
        return lastPosition;
    }

    // Function for getting the item icon's location index value
    public int GetLocationIndex()
    {
        return locationIndex;
    }

    // Function for setting the item icon's location index value
    public void SetLocationIndex(int index)
    {
        locationIndex = index;
    }

    // Function for getting the weapon type
    public string GetWeaponType()
    {
        return weaponType;
    }

    // Function for setting the weapon type
    public void SetWeaponType(string type)
    {
        weaponType = type;
    }

    // Function for getting whether or not this icon has a sprite
    public bool GetHasASprite()
    {
        return hasASprite;
    }

    // Function for setting whether or not this icon has a sprite
    public void SetHasASprite(bool value)
    {
        hasASprite = value;
    }

    // Function for getting whether or not this icon is in a use spot
    public bool GetInUseSpot()
    {
        return inUseSpot;
    }

    // Function for getting whether or not an icon WAS in a use spot very recently
    public bool GetWasInUseSpot()
    {
        return wasInUseSpot;
    }

    // Function for setting whether or not an icon WAS in a use spot very recently
    public void SetWasInUseSpot(bool value)
    {
        wasInUseSpot = value;
    }

    // Function for setting whether or not this icon is in a use spot
    public void SetInUseSpot(bool value)
    {
        inUseSpot = value;
    }

    // Function for getting the current slot of this icon
    public GameObject GetCurrentSlot()
    {
        return currentSlot;
    }

    // Function for setting the current slot of this icon
    public void SetCurrentSlot(GameObject slot)
    {
        currentSlot = slot;
    }

    // Function for getting the weapon damage of the weapon assigned to this icon
    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    // Function for setting the weapon damage of the weapon assigned to this icon
    public void SetWeaponDamage(int damage)
    {
        weaponDamage = damage;
    }

    // Function for setting the weapon sprite type
    public void SetWeaponSpriteType(string type)
    {
        weaponSpriteType = type;
    }

    // Function for getting the weapon sprite type
    public string GetWeaponSpriteType()
    {
        return weaponSpriteType;
    }

    // Function for setting the subicon's sprite
    public void SetSubIconSprite(Sprite theSprite)
    {
        subIcon.sprite = theSprite;
    }

    // Function for getting the subicon's sprite
    public Sprite GetSubIconSprite()
    {
        return subIcon.sprite;
    }

    // Function for flipping the subIcon
    public void FlipSubIcon()
    {
        subIcon.GetComponent<RectTransform>().localScale = new Vector2(-1f, 1f);
    }

    // Reset the subicon's flipped state (for when things are dropped)
    public void ResetSubIconFlip()
    {
        subIcon.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
    }

    // Function for setting the subIcon's angle
    public void SetSubIconAngle(int angle)
    {
        // Storage for use later, when transferring to the use slot
        spriteAngle = angle;

        subIcon.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Function for getting the subIcon's angle
    public int GetSubIconAngle()
    {
        return spriteAngle;
    }

    // Function for setting whether or not the subIcon should be flipped
    public void SetIsFlipped(bool value)
    {
        isFlipped = value;
    }

    // Function for getting whether or not the subIcon should be flipped
    public bool GetIsFlipped()
    {
        return isFlipped;
    }

    // Function for setting the buy value of the icon's weapon
    public void SetBuyValue(int value)
    {
        buyValue = value;
    }

    // Function for setting the sell value of the icon's weapon
    public void SetSellValue(int value)
    {
        sellValue = value;
    }

    // Function for getting the buy value of the icon's weapon
    public int GetBuyValue()
    {
        return buyValue;
    }

    // Function for getting the sell value of the icon's weapon 
    public int GetSellValue()
    {
        return sellValue;
    }
    
    // Function for setting the weapon name
    public void SetWeaponName(string name)
    {
        weaponName = name;
    }

    // Function for getting the weapon name
    public string GetWeaponName()
    {
        return weaponName;
    }

    // Function for setting the weapon description
    public void SetWeaponDescription(string description)
    {
        weaponDescription = description;
    }

    // Function for getting the weapon description
    public string GetWeaponDescription()
    {
        return weaponDescription;
    }

    // Function for setting the weapon attributes
    public void SetWeaponAttributes(string attributes)
    {
        weaponAttributes = attributes;
    }

    // Function for getting the weapon attributes
    public string GetWeaponAttributes()
    {
        return weaponAttributes;
    }

    // Function for setting the talisman type
    public void SetTalismanType(string value)
    {
        talismanType = value;
    }

    // Function for setting the talisman buff
    public void SetTalismanBuff(int value)
    {
        talismanBuff = value;
    }

    // Function for getting the talisman type
    public string GetTalismanType()
    {
        return talismanType;
    }

    // Function for getting the talisman buff
    public int GetTalismanBuff()
    {
        return talismanBuff;
    }
 }
