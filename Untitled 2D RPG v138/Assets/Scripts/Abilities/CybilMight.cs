using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CybilMight : MonoBehaviour
{
    [Header("Alternate Animator Controllers")]
    [SerializeField] RuntimeAnimatorController regularController;
    [SerializeField] RuntimeAnimatorController cybilMightController;

    [Header("Cybil's Might Sword")]
    [SerializeField] GameObject cybilSword;

    // Flag for player's Cybil's Might status
    bool isMighted = false;

    // Player references
    GameObject player;
    Animator playerAnim;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerAnim = player.GetComponent<Animator>();
    }

    // Public Functions

    // Function that activates the Cybil's Might ability for the player
    public void ActivateCybilMight()
    {
        // Set the player's Cybil's Might flag to true
        isMighted = true;

        // Activate the transition visual
        playerAnim.SetBool("cybilMightActive", true);

        // Weapon gets activated halfway through the animation

        // Animator controller gets switched by the SwitchAnimatorControllers() animation event function
    }

    public void DeactivateCybilMight()
    {
        // Set the player' Cybil's Might flag to false 
        isMighted = false;

        // Restore the player's current weapon

        // Activate the transition visual
        playerAnim.SetBool("cybilMightActive", false);

        // Animator controller gets switched by the SwitchAnimatorControllers() animation event function
    }

    // Function that actually activates the animator controller change during the transition animation
    // (Gets called by an animation event on the transition animation)
    public void SwitchAnimatorControllers()
    {
        // Switching back to regular controller
        if (playerAnim.runtimeAnimatorController == cybilMightController)
        {
            playerAnim.runtimeAnimatorController = regularController;
            playerAnim.SetBool("cybilMightActive", false);

            // Dequip Cybil's might, use the player's stored weapon
            player.GetComponent<PlayerController>().EquipWeapon(player.GetComponent<PlayerController>().GetCurrentWeaponSprite(),
                                                                   player.GetComponent<PlayerController>().GetCurrentWeaponSpriteType(),
                                                                   player.GetComponent<PlayerController>().GetCurrentWeaponDamage());

            // Deactivate the weapon if the player did not have a weapon beforehand
            if (!player.GetComponent<PlayerController>().GetWeaponEnabled())
            {
                player.GetComponent<PlayerController>().DeactivateWeapon();
            }
        }
        // Switching to the Cybil Might's controller
        else if (playerAnim.runtimeAnimatorController == regularController)
        {
            playerAnim.runtimeAnimatorController = cybilMightController;
            playerAnim.SetBool("cybilMightActive", true);

            // Equip the player with Cybil's Might - indicate this is a special pass so it doesn't get stored
            player.GetComponent<PlayerController>().EquipWeapon(cybilSword.GetComponent<Weapon>().GetWeaponSprite(),
                                                                cybilSword.GetComponent<Weapon>().GetWeaponSpriteType(),
                                                                cybilSword.GetComponent<Weapon>().GetWeaponDamage());
        }
        player.GetComponent<PlayerController>().SetSpawnAnimations();
        MaintainAnimBools();
    }

    // Function that maintains all of the active Animator booleans when the controller is switched
    public void MaintainAnimBools()
    {
        if (GetComponent<Invisibility>().GetIsInvisible())
        {
            playerAnim.SetBool("invisibilityActive", true);
        }
        if (isMighted)
        {
            playerAnim.SetBool("cybilMightActive", true);
        }
    }

    // Function that gets the player's Cybil Might flag
    public bool GetIsMighted()
    {
        return isMighted;
    }

    // Function that sets the player's Cybil Might flag
    public void SetIsMighted(bool value)
    {
        isMighted = value;
    }
}
