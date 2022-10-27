using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    [SerializeField] Sprite itemSprite;

    GameObject gameController;

    private void Start()
    {
        gameController = GameObject.FindWithTag("GameController");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Only do something if the inventory is not full
            if (!gameController.GetComponent<Inventory>().GetInventoryFull())
            {
                // Find the next open image slot in the inventory and put this item's sprite there (and its weapon type)
                gameController.GetComponent<Inventory>().FillInventoryImage(itemSprite, GetComponent<Weapon>());

                // Increment the number of items recorded in the inventory
                gameController.GetComponent<Inventory>().IncrementNumberOfItems();

                Destroy(gameObject);
            }
        }
    }
}
