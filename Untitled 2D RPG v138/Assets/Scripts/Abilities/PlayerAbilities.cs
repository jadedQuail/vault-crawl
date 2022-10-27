using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    // Abilities' keycodes
    [Header("Abilities Key Assignment")]
    [SerializeField] KeyCode ability_0;
    [SerializeField] KeyCode ability_1;
    [SerializeField] KeyCode ability_2;
    [SerializeField] KeyCode ability_3;

    [Header("Ability Reload Times")]
    [SerializeField] float ability_0_reload_time;
    [SerializeField] float ability_1_reload_time;
    [SerializeField] float ability_2_reload_time;
    [SerializeField] float ability_3_reload_time;

    [Header("Ability Use Times")]
    [SerializeField] float ability_0_usetime;
    [SerializeField] float ability_1_usetime;
    [SerializeField] float ability_2_usetime;
    [SerializeField] float ability_3_usetime;


    private string equippedAbil0 = "Invisibility";
    private string equippedAbil1 = "Cybil_Might";
    private string equippedAbil2 = "Trailblazer";    // Other ability is "Hyper_Speed"
    private string equippedAbil3 = "Flash_Freeze";

    // Use counters
    float ability_0_usecounter;
    float ability_1_usecounter;
    float ability_2_usecounter;
    float ability_3_usecounter;

    // "In use" flags
    bool ability_0_in_use = false;
    bool ability_1_in_use = false;
    bool ability_2_in_use = false;
    bool ability_3_in_use = false;

    // Player flags for being able to use abilities
    private bool canUseAbil0 = true;
    private bool canUseAbil1 = true;
    private bool canUseAbil2 = true;
    private bool canUseAbil3 = true;

    bool ability_0_reset = false;
    bool ability_1_reset = false;
    bool ability_2_reset = false;
    bool ability_3_reset = false;

    float ability_0_counter = 0f;
    float ability_1_counter = 0f;
    float ability_2_counter = 0f;
    float ability_3_counter = 0f;

    float ability_0_ratio = 0f;
    float ability_1_ratio = 0f;
    float ability_2_ratio = 0f;
    float ability_3_ratio = 0f;

    bool ability_0_begin;
    bool ability_1_begin;
    bool ability_2_begin;
    bool ability_3_begin;

    bool abilitiesPaused = false;
    bool cooldownsPaused = false;

    // Major object references
    GameObject hud;
    GameObject abilityManager;

    private void Start()
    {
        hud = GameObject.FindWithTag("HUD");
        abilityManager = GameObject.FindWithTag("AbilityManager");
    }

    private void Update()
    {
        // Use abilities if key is pressed
        if (!abilitiesPaused)
        {
            if (Input.GetKeyDown(ability_0) && canUseAbil0) { UseAbility(0, equippedAbil0); }
            if (Input.GetKeyDown(ability_1) && canUseAbil1) { UseAbility(1, equippedAbil1); }
            if (Input.GetKeyDown(ability_2) && canUseAbil2) { UseAbility(2, equippedAbil2); }
            if (Input.GetKeyDown(ability_3) && canUseAbil3) { UseAbility(3, equippedAbil3); }
        }

        // Constantly check and see if a visual cooldown needs to occur
        CheckForCooldown();

        // Constantly check and see if abilities are in the middle of their active "usetime"
        CheckForUse();
    }

    // Function that "uses" an ability
    private void UseAbility(int abilitySlot, string equippedAbil)
    {
        // Set the used ability to "in use"
        if (abilitySlot == 0) { ability_0_in_use = true; }
        else if (abilitySlot == 1) { ability_1_in_use = true; }
        else if (abilitySlot == 2) { ability_2_in_use = true; }
        else if (abilitySlot == 3) { ability_3_in_use = true; }

        if (equippedAbil == "Invisibility")
        {
            abilityManager.GetComponent<Invisibility>().ActivateInvisibility();
        }
        else if (equippedAbil == "Cybil_Might")
        {
            abilityManager.GetComponent<CybilMight>().ActivateCybilMight();
        }
        else if (equippedAbil == "Trailblazer")
        {
            abilityManager.GetComponent<Trailblazer>().ActivateTrailblazer();
        }
        else if (equippedAbil == "Hyper_Speed")
        {
            abilityManager.GetComponent<HyperSpeed>().ActivateHyperSpeed();
        }
        else if (equippedAbil == "Flash_Freeze")
        {
            // With flash freeze, have to pass usetime to know when to unfreeze different groups of enemies
            abilityManager.GetComponent<FlashFreeze>().ActivateFlashFreeze(ability_3_usetime);
        }

        // Start the cooldown visual for the ability that was used
        if (abilitySlot == 0) { ability_0_reset = true; }
        else if (abilitySlot == 1) { ability_1_reset = true; }
        else if (abilitySlot == 2) { ability_2_reset = true; }
        else if (abilitySlot == 3) { ability_3_reset = true; }
    }

    // Function that "deactivates" an ability after use is complete
    private void DeactivateAbility(int abilitySlot, string equippedAbil)
    {
        // Set the used ability to no longer in use
        if (abilitySlot == 0) { ability_0_in_use = false; }
        else if (abilitySlot == 1) { ability_1_in_use = false; }
        else if (abilitySlot == 2) { ability_2_in_use = false; }
        else if (abilitySlot == 3) { ability_3_in_use = false; }

        if (equippedAbil == "Invisibility")
        {
            abilityManager.GetComponent<Invisibility>().DeactivateInvisibility();
        }
        else if (equippedAbil == "Cybil_Might")
        {
            abilityManager.GetComponent<CybilMight>().DeactivateCybilMight();
        }
        else if (equippedAbil == "Trailblazer")
        {
            abilityManager.GetComponent<Trailblazer>().DeactivateTrailblazer();
        }
        else if (equippedAbil == "Hyper_Speed")
        {
            abilityManager.GetComponent<HyperSpeed>().DeactivateHyperSpeed();
        }
        else if (equippedAbil == "Flash_Freeze")
        {
            // Flash freeze deactivates by itself, from the enemy's side
        }
    }

    // Function that checks if cooldowns should occur, based on the "reset" booleans
    private void CheckForCooldown()
    {
        if (ability_0_reset)
        {
            // Reset ability 0 for a new cooldown
            ability_0_counter = 0;
            ability_0_begin = true;
            ability_0_reset = false;

            // Player can't use the ability until the cooldown is done
            canUseAbil0 = false;

            // Enable the cooldown image
            hud.GetComponent<HUD>().EnableCooldownImage(0, true);
        }

        if (ability_1_reset)
        {
            // Reset ability 0 for a new cooldown
            ability_1_counter = 0;
            ability_1_begin = true;
            ability_1_reset = false;

            // Player can't use the ability until the cooldown is done
            canUseAbil1 = false;

            // Enable the cooldown image
            hud.GetComponent<HUD>().EnableCooldownImage(1, true);
        }

        if (ability_2_reset)
        {
            // Reset ability 0 for a new cooldown
            ability_2_counter = 0;
            ability_2_begin = true;
            ability_2_reset = false;

            // Player can't use the ability until the cooldown is done
            canUseAbil2 = false;

            // Enable the cooldown image
            hud.GetComponent<HUD>().EnableCooldownImage(2, true);
        }

        if (ability_3_reset)
        {
            // Reset ability 0 for a new cooldown
            ability_3_counter = 0;
            ability_3_begin = true;
            ability_3_reset = false;

            // Player can't use the ability until the cooldown is done
            canUseAbil3 = false;

            // Enable the cooldown image
            hud.GetComponent<HUD>().EnableCooldownImage(3, true);
        }

        // ------------------------------------------- //

        if (!cooldownsPaused)
        {
            if (ability_0_begin)
            {
                ability_0_counter += Time.deltaTime;
                ability_0_ratio = ability_0_counter / ability_0_reload_time;
                hud.GetComponent<HUD>().SetCooldownImageFill(0, 1 - ability_0_ratio);
            }

            if (ability_1_begin)
            {
                ability_1_counter += Time.deltaTime;
                ability_1_ratio = ability_1_counter / ability_1_reload_time;
                hud.GetComponent<HUD>().SetCooldownImageFill(1, 1 - ability_1_ratio);
            }

            if (ability_2_begin)
            {
                ability_2_counter += Time.deltaTime;
                ability_2_ratio = ability_2_counter / ability_2_reload_time;
                hud.GetComponent<HUD>().SetCooldownImageFill(2, 1 - ability_2_ratio);
            }

            if (ability_3_begin)
            {
                ability_3_counter += Time.deltaTime;
                ability_3_ratio = ability_3_counter / ability_3_reload_time;
                hud.GetComponent<HUD>().SetCooldownImageFill(3, 1 - ability_3_ratio);
            }
        }

        // ------------------------------------------- //

        // Cooldown complete
        if (ability_0_counter >= ability_0_reload_time)
        {
            ability_0_begin = false;

            // Player can use the ability again
            canUseAbil0 = true;
        }

        if (ability_1_counter >= ability_1_reload_time)
        {
            ability_1_begin = false;

            // Player can use the ability again
            canUseAbil1 = true;
        }

        if (ability_2_counter >= ability_2_reload_time)
        {
            ability_2_begin = false;

            // Player can use the ability again
            canUseAbil2 = true;
        }

        if (ability_3_counter >= ability_3_reload_time)
        {
            ability_3_begin = false;

            // Player can use the ability again
            canUseAbil3 = true;
        }
    }

    // Function that keeps track of how long abilities should be used
    private void CheckForUse()
    {
        if (!abilitiesPaused)
        {
            if (ability_0_in_use == true)
            {
                ability_0_usecounter += Time.deltaTime;
                if (ability_0_usecounter >= ability_0_usetime)
                {
                    DeactivateAbility(0, equippedAbil0);
                    ability_0_usecounter = 0f;
                }
            }

            if (ability_1_in_use == true)
            {
                ability_1_usecounter += Time.deltaTime;
                if (ability_1_usecounter >= ability_1_usetime)
                {
                    DeactivateAbility(1, equippedAbil1);
                    ability_1_usecounter = 0f;
                }
            }

            if (ability_2_in_use == true)
            {
                ability_2_usecounter += Time.deltaTime;
                if (ability_2_usecounter >= ability_2_usetime)
                {
                    DeactivateAbility(2, equippedAbil2);
                    ability_2_usecounter = 0f;
                }
            }

            if (ability_3_in_use == true)
            {
                ability_3_usecounter += Time.deltaTime;
                if (ability_3_usecounter >= ability_3_usetime)
                {
                    DeactivateAbility(3, equippedAbil3);
                    ability_3_usecounter = 0f;
                }
            }
        }
    }

    // Function that equips a new ability to one of the player's slots
    public void EquipAbility(int abilNum, string abilName)
    {
        if (abilNum == 0) { equippedAbil0 = abilName; }
        else if (abilNum == 1) { equippedAbil1 = abilName; }
        else if (abilNum == 2) { equippedAbil2 = abilName; }
        else if (abilNum == 3) { equippedAbil3 = abilName; }
    }

    // Function that freezes all abilities' active timers and cooldowns
    public void FreezeAbilities()
    {
        abilitiesPaused = true;
        cooldownsPaused = true;
    }

    // Function that unfreezes all abilities' active timers and cooldowns
    public void UnfreezeAbilities()
    {
        abilitiesPaused = false;
        cooldownsPaused = false;
    }

    // Function that calls "SwitchAnimatorControllers()" on the Cybil's Might script, from the animation event
    public void Call_SwitchAnimatorControllers()
    {
        abilityManager.GetComponent<CybilMight>().SwitchAnimatorControllers();
    }

    // Function that resets all abilities and cooldowns
    public void ResetAllAbilities()
    {
        // Reset all the cooldowns
        ability_0_begin = false;
        ability_1_begin = false;
        ability_2_begin = false;
        ability_3_begin = false;

        // Allow for ability use
        canUseAbil0 = true;
        canUseAbil1 = true;
        canUseAbil2 = true;
        canUseAbil3 = true;

        // Fill the cooldown image to full
        hud.GetComponent<HUD>().SetCooldownImageFill(0, 0);
        hud.GetComponent<HUD>().SetCooldownImageFill(1, 0);
        hud.GetComponent<HUD>().SetCooldownImageFill(2, 0);
        hud.GetComponent<HUD>().SetCooldownImageFill(3, 0);

        // Deactivate abilities
        DeactivateAbility(0, equippedAbil0);
        DeactivateAbility(1, equippedAbil1);
        DeactivateAbility(2, equippedAbil2);
        DeactivateAbility(3, equippedAbil3);

        // Reset use counters
        ability_0_usecounter = 0f;
        ability_1_usecounter = 0f;
        ability_2_usecounter = 0f;
        ability_3_usecounter = 0f;
    }
}
