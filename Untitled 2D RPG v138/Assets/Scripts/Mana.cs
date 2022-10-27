using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField] float maxMana;
    [SerializeField] float mana;

    [SerializeField] float manaRegenTime = 2f;
    [SerializeField] float manaCooldownTime = 5f;
    [SerializeField] float manaRegenAmount = 1f;
    float manaRegenCounter;

    GameObject gameController;

    // Private Functions

    private void Start()
    {
        gameController = GameObject.FindWithTag("GameController");

        // This creates a delay for mana regeneration when it is first used
        manaRegenCounter = manaRegenTime;
    }

    private void Update()
    {
        RegenerateMana();
    }

    // Function for regenerating mana
    private void RegenerateMana()
    {
        if (mana < maxMana)
        {
            // Start subtracting time from the health counter, if that's allowed
            if (gameController.GetComponent<GameController>().GetCanCountdown())
            {
                manaRegenCounter -= Time.deltaTime;
            }

            // If our counter has reached 0, then we can now heal the player and reset the counter.
            if (manaRegenCounter <= 0f)
            {
                manaRegenCounter = manaRegenTime;
                // Player can't heal in menus, must be in open world
                if (gameController.GetComponent<GameController>().GetCanOpenMenus())
                {
                    mana += manaRegenAmount;
                }
            }
        }
    }

    // Public Functions

    // Function that returns the player's mana information
    public (float, float) GetManaInfo()
    {
        return (mana, maxMana);
    }

    // Function that increments the player's mana
    public void IncrementMana(int value)
    {
        if (mana + value < maxMana)
        {
            mana += value;
        }
        else
        {
            mana = maxMana;
        }
    }

    // Function that decrements the player's mana
    public void DecrementMana(int value)
    {
        if (mana - value <= 0f)
        {
            mana = 0;
        }
        else
        {
            mana -= value;
        }
    }

    // Function that returns the current mana 
    public float GetCurrentMana()
    {
        return mana;
    }
}
