using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    private int playerMoney = 0;

    // PUBLIC FUNCTIONS

    // Function that adds to player's money
    public void AddToPlayerMoney(int amount)
    {
        playerMoney += amount;
    }

    // Function that subtracts from the player's money
    public void SubtractFromPlayerMoney(int amount)
    {
        playerMoney -= amount;
    }

    // Function that gets the player's money
    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    // Function that sets the player's money
    public void SetPlayerMoney(int amount)
    {
        playerMoney = amount;
    }
}
