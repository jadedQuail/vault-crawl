using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] float xPos;
    [SerializeField] float yPos;
    [SerializeField] int slotIndexValue;
    [SerializeField] bool useSlot;
    [SerializeField] string useSlotType;

    private bool isOpen = true;

    // Override for blocking movement (Failed swap)
    private bool blockMovement = false;

    GameObject gameController;
    GameObject player;
    GameObject hud;

    GameObject currentIcon;

    private void Start()
    {
        gameController = GameObject.FindWithTag("GameController");
        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
    }

    // Function that swaps the slots of an old object and a new object
    // (i.e. we are sending the old icon to the new icon's place)
    // This function only gets called ONCE, by whatever item is being dropped
    private void SwapSlots(GameObject oldIcon, GameObject newIcon)  // New icon is what is getting dragged into old icon's slot
    {
        // If we're trying to swap a talisman out of the use slot while the player is hurt, then block it.
        if (!player.GetComponent<Health>().GetIsHurt() || (!oldIcon.GetComponent<ItemIcon>().GetInUseSpot() && !oldIcon.GetComponent<ItemIcon>().GetInUseSpot()))
        {
            // Set spot status of the image to true
            oldIcon.GetComponent<ItemIcon>().SetHasASpot(true);

            // Set a new "last position"
            oldIcon.GetComponent<ItemIcon>().SetLastPosition(newIcon.GetComponent<ItemIcon>().GetLastPosition());
            // Where is the old icon now (it's NEW location)

            // Set a new index value

            // Adjust index values; both slots are still occupied
            int indexDraggedFrom = newIcon.GetComponent<ItemIcon>().GetLocationIndex();
            int indexDraggedTo = oldIcon.GetComponent<ItemIcon>().GetLocationIndex();

            // Old and new icon swap indices
            oldIcon.GetComponent<ItemIcon>().SetLocationIndex(indexDraggedFrom);
            newIcon.GetComponent<ItemIcon>().SetLocationIndex(indexDraggedTo);

            // Both indices should be occupied
            gameController.GetComponent<Inventory>().SetSlotTrackerAt(indexDraggedFrom, false);
            gameController.GetComponent<Inventory>().SetSlotTrackerAt(indexDraggedTo, false);

            // Set "isOpen" on that new slot to be false (might already be, but we're making sure)
            newIcon.GetComponent<ItemIcon>().GetCurrentSlot().GetComponent<ItemSlot>().SetSlotOpen(false);

            // Set a new current slot for the item icon
            oldIcon.GetComponent<ItemIcon>().SetCurrentSlot(newIcon.GetComponent<ItemIcon>().GetCurrentSlot().GetComponent<ItemSlot>().gameObject);

            // Set a new "current icon" for the item slot that the "old Icon" now occupies
            // (I am aware this is a total shit show, sorry)
            oldIcon.GetComponent<ItemIcon>().GetCurrentSlot().GetComponent<ItemSlot>().SetCurrentIcon(oldIcon);

            // Turn of the old icon's "useSlot" status if it has it and doesn't need it anymore
            if (oldIcon.GetComponent<ItemIcon>().GetInUseSpot())
            {
                oldIcon.GetComponent<ItemIcon>().SetInUseSpot(false);
            }

            // If the old icon just got moved to a useSlot, turn on that functionality
            if (oldIcon.GetComponent<ItemIcon>().GetLocationIndex() >= 24)
            {
                oldIcon.GetComponent<ItemIcon>().SetInUseSpot(true);
                // If this is a sword, set the item's sprite to be the player image's sprite
                if (oldIcon.GetComponent<ItemIcon>().GetWeaponType() == "sword")
                {
                    gameController.GetComponent<Inventory>().FillPlayerWeaponImage(oldIcon.GetComponent<ItemIcon>().GetSubIconSprite(), oldIcon.GetComponent<ItemIcon>().GetSubIconAngle());
                }

                // If this is a talisman and it's in the use slot, then add its buff
                if (oldIcon.GetComponent<ItemIcon>().GetWeaponType() == "talisman")
                {
                    player.GetComponent<PlayerProgression>().AddBuff(oldIcon.GetComponent<ItemIcon>().GetTalismanType(),
                                                                     oldIcon.GetComponent<ItemIcon>().GetTalismanBuff());
                }
            }

            // Set the icon location
            oldIcon.GetComponent<RectTransform>().anchoredPosition = oldIcon.GetComponent<ItemIcon>().GetLastPosition();
        }
    }

    // PUBLIC FUNCTIONS

    public void OnDrop(PointerEventData eventData)
    {
        // This is the item being dragged; if it's not null, then it has been
        // dropped in the box

        if (eventData.pointerDrag != null)
        {
            // Slot is not open; swap must be performed
            if (isOpen == false)
            {
                // Situation where a swap involving a use spot is about to occur
                if (currentIcon.GetComponent<ItemIcon>().GetInUseSpot() || eventData.pointerDrag.GetComponent<ItemIcon>().GetInUseSpot()
                    || currentIcon.GetComponent<ItemIcon>().GetWasInUseSpot() || eventData.pointerDrag.GetComponent<ItemIcon>().GetWasInUseSpot())
                {
                    // Only make a use spot swap if the items are the same type
                    if (currentIcon.GetComponent<ItemIcon>().GetWeaponType() == eventData.pointerDrag.GetComponent<ItemIcon>().GetWeaponType())
                    {
                        SwapSlots(currentIcon, eventData.pointerDrag);
                    }
                    else
                    {
                        // Deny the new object the ability to move
                        blockMovement = true;
                    }
                }
                else // Not a use spot situation; swap away!
                {
                    SwapSlots(currentIcon, eventData.pointerDrag);
                }
            }

            // If it's a use slot, only accept the right items
            if (useSlot == true)
            {
                if (eventData.pointerDrag.GetComponent<ItemIcon>().GetWeaponType() == "talisman" && player.GetComponent<Health>().GetIsHurt())
                {
                    hud.GetComponent<HUD>().EnableWarningPanel(true);
                    hud.GetComponent<HUD>().SetWarningText("You cannot equip a talisman while hurt!");
                }
                else if (eventData.pointerDrag.GetComponent<ItemIcon>().GetWeaponType() == useSlotType)
                {
                    // If this is a sword, set the item's sprite to be the player image's sprite
                    if (useSlotType == "sword")
                    {
                        gameController.GetComponent<Inventory>().FillPlayerWeaponImage(eventData.pointerDrag.GetComponent<ItemIcon>().GetSubIconSprite(), eventData.pointerDrag.GetComponent<ItemIcon>().GetSubIconAngle());
                    }

                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

                    // Item has landed, so it has a spot
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetHasASpot(true);
                    // Additionally, this is a use spot, so set that as well

                    eventData.pointerDrag.GetComponent<ItemIcon>().SetInUseSpot(true);

                    // Set the item's last stable position to be this one (and adjust the index)
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetLastPosition(new Vector2(xPos, yPos));

                    // Adjust the index; free up the last one, update the new one
                    gameController.GetComponent<Inventory>().SetSlotTrackerAt(eventData.pointerDrag.GetComponent<ItemIcon>().GetLocationIndex(), true);
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetLocationIndex(slotIndexValue);
                    gameController.GetComponent<Inventory>().SetSlotTrackerAt(slotIndexValue, false);

                    // Set a new current slot for the item icon
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetCurrentSlot(gameObject);

                    // Store the current icon
                    currentIcon = eventData.pointerDrag;

                    // This slot is now occupied; set it as such!
                    isOpen = false;
                }
            }
            else // If it's not a use slot, accept any item
            {
                if (!blockMovement)
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetHasASpot(true);

                    // Set the item's last stable position to be this one
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetLastPosition(new Vector2(xPos, yPos));

                    // Update the index slots; last one is now open, new one is now closed
                    gameController.GetComponent<Inventory>().SetSlotTrackerAt(eventData.pointerDrag.GetComponent<ItemIcon>().GetLocationIndex(), true);
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetLocationIndex(slotIndexValue);
                    gameController.GetComponent<Inventory>().SetSlotTrackerAt(slotIndexValue, false);

                    // Set a new current slot for the item icon
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetCurrentSlot(gameObject);

                    // No longer "was" in use spot anytime recently
                    eventData.pointerDrag.GetComponent<ItemIcon>().SetWasInUseSpot(false);

                    // Store the current icon
                    currentIcon = eventData.pointerDrag;

                    // This slot is now occupied; set it as such!
                    isOpen = false;
                }

                // Reset
                blockMovement = false;
            }
        }
    }

    // Get this slot's rect transform
    public Vector2 GetRectTransform()
    {
        return new Vector2(xPos, yPos);
    }

    // Get whether or not this slot is open
    public bool IsSlotOpen()
    {
        return isOpen;
    }

    // Set whether or not this slot is open
    public void SetSlotOpen(bool value)
    {
        isOpen = value;
    }

    // Function for getting the index value of the slot
    public int GetSlotIndexValue()
    {
        return slotIndexValue;
    }

    // Function for setting the current icon of this slot (for initial acquisition of item)
    public void SetCurrentIcon(GameObject icon)
    {
        currentIcon = icon;
    }

    // Function for getting the use slot type
    public string GetUseSlotType()
    {
        return useSlotType;
    }

    // Function for getting whether or not this slot is a useSlot
    public bool GetIsUseSlot()
    {
        return useSlot;
    }
}
