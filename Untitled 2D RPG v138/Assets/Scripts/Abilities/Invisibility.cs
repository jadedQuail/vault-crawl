using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : MonoBehaviour
{
    // Player reference
    GameObject player;

    // Flag for player's invisibility
    bool isInvisible = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");    
    }

    // Public Functions

    // Function that implements turning invisible for the player
    public void ActivateInvisibility()
    {
        // Set the player's invsibility flag to true
        isInvisible = true;

        // Make the player transparent by transitioning their alpha value down on the "Color" animation layer
        player.GetComponent<Animator>().SetBool("invisibilityActive", true);
    }

    // Function that deactivates turning invisible for the player
    public void DeactivateInvisibility()
    {
        // Set the player's invisibility flag to false
        isInvisible = false;

        // Set the player back to full color on the "Hurt / Invisible" animation layer
        player.GetComponent<Animator>().SetBool("invisibilityActive", false);
    }

    // Function that gets the player's invisibility flag
    public bool GetIsInvisible()
    {
        return isInvisible;
    }

    // Function that sets the player's invisibility flag
    public void SetIsInvisible(bool value)
    {
        isInvisible = value;
    }
}
