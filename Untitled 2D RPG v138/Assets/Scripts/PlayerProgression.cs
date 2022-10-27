using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    // This is the amount of damage the player does
    // Consists of player's attack, weapon damage, bonuses, etc.
    private float playerDamage;

    // Player's personal attack level (inherent, level-based, not weapon-related)
    [SerializeField] private int playerAttack = 10;

    // Player's weapon damage (whatever he's holding)
    private float currentWeaponDamage;

    // Player's bow damage (net)
    private float currentBowDamage;

    // Player's bow damage (base + bow, no buff)
    private float rawBowDamage;

    // Stores the current buff type and value
    string currentBuffType = "null";
    int currentBuffValue = 0;

    // Stores the base bow damage
    [SerializeField] float baseBowDamage = 10f;

    // Stores the player's base speed
    [SerializeField] float baseSpeed = 10f;

    // PRIVATE FUNCTIONS

    private void Start()
    {
        playerDamage = playerAttack;

        // Default bow
        SetCurrentBowDamage(baseBowDamage);
    }

    // Function that re-calls damage, bow, and health setters to have them re-evaluate buffs
    // (Called after a buff is equipped or de-equipped, hence forcing an adjustment)
    private void ReCallBuffableStats()
    {
        // Re-call damage / health adjustments
        if (currentBuffType == "fixed_attack" || currentBuffType == "percent_attack")
        {
            // By setting the same exact damage again, this function will re-check for the buff and apply accordingly
            SetPlayerDamage(GetCurrentWeaponDamage());
        }
        else if (currentBuffType == "fixed_bow" || currentBuffType == "percent_bow")
        {
            // By setting the same exact damage again, this function will re-check for the buff and apply accordingly
            SetCurrentBowDamage(rawBowDamage);
        }
        else if (currentBuffType == "fixed_HP" || currentBuffType == "percent_HP")
        {
            // Adjust health
            GetComponent<Health>().SetMaxHealth((int)GetComponent<Health>().GetRawMaxHealth());
            GetComponent<Health>().SetHealth(GetComponent<Health>().GetMaxHealth());
        }
        else if (currentBuffType == "percent_speed")
        {
            float speedMultiplier = 1 + (currentBuffValue / 100f);
            GetComponent<PlayerController>().SetMoveSpeed(baseSpeed * speedMultiplier);
        }
    }

    // PUBLIC FUNCTIONS

    // Function that gets the player's attack stat
    public int GetPlayerAttack()
    {
        return playerAttack;
    }

    // Function that sets the player's attack stat
    public void SetPlayerAttack(int attack)
    {
        playerAttack = attack;
    }

    // Function that gets the player's overall damage
    public float GetPlayerDamage()
    {
        return playerDamage;
    }

    // Function that sets the player's overall damage (only needs current weapon damage as input)
    public void SetPlayerDamage(float weaponDam)
    {
        // Set the current weapon damage
        currentWeaponDamage = weaponDam;

        // Calculate player damage first
        playerDamage = playerAttack + currentWeaponDamage;

        // Then multiply by buff (if one is on)
        if (currentBuffValue != 0 && currentBuffType == "fixed_attack")
        {
            playerDamage += currentBuffValue;
        }
        else if (currentBuffValue != 0 && currentBuffType == "percent_attack")
        {
            playerDamage = Mathf.Floor(playerDamage * (1.00f + ((float)currentBuffValue / 100f)));
        }
    }

    // Function that set's the player's speed
    public void SetPlayerSpeed(float speed)
    {

    }

    // Function that gets the player's current weapon damage
    public float GetCurrentWeaponDamage()
    {
        return currentWeaponDamage;
    }

    // Function that sets the player's current weapon damage
    public void SetCurrentWeaponDamage(float value)
    {
        currentWeaponDamage = value;
    }

    // Function that gets the player's current bow damage
    public float GetCurrentBowDamage()
    {
        return currentBowDamage;
    }

    // Function that sets the player's current bow damage
    public void SetCurrentBowDamage(float value)
    {
        // Set the bow's raw damage
        rawBowDamage = value;

        // Set the current damage to be the raw damage, for a start
        currentBowDamage = rawBowDamage;

        // Then multiply by buff (if one is on)
        if (currentBuffValue != 0 && currentBuffType == "fixed_bow")
        {
            currentBowDamage += currentBuffValue;
        }
        else if (currentBuffValue != 0 && currentBuffType == "percent_bow")
        {
            currentBowDamage = Mathf.Floor(currentBowDamage * (1.00f + ((float)currentBuffValue / 100f)));
        }
    }

    // Function that gets the buff type of the currently equipped buff
    public string GetCurrentBuffType()
    {
        return currentBuffType;
    }

    // Function that gets the buff value of the currently equipped buff
    public int GetCurrentBuffValue()
    {
        return currentBuffValue;
    }

    // Function that adds a buff to a player
    public void AddBuff(string buffType, int buffValue)
    {
        // If we're adding a buff, clear out whatever the old buff was (in the case of a slot swap)
        RemoveBuff();

        // Store the current buff type and value
        currentBuffType = buffType;
        currentBuffValue = buffValue;

        // Re-call damage / health adjustments
        ReCallBuffableStats();
    }

    // Function that removes a buff from a player
    public void RemoveBuff()
    {
        // Reset the buff value
        currentBuffValue = 0;

        // Re-call damage / health adjustments
        ReCallBuffableStats();

        // Turn off the buff
        currentBuffType = "null";
    }
}
