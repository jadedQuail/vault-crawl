using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator enemyAnimator;

    [Header("")]
    // maxHealth is the net maximum health, includes buffs.
    [SerializeField] float maxHealth;
    [SerializeField] bool isEnemy;
    [SerializeField] bool isBaron;
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject damageText;
    [SerializeField] GameObject xpText;

    [SerializeField] float healthRegenTime = 2f;
    [SerializeField] float healthCooldownTime = 5f;
    [SerializeField] float healthRegenAmount = 5f;

    [SerializeField] BaronHealthbar baronHealthbar;

    // Max health before any buffs
    float rawMaxHealth;

    float healthRegenCounter;

    float health;

    GameObject player;
    GameObject gameController;
    GameObject hud;

    // Bool for whether or not the player is hurt; false if health is full, true if it is not.
    bool isHurt = false;

    bool playerDead = false;

    bool playerDiedByTime = false;

    // The death blend tree has 4 animations, and all 4 anim events get called at the same time
    // This boolean is used to ensure that the death function only gets called ONCE
    bool blockAnimRecall = false;

    // Start is called before the first frame update
    void Start()
    {
        rawMaxHealth = maxHealth;
        health = maxHealth;
        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
        gameController = GameObject.FindWithTag("GameController");

        if (isEnemy && isBaron)
        {
            baronHealthbar.SetSize(health, maxHealth);
        }

        // This creates a delay for health regeneration when it is first used
        healthRegenCounter = healthRegenTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDiedByTime) { health = 0; }

        if (health <= 0)
        {
            health = 0;

            if (isEnemy)
            {
                // Shut down the enemy, culminating in death and deletion
                EnemyShutDown();
            }
            
            if (isPlayer && !playerDead)
            {
                // Shut down the player, culminating in death and deletion
                PlayerShutdown();
                playerDead = true;
            }
        }
        else
        {
            // Regenerate the player's health, as conditions apply
            RegenerateHealth();
        }

        // Check if the player is hurt
        if (isPlayer)
        {
            // Health is full
            if (health == maxHealth)
            {
                isHurt = false;
            }
            else
            {
                isHurt = true;
            }
        }
    }

    // PRIVATE FUNCTIONS

    // Function for regenerating player health
    private void RegenerateHealth()
    {
        if (isPlayer)
        {
            if (health < maxHealth)
            {
                // Start subtracting time from the health counter, if that's allowed
                if (gameController.GetComponent<GameController>().GetCanCountdown())
                {
                    healthRegenCounter -= Time.deltaTime;
                }

                // If our counter has reached 0, then we can now heal the player and reset the counter.
                if (healthRegenCounter <= 0f)
                {
                    healthRegenCounter = healthRegenTime;
                    // Player can't heal in menus, must be in open world
                    if (gameController.GetComponent<GameController>().GetCanOpenMenus())
                    {
                        // Only add the amount if it doesn't go over the max health
                        if (health + healthRegenAmount > maxHealth)
                        {
                            health = maxHealth;
                        }
                        else
                        {
                            health += healthRegenAmount;
                        }
                    }
                }
            }
        }
    }

    // Function that prepares an enemy for death
    private void EnemyShutDown()
    {
        // If the enemy is frozen while dying, un-freeze it so it can die
        GetComponent<EnemyController>().ToggleFreeze(false);

        // Play the death animation
        enemyAnimator.SetTrigger("die");

        // Indicate that the enemy is shutting down
        GetComponent<EnemyController>().SetShuttingDown(true);

        // The enemy can no longer attack
        GetComponent<EnemyController>().SetCanAttack(false);

        // Prevent this enemy from being able to move
        GetComponent<EnemyController>().SetCanMove(false);

        // If this is a baron, disable its laser_charging child
        if (isBaron)
        {
            if (GetComponentInChildren<LaserCharging>() != null)
            {
                GetComponentInChildren<LaserCharging>().DisableLaserCharging();
            }
        }

        // Deactivate the enemy in the player's enemy list immediately,
        // so that way the player can start hitting the next enemy
        GameObject[] enemyArray = gameController.GetComponent<GameController>().GetEnemyList();
        int indexValue = System.Array.IndexOf(enemyArray, gameObject);
        gameController.GetComponent<GameController>().NullifyEnemyAtIndex(indexValue);
    }

    // Function that prepares the player for death
    private void PlayerShutdown()
    {
        // Disable the player's weapon
        player.GetComponent<PlayerController>().ToggleWeapon("disable");

        // Prevent the player from being able to move
        player.GetComponent<PlayerController>().SetCanMove(false);

        // Kill the velocity on the player's Rigidbody2D
        player.GetComponent<PlayerController>().StopVelocity();

        // Set the player's revival status to false
        GameObject.FindWithTag("Master").GetComponent<Master>().SetPlayerRevived(false);

        // Reset the dungeon's timer
        GameObject.FindWithTag("DungeonManager").GetComponent<DungeonTimer>().SetTimerSet(false);

        // Reset the dungeon's mob count
        hud.GetComponent<HUD>().SetEnemyCountText(0); // Input doesn't matter - text will be lines
        hud.GetComponent<HUD>().SetBaronCountText(0);

        player.GetComponent<Animator>().SetTrigger("playerDie");
    }

    // Function that finalizes the death of the enemy
    private void EnemyDie()
    {
        // Reset locked target
        player.GetComponent<PlayerController>().ResetLockedTarget();
        
        // If this is a Baron, destroy parent object, remove from list; otherwise, just destroy the object
        if (isBaron)
        {
            if (GameObject.FindWithTag("DungeonManager") != null)
            {
                GameObject dungeonManager = GameObject.FindWithTag("DungeonManager");
                dungeonManager.GetComponent<BaronLoader>().RemoveBaronFromList(gameObject);
                dungeonManager.GetComponent<DungeonTimer>().DecrementBaronCount();
            }
            
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Spawn a coin, set its value
        gameController.GetComponent<DropController>().DropCoin(gameObject);

        // Give the player the enemy's mana bonus
        player.GetComponent<Mana>().IncrementMana(GetComponent<EnemyController>().GetManaReward());

        // Drop the enemy's items
        GetComponent<EnemyController>().DropItems();
    }

    // Function that calculates the net max health, accounting for active buffs
    // (Always called after any max health change)
    private void CalculateNetMaxHealth()
    {
        // Store buff variables from the PlayerProgression component
        int currBuffVal = GetComponent<PlayerProgression>().GetCurrentBuffValue();
        string currBuffType = GetComponent<PlayerProgression>().GetCurrentBuffType();

        if (currBuffVal != 0 && currBuffType == "fixed_HP")
        {
            maxHealth = rawMaxHealth + currBuffVal;
        }
        else if (currBuffVal != 0 && currBuffType == "percent_HP")
        {
            maxHealth = Mathf.Floor(rawMaxHealth * (1.00f + ((float)currBuffVal / 100f)));
        }
        else  // No buffs on; net max health is just the raw max health.
        { 
            maxHealth = rawMaxHealth;
        }
    }

    // PUBLIC FUNCTIONS

    // Function for losing health
    public void LoseHealth(float healthToLose)
    {
        if (health >= 1)
        {
            health -= healthToLose;

            if (isEnemy && isBaron)
            {
                baronHealthbar.SetSize(health, maxHealth);
            }

            // TESTING
            ShowText(healthToLose.ToString(), damageText);

            // Reset the healthRegenCounter, but set it to "healthCooldownTime" because an enemy hit
            // means a longer cooldown time must occur.
            healthRegenCounter = healthCooldownTime;
        }
    }

    // Function for getting the health of the particular entity
    public float GetHealth()
    {
        return health;
    }

    // Function for getting health information (for UI purposes)
    public (float, float) GetHealthInfo()
    {
        return (health, maxHealth);
    }

    // Function for getting max health information
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Function for getting raw max health information
    public float GetRawMaxHealth()
    {
        return rawMaxHealth;
    }

    // Function for incrementing max health (i.e. leveling up)
    public void IncrementMaxHealth(int value)
    {
        // Set the raw max health
        rawMaxHealth += value;

        // Re-calculate net
        CalculateNetMaxHealth();
    }

    public void SetMaxHealth(int value)
    {
        // Set the raw max health
        rawMaxHealth = value;

        // Re-calculate net
        CalculateNetMaxHealth();
    }

    // Function for setting the health of the particular entity
    public void SetHealth(float healthToSet)
    {
        health = healthToSet;
    }

    // Function that triggers at the end of the enemy's die animation
    public void DeathAnimation(string message)
    {
        if (message == "die" && blockAnimRecall == false)
        {
            // Kill the enemy, show and add experience to the player
            EnemyDie();
            ShowText("+" + GetComponent<EnemyController>().GetEnemyXP().ToString(), xpText);
            player.GetComponent<PlayerLeveling>().AddXP(GetComponent<EnemyController>().GetEnemyXP());

            // Prevent the death animation from occurring again
            blockAnimRecall = true;
        }

        if (message == "playerDie")
        {
            // Activate the game over panel
            if (playerDiedByTime)
            {
                hud.GetComponent<HUD>().OpenGameOverPanel("Time's up!");
            }
            else
            {
                hud.GetComponent<HUD>().OpenGameOverPanel("You died!");
            }
        }
    }

    // Function that triggers damage text
    public void ShowText(string theText, GameObject textItem)
    {
        GameObject instantiatedText = Instantiate(textItem, transform.position, Quaternion.identity);
        instantiatedText.GetComponentInChildren<TextMesh>().text = theText;
    }

    // Function that sets the "playerDead" flag
    public void SetPlayerDead(bool value)
    {
        playerDead = value;
    }

    // Function that gets the "playerDead" flag
    public bool GetPlayerDead()
    {
        return playerDead;
    }

    // Function that sets the "playerDeadByTime" flag
    public void SetPlayerDiedByTime(bool value)
    {
        playerDiedByTime = value;
    }

    // Function that gets whether or not the player is hurt
    public bool GetIsHurt()
    {
        return isHurt;
    }
}
