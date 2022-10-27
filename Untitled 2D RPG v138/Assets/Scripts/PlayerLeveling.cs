using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeveling : MonoBehaviour
{
    // NOTE: The experience needed for the player to level up at any given time is equal to:

        // playerLevels[currentLevel-1]

    [SerializeField] int[] playerLevels;
    [SerializeField] int numOfLevels = 49;

    // This has to be 1 instead of 0 or else Unity loses its stupid mind
    int netXP = 1;

    int currentLevel;
    int nextLevel;

    GameObject hud;

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.FindWithTag("HUD");

        // Set the size of playerLevels
        playerLevels = new int[numOfLevels];
        PopulateLevelsWithXP();

        // Set the first level
        currentLevel = 1;
        nextLevel = 2;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForLevelUp();
    }

    // PRIVATE FUNCTIONS

    // Function that uses an equation to populate how much experience is needed to get to the next level
    private void PopulateLevelsWithXP()
    {
        for (int i = 0; i < playerLevels.Length; i++)
        {
            if (i == 0) 
            {
                playerLevels[i] = 200;
            }
            else
            {
                playerLevels[i] = (int)((playerLevels[i - 1] + 100) * 1.17f);
            }
        }
    }

    // Function that checks and sees if the player has leveled up
    private void CheckForLevelUp()
    {
        if (netXP >= playerLevels[currentLevel - 1] && currentLevel < playerLevels.Length)
        {
            // Level the player up
            LevelUp();

            // Open up the level up panel
            hud.GetComponent<HUD>().EnableLevelUpPanel(true);
        }
    }

    // Function that levels the player up
    private void LevelUp()
    {
        currentLevel += 1;
        nextLevel += 1;

        // Give the player 10 more health
        // (Placeholder for stat increases)
        GetComponent<Health>().IncrementMaxHealth(10);

        // Give the player full health again
        GetComponent<Health>().SetHealth(GetComponent<Health>().GetMaxHealth());
    }

    // PUBLIC FUNCTIONS

    // Function for getting XP info (for HUD)
    public (int, int, int) GetXPInfo()
    {
        if (currentLevel == 1)
        {
            return (netXP, 200, 0);
        }
        else
        {
            return (netXP, playerLevels[currentLevel - 1], playerLevels[currentLevel - 2]);
        }
    }

    // Function for getting the player's current level
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // Function for setting the player's current level
    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    // Function for getting the player's next level
    public int GetNextLevel()
    {
        return nextLevel;
    }

    // Function for setting the player's next level
    public void SetNextLevel(int level)
    {
        nextLevel = level;
    }

    // Function that gets the netXP
    public int GetNetXP()
    {
        return netXP;
    }

    // Function that sets the netXP
    public void SetNetXP(int xp)
    {
        netXP = xp;
    }

    // Function that adds to the player's net XP
    public void AddXP(int xp)
    {
        netXP += xp;
    }
}
