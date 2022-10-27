using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that keeps count of how many arrows the player has
public class ArrowCount : MonoBehaviour
{
    // Had to instance this class in order to get the clone to not update the HUD to the 
    // default number of arrowsp
    public static ArrowCount instance;

    public int startingArrowCount = 30;

    // What the player has vs. what they can have max
    private int arrowCount;

    GameObject hud;

    // PRIVATE FUNCTIONS

    private void Start()
    {
        // Keep this object in all scenes; destroy clones
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        arrowCount = startingArrowCount;

        // Set initial arrow count text
        hud = GameObject.FindWithTag("HUD");
        hud.GetComponent<HUD>().SetArrowCountText(arrowCount);
    }

    // PUBLIC FUNCTIONS

    // Function that subtracts an arrow from the player's arrow count
    public void SubtractArrow()
    {
        arrowCount -= 1;
    }

    // Function that adds arrows to the player's arrow count 
    public void AddArrows(int amount)
    {
        // Increment arrows by amount
        arrowCount += amount;

        // Update the arrow count text
        hud.GetComponent<HUD>().SetArrowCountText(arrowCount);
    }

    // Function that returns the player's current number of arrows
    public int GetArrowCount()
    {
        return arrowCount;
    }
}
